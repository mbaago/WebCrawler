using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public class MainIndexer
    {
        public MainIndexer(IEnumerable<string> stopWords)
        {
            StopWords = stopWords ?? Enumerable.Empty<string>();
        }
        public IEnumerable<string> StopWords { get; set; }

        public IEnumerable<string> Tokenizer(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var paragraphs = doc.DocumentNode.SelectNodes("//p")
                .Select(p => Danish.MakeDanish(p.InnerText));

            var longParagraph = string.Join(" ", paragraphs);

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
