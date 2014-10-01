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

        public tf_idf_searcher()
        {
            Database = new DB();
        }

        public double TF_IDF(string term, string prettyURL)
        {
            int tf1 = 1 + Database.TermFrequencyInDocument(term, prettyURL);
            double tf = Math.Log10(tf1);

            int N = Database.TotalSitesInDB();
            int df1 = Database.DocumentFrequency(term);
            double df2 = N / df1;
            double df = Math.Log10(df2);

            double result = tf * df;

            return result;
        }
    }
}
