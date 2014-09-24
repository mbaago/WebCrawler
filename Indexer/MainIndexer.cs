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
        public MainIndexer(IEnumerable<string> stopWords)
        {
            StopWords = stopWords ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> StopWords { get; set; }
        private DB DataBase = new DB();

        public Dictionary<string, List<int>> CreateInverseIndex()
        {
            // get from db
            var siteWithContent = DataBase.GetAllPages();

            // term -> sites with term
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();

            foreach (var site in siteWithContent)
            {
                var a = Tokenizer(site.html);
                var b = StopWordRemover(a).ToArray();
                var c = CaseFolder(b).ToArray();
                var d = Stemmer(c).ToArray();

                foreach (var word in d)
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

        public IEnumerable<string> Tokenizer(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var paragraphs = doc.DocumentNode.SelectNodes("//p");

            if (paragraphs == null)
            {
                return Enumerable.Empty<string>();
            }

            var innerTexts = paragraphs.Select(p => Danish.MakeDanish(p.InnerText));

            var longParagraph = string.Join(" ", innerTexts);

            return longParagraph.Split(' ');
        }

        public IEnumerable<string> StopWordRemover(IEnumerable<string> input)
        {
            return input.Except(StopWords);
        }

        public IEnumerable<string> CaseFolder(IEnumerable<string> input)
        {
            return input.Select(s => s.ToLower());
        }

        public IEnumerable<string> Stemmer(IEnumerable<string> input)
        {
            var ret = input
                .Select(s => s.Replace("sses", "ss"))
                .Select(s => s.Replace("ies", "i"))
                .Select(s => s.Replace("ational", "ate"))
                .Select(s => s.Replace("tional", "tion"));

            return ret;
        }
    }
}
