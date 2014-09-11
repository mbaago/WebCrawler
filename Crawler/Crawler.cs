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

            VisitedURLS = new List<string>();

            RobotStuff = new RobotsStuff(TimeSpan.FromSeconds(60));
        }

        public int TotalVisits { get; set; }
        private RobotsStuff RobotStuff { get; set; }

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
        private List<string> VisitedURLS { get; set; }

        public void AddURLToQueue(string url)
        {
            url = URLStuff.MakeURLPretty(url);

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
                string dom = URLStuff.ExtractDomain(url);
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
                var dom = URLStuff.ExtractDomain(item);

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
            string dom = URLStuff.ExtractDomain(url);
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
            url = URLStuff.MakeURLPretty(url);
            WebClient web = new WebClient();
            string dom = URLStuff.ExtractDomain(url);

            RobotStuff.DownloadRobotsIfTooOld(url);

            if (RobotStuff.IsVisitAllowed(url))
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
                if (URLStuff.IsValidURL(fullPath))
                {
                    string ur = URLStuff.MakeURLPretty(fullPath);

                    urls.Add(ur);
                }
            }

            return urls;
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
    }
}
