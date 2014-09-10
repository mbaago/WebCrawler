using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace Crawler
{
    public class Crawler
    {
        public Crawler(int numFrontQueues)
        {
            MaxNumberOfBackQueues = 3;
            FrontQueues = new Queue<string>[numFrontQueues];
            for (int i = 0; i < FrontQueues.Length; i++)
            {
                FrontQueues[i] = new Queue<string>();
            }
            BackQueues = new Dictionary<string, Queue<string>>();
            DomainsVisited = new Dictionary<string, DateTime>();
            Robots = new Dictionary<string, Tuple<DateTime, string[]>>();

            VisitedURLS = new List<string>();
        }

        public int TotalVisits { get; set; }

        public void CrawlTheWeb(IEnumerable<string> seed)
        {
            foreach (var item in seed)
            {
                AddURLToQueue(item);
            }

            foreach (var url in seed)
            {

                foreach (var link in ExtractLinksFromHTML(url))
                {
                    AddURLToQueue(link);
                }
            }


        }

        private int MaxNumberOfBackQueues { get; set; }
        private Queue<string>[] FrontQueues { get; set; }
        private Dictionary<string, Queue<string>> BackQueues { get; set; }
        private Dictionary<string, DateTime> DomainsVisited { get; set; }
        private Dictionary<string, Tuple<DateTime, string[]>> Robots { get; set; }
        private List<string> VisitedURLS { get; set; }

        public void AddURLToQueue(string url)
        {
            url = MakeURLPretty(url);

            if (!VisitedURLS.Contains(url))
            {
                int q = new Random().Next(0, FrontQueues.Length);
                FrontQueues[q].Enqueue(url);
            }
        }

        public string BackQueueSelector()
        {
            // første gang, alle tomme
            while (BackQueues.Count < MaxNumberOfBackQueues)
            {
                string url = FrontQueueSelector();
                string dom = ExtractDomain(url);
                if (BackQueues.Keys.Contains(dom))
                {
                    BackQueues[dom].Enqueue(url);
                }
                else
                {
                    BackQueues.Add(dom, new Queue<string>());
                    BackQueues[dom].Enqueue(url);
                }
            }

            // Simulate the heap
            var orderedTimes = DomainsVisited
                .Where(d => BackQueues.Keys.Contains(d.Key))
                .OrderByDescending(t => t.Value);

            var oldest = orderedTimes.First();

            while (DateTime.Now - oldest.Value < TimeSpan.FromSeconds(1))
            {
                System.Threading.Thread.Sleep(10);
            }

            var returnURL = BackQueues[oldest.Key].Dequeue();

            while (BackQueues[oldest.Key].Count == 0)
            {
                var item = FrontQueueSelector();
                var dom = ExtractDomain(item);

                if (BackQueues.ContainsKey(dom))
                {
                    BackQueues[dom].Enqueue(item);
                }
                else
                {
                    BackQueues[dom] = new Queue<string>();
                    BackQueues[dom].Enqueue(item);
                }
            }

            return returnURL;
        }

        private string FrontQueueSelector()
        {
            int rand = new Random().Next(0, FrontQueues.Length);

            for (int i = 0; i < FrontQueues.Length; i++)
            {
                if (FrontQueues[rand].Count > 0)
                {
                    return FrontQueues[rand].Dequeue();
                }

                rand = rand++ % FrontQueues.Length;
            }

            throw new Exception("FrontQueues were empty");
        }

        private void VisitDomain(string url)
        {
            string dom = ExtractDomain(url);
            if (DomainsVisited.ContainsKey(dom))
            {
                DomainsVisited[dom] = DateTime.Now;
            }
            else
            {
                DomainsVisited.Add(dom, DateTime.Now);
            }
        }

        public string DownloadHTML(string url)
        {
            url = MakeURLPretty(url);
            WebClient web = new WebClient();
            string dom = ExtractDomain(url);
            if (!Robots.ContainsKey(dom))
            {
                string robot = web.DownloadString("http://" + dom + "/robots.txt");
                Robots.Add(dom, new Tuple<DateTime, string[]>(DateTime.Now, robot.Split('\n')));
                VisitDomain(url);
            }

            if (MayVisit(url))
            {
                VisitDomain(url);
                VisitedURLS.Add(url);
                return web.DownloadString(url);
            }
            else
            {
                return null;
            }
        }

        public string MakeURLPretty(string url)
        {
            var lowURL = url.ToLower();

            if (lowURL.StartsWith("http://www"))
            {
                url = url;
            }
            else if (lowURL.StartsWith("http://"))
            {
                var s = url.Split(new string[] { "//" }, StringSplitOptions.None);
                url = s[0] + "//www." + s[1];
            }
            else if (lowURL.StartsWith("www"))
            {
                url = "http://" + url;
            }
            else
            {
                url = "http://www." + url;
            }

            url = new Uri(url).ToString();

            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        public string ExtractDomain(string url)
        {
            url = MakeURLPretty(url);
            var splitter = url.Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries);
            return splitter[1];
        }

        public IEnumerable<string> ExtractLinksFromHTML(string url)
        {
            var html = DownloadHTML(url);

            if (html == null)
            {
                return Enumerable.Empty<string>();
            }

            var hrefs = html.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

            List<string> urls = new List<string>();

            foreach (var href in hrefs)
            {
                var link = href.Split('\"').First();
                string fullPath = (link.StartsWith("/") ? url : "") + link;
                if (IsValidURL(fullPath))
                {
                    string ur = MakeURLPretty(fullPath);

                    urls.Add(ur);
                }
            }

            return urls;
        }

        public bool IsValidURL(string url)
        {
            return Regex.IsMatch(url, @"http://www\..*");
        }

        public bool MayVisit(string url)
        {
            return !CannotAccess(url, Robots[ExtractDomain(url)].Item2).IsMatch(url); ;
        }


        #region Jaccard
        /// <summary>
        /// Determine if two strings are near-duplicates
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <param name="howClose">When are the two strings near-duplicates (0-1.0).</param>
        /// <returns>True if the strings are near-duplicates, otherwise false.</returns>
        public bool IsNearDuplicates(string s1, string s2, int shingleSize, double howClose)
        {
            double jaccard = Jaccard(s1, s2, shingleSize);
            return jaccard >= howClose;
        }

        /// <summary>
        /// Calculate the Jaccard similarity between two strings.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <returns>The Jaccard similarity between the two input strings.</returns>
        public double Jaccard(string s1, string s2, int shingleSize)
        {
            var wordsInS1 = getWordsInSentence(s1);
            var wordsInS2 = getWordsInSentence(s2);

            // Easy solution if shinglesize > words in input
            if (wordsInS1.Count() < shingleSize || wordsInS2.Count() < shingleSize)
            {
                return double.NaN;
            }

            var shingles1 = GetShingles(wordsInS1, shingleSize);
            var shingles2 = GetShingles(wordsInS2, shingleSize);

            int cap = shingles1.Intersect(shingles2).Count();
            int cup = shingles1.Union(shingles2).Count();

            return (double)cap / cup;
        }

        /// <summary>
        /// Convert a string to an array of the words in the string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string[] getWordsInSentence(string s)
        {
            char[] sep = { ' ', ',', '.' };
            var words = s.Split(sep).Except(new string[] { "" }).Distinct().ToArray();
            return words;
        }

        /// <summary>
        /// Create a set of shingles from a set of words.
        /// </summary>
        /// <param name="words">The words to create the shingles from.</param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <returns></returns>
        private IEnumerable<int> GetShingles(string[] words, int shingleSize)
        {
            var shingles = new List<int>();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < words.Count() - shingleSize + 1; i++)
            {
                for (int j = 0; j < shingleSize; j++)
                {
                    builder.Append(words[j + i]);
                }

                shingles.Add(builder.ToString().GetHashCode());

                // We reuse the StringBuilder, so it must be cleared first.
                builder.Clear();
            }

            return shingles;
        }
        #endregion

        #region robots
        /// <summary>
        /// Parse robots.txt from a specific url.
        /// Assumes robots.txt is properly formatted.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="robots"></param>
        /// <returns>A regex to determine if a url is disallowed.</returns>
        public Regex CannotAccess(string url, IEnumerable<string> robots)
        {
            // No restrictions
            if (robots == null || robots.Count() == 0)
            {
                return new Regex(@".*");
            }

            // Find what is disallowed.
            var disallow = robots.SkipWhile(s => !Regex.IsMatch(s, @"User-Agent: \*", RegexOptions.IgnoreCase)) // Start from user agent *.
                .TakeWhile(s => !string.IsNullOrWhiteSpace(s)) // Read until blank line (where allow/disallow hopefully ends).
                .Skip(1) // Skip the user agent string.
                .Where(s => s.StartsWith("Disallow")) // We only need disallowed stuff.
                .Select(s => s.Split(':').Last().Trim()); // Select the disallowed stuff.

            if (disallow.Count() == 0)
            {
                return new Regex(@".*");
            }

            // Build the regex string
            StringBuilder regPattern = new StringBuilder(url + "(" + disallow.First());
            foreach (var s in disallow.Skip(1))
            {
                regPattern.Append('|');
                //regPattern.Append(url);
                regPattern.Append(s);
            }
            regPattern.Append(')');

            regPattern.Replace("*", ".*").Replace(".", "\\.");

            return new Regex(regPattern.ToString());
        }
        #endregion
    }
}
