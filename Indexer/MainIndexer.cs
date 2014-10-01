using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLStuff;
using PetersWeb;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Indexer
{
    public class MainIndexer
    {
        public MainIndexer(IEnumerable<string> stopWords, IEnumerable<char> charsToRemove, ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> toIndex, System.Threading.CountdownEvent cte)
        {
            StopWords = stopWords ?? Enumerable.Empty<string>();
            CharsToRemove = charsToRemove ?? Enumerable.Empty<char>();
            ToBeIndexedQueue = toIndex;
            CTE = cte;
        }

        private System.Threading.CountdownEvent CTE { get; set; }

        private ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> ToBeIndexedQueue { get; set; }
        public IEnumerable<string> StopWords { get; set; }
        public IEnumerable<char> CharsToRemove { get; set; }
        private DB DataBase = new DB();

        public void CreateInverseIndexWriteToDB()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            while (true)
            {
                var site = TryToDequeueOtherwiseSignal();
                if (site == null)
                {
                    CTE.Signal();
                    return;
                }
                Debug.WriteLine("Indexer got " + site.Item1);

                // write to db
                Page page = DataBase.GetPageFromURL(site.Item1.GetPrettyURL);
                if (page != null)
                {
                    var existingShingles = DataBase.GetShinglesFromPrettyURL(site.Item1.GetPrettyURL);

                    // get next site. in here, check if update needed.
                    continue;
                }
                DataBase.InsertNewDownloadedPage(site.Item1.GetPrettyURL, site.Item2);

                doc.LoadHtml(site.Item2);
                var content = ReadContentFromHTMLDoc(doc);
                var tokenized = Tokenizer(content);
                var stopWordsRemoved = StopWordRemover(tokenized).ToArray();
                var caseFolded = CaseFolder(stopWordsRemoved).ToArray();
                var stemmed = Stemmer(caseFolded).ToArray();

                DataBase.InsertTokens(site.Item1.GetPrettyURL, stemmed);
            }
        }

        private bool IsSiteContentCloseToExistingSite()
        {
            throw new NotImplementedException();
        }

        private Tuple<PrettyURL, string, DateTime> TryToDequeueOtherwiseSignal()
        {
            var site = default(Tuple<PrettyURL, string, DateTime>);
            bool success = false;
            // try to dq five times, then fail?
            for (int i = 0; i < 50; i++)
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
