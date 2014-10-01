using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLStuff;
using PetersWeb;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Peter
{
    public class MainIndexer
    {
        public MainIndexer(IEnumerable<string> stopWords, IEnumerable<char> charsToRemove, ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> toIndex, System.Threading.CountdownEvent cte)
        {
            StopWords = stopWords ?? Enumerable.Empty<string>();
            CharsToRemove = charsToRemove ?? Enumerable.Empty<char>();
            ToBeIndexedQueue = toIndex;
            CTE = cte;
            Jaccard_IAm = new Jaccard(4, 0.9, CharsToRemove.ToArray());
        }

        private System.Threading.CountdownEvent CTE { get; set; }

        private ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> ToBeIndexedQueue { get; set; }
        public IEnumerable<string> StopWords { get; set; }
        public IEnumerable<char> CharsToRemove { get; set; }
        private Jaccard Jaccard_IAm { get; set; }
        private DB DataBase = new DB();

        public void CreateInverseIndexWriteToDB(bool fromDB)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            Stopwatch watch = new Stopwatch();
            StringBuilder sb = new StringBuilder();
            int pages = 0;

            while (true)
            {
                sb.Clear();
                sb.Append("page: " + pages);
                var site = TryToDequeueOtherwiseSignal();
                if (site == null)
                {
                    CTE.Signal();
                    return;
                }

                if (site.Item2 == null)
                {
                    continue;
                }

                //Debug.WriteLine("Indexer got " + site.Item1);

                // we skip if site is already in db. maybe check for age instead?
                Page page = DataBase.GetPageFromURL(site.Item1.GetPrettyURL);
                if (page != null && !fromDB)
                {
                    continue;
                }

                watch.Restart();
                doc.LoadHtml(site.Item2);
                var content = ReadContentFromHTMLDoc(doc);
                var tokenized = Tokenizer(content);
                var stopWordsRemoved = StopWordRemover(tokenized).ToArray();
                var caseFolded = CaseFolder(stopWordsRemoved).ToArray();
                var stemmed = Stemmer(caseFolded).ToArray();

                sb.Append(", Stem: " + (int)watch.Elapsed.TotalMilliseconds);


                watch.Restart();
                var shinglesOnDownladedSite = Jaccard_IAm.GetShinglesHashes(stemmed.ToArray());
                // only added different site
                // maybe add but with html = null instead?
                if (IsSiteContentCloseToExistingSite(shinglesOnDownladedSite))
                {
                    continue;
                }
                sb.Append(", cmp: " + (int)watch.Elapsed.TotalMilliseconds);

                watch.Restart();
                if (!fromDB)
                {
                    DataBase.InsertNewDownloadedPage(site.Item1.GetPrettyURL, site.Item2);
                }
                //DataBase.InsertTokens(site.Item1.GetPrettyURL, stemmed);

                DataBase.ManualTokenInserter(site.Item1.GetPrettyURL, stemmed);
                DataBase.ManualTokenInserter(site.Item1.GetPrettyURL, stemmed);
                DataBase.InsertShingles(site.Item1.GetPrettyURL, shinglesOnDownladedSite);
                sb.Append(", insert: " + (int)watch.Elapsed.TotalMilliseconds);
                sb.Append(", " + site.Item1);

                Debug.WriteLine(sb.ToString());
                pages++;
            }
        }

        private bool IsSiteContentCloseToExistingSite(IEnumerable<int> newShingles)
        {
            foreach (var shingles in DataBase.GetAllShingleSets())
            {
                if (Jaccard_IAm.IsNearDuplicate(newShingles, shingles))
                {
                    return true;
                }
            }
            return false;
        }

        private Tuple<PrettyURL, string, DateTime> TryToDequeueOtherwiseSignal()
        {
            var site = default(Tuple<PrettyURL, string, DateTime>);
            bool success = false;
            // try to dq some times, then fail?
            for (int i = 0; i < 100; i++)
            {
                success = ToBeIndexedQueue.TryDequeue(out site);
                if (success)
                {
                    return site;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            return null;
        }

        private string ReadContentFromHTMLDoc(HtmlAgilityPack.HtmlDocument doc)
        {
            var paragraphs = doc.DocumentNode.SelectNodes("//p");

            if (paragraphs == null)
            {
                return "";
            }

            var innerTexts = paragraphs.Select(p => Danish.MakeDanish(p.InnerText));
            var longParagraph = string.Join(" ", innerTexts);

            return longParagraph;
        }

        public IEnumerable<string> Tokenizer(string content)
        {
            var charRemoved = new string(content.Where(c => !CharsToRemove.Contains(c)).ToArray()); // must be a better way
            var tokenized = charRemoved.Split(' ');

            return tokenized;
        }

        public IEnumerable<string> StopWordRemover(IEnumerable<string> input)
        {
            var stopWordRemoved = input.Except(StopWords);
            return stopWordRemoved;
        }

        public IEnumerable<string> CaseFolder(IEnumerable<string> input)
        {
            var caseFolded = input.Select(s => s.ToLower());
            return caseFolded;
        }

        public IEnumerable<string> Stemmer(IEnumerable<string> input)
        {
            //var ret = input
            //    .Select(s => s.Replace("sses", "ss"))
            //    .Select(s => s.Replace("ies", "i"))
            //    .Select(s => s.Replace("ational", "ate"))
            //    .Select(s => s.Replace("tional", "tion"));

            var ret = input;

            return ret;
        }
    }
}
