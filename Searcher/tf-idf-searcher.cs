using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetersWeb;

namespace Peter
{
    public class tf_idf_searcher
    {
        private DB Database { get; set; }
        private Jaccard Jaccard_IAm { get; set; }

        public tf_idf_searcher()
        {
            Database = new DB();
            Jaccard_IAm = new Jaccard(4, 0.9, null);
        }

        public IEnumerable<KeyValuePair<string, double>> SearchAndGetURLs(string query)
        {
            var answer = CosineScore(query);

            var sorted = answer
                .OrderByDescending(k => k.Value)
                .TakeWhile(s => s.Value > 0);

            return sorted;
        }

        private double IDF(string term)
        {
            int N = Database.TotalSitesInDB();
            int df1 = Database.DocumentFrequency(term);
            double df2 = N / df1;
            double df = Math.Log10(df2);
            return df;
        }

        private double TF(string term, string prettyURL)
        {
            int tf1 = 1 + Database.TermFrequencyInDocument(term, prettyURL);
            double tf = Math.Log10(tf1);
            return tf;
        }

        public double TF_IDF(string term, string prettyURL)
        {
            double tf = TF(term, prettyURL);
            double df = IDF(term);

            double result = tf * df;

            return result;
        }



        public Dictionary<string, double> CosineScore(string query)
        {
            Dictionary<string, double> Scores = new Dictionary<string, double>();
            Dictionary<string, double> Length = new Dictionary<string, double>();
            var wordsInQuery = Jaccard_IAm.getWordsInSentence(query);

            //var allTerms = Database.GetAllTerms();
            //foreach (var doc in Database.GetAllPages())
            //{
            //    int sum = 0;
            //    foreach (var term in allTerms)
            //    {
            //        int count = Database.TermFrequencyInDocument(term, doc.url);
            //        sum += count * count;
            //    }

            //    Length[doc.url] = Math.Sqrt(sum);
            //}

            foreach (var doc in Database.GetAllPages())
            {
                int sum = 0;
                foreach (var term in wordsInQuery)
                {
                    int val = Database.TermFrequencyInDocument(term, doc.url);
                    sum += val * val;
                }

                double sq = Math.Sqrt(sum);
                Length[doc.url] = 1 / sq;
            }


            foreach (var word in wordsInQuery)
            {
                var postingList = Database.GetInvertedIndexForSingleToken(word);

                foreach (var doc in postingList)
                {
                    if (!Scores.ContainsKey(doc.Page.url))
                    {
                        Scores[doc.Page.url] = 0;
                    }

                    Scores[doc.Page.url] += TF_IDF(word, doc.Page.url);
                }
            }

            foreach (var item in Length)
            {
                //Scores[item.Key] = item.Value / Length[item.Key];
            }

            return Scores;
        }
    }
}
