using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLStuff;
using PetersWeb;

namespace Indexer
{
    public class MainIndexer
    {
        public MainIndexer(IEnumerable<string> stopWords, IEnumerable<char> charsToRemove)
        {
            StopWords = stopWords ?? Enumerable.Empty<string>();
            CharsToRemove = charsToRemove ?? Enumerable.Empty<char>();
        }

        public IEnumerable<string> StopWords { get; set; }
        public IEnumerable<char> CharsToRemove { get; set; }

        private DB DataBase = new DB();

        public Dictionary<string, List<int>> CreateInverseIndex()
        {
            // get from db
            var siteWithContent = DataBase.GetAllPages();

            // term -> sites with term
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            foreach (var site in siteWithContent)
            {
                doc.LoadHtml(site.html);
                var tokenized = Tokenizer(doc);
                var stopWordsRemoved = StopWordRemover(tokenized).ToArray();
                var caseFolded = CaseFolder(stopWordsRemoved).ToArray();
                var stemmed = Stemmer(caseFolded).ToArray();
                
                foreach (var word in stemmed)
                {
                    if (!result.ContainsKey(word))
                    {
                        result.Add(word, new List<int>());
                    }
                    result[word].Add(site.id);
                }
            }

            return result;
        }

        public IEnumerable<string> Tokenizer(HtmlAgilityPack.HtmlDocument doc)
        {
            var paragraphs = doc.DocumentNode.SelectNodes("//p");

            if (paragraphs == null)
            {
                return Enumerable.Empty<string>();
            }

            var innerTexts = paragraphs.Select(p => Danish.MakeDanish(p.InnerText));
            var longParagraph = string.Join(" ", innerTexts);
            var charRemoved = new string(longParagraph.Where(c => !CharsToRemove.Contains(c)).ToArray()); // must be a better way
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
