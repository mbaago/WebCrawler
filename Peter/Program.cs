using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;
using URLStuff;
using PetersWeb;
using Indexer;

namespace Peter
{
    class Program
    {
        static DBContextDataContext dbCon = new DBContextDataContext();
        static void Main(string[] args)
        {
            //database.clearPages();
            //return;

            if (database.GetAllPages().Count() == 0)
            {

                Console.WriteLine("Started");
                int sitesToCrawl = 100;
                int numFrontQueues = 10;
                int numBackQueues = 3;
                TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
                TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
                var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("aau.dk"), new PrettyURL("politikken.dk") };

                Crawler.Crawler crawler = new Crawler.Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed);
                var sites = crawler.CrawlTheWeb(sitesToCrawl);
                Console.WriteLine("Completed downloading");

                Console.WriteLine("Inserting into db");
                foreach (var site in sites)
                {
                    database.insertNew(site.Key, site.Value);
                }
                Console.WriteLine("db insertion completed");
            }

            Console.WriteLine("Creating index");
            var stopWords = new string[] { "og", "i", "jeg", "det", "at", "en", "den", "til", "er", "som", "på", "de", "med", "han", "af", "for", "ikke", "der", "var", "mig", "sig", "men", "et", "har", "om", "vi", "min", "havde", "ham", "hun", "nu", "over", "da", "fra", "du", "ud", "sin", "dem", "os", "op", "man", "hans", "hvor", "eller", "hvad", "skal", "selv", "her", "alle", "vil", "blev", "kunne", "ind", "når", "være", "dog", "noget", "ville", "jo", "deres", "efter", "ned", "skulle", "denne", "end", "dette", "mit", "også", "under", "have", "dig", "anden", "hende", "mine", "alt", "meget", "sit", "sine", "vor", "mod", "disse", "hvis", "din", "nogle", "hos", "blive", "mange", "ad", "bliver", "hendes", "været", "thi", "jer", "sådan" };
            var charsToRemove = new char[] { ',', '.', '?' };
            MainIndexer indexer = new MainIndexer(stopWords, charsToRemove);
            var index = indexer.CreateInverseIndex();
            Console.WriteLine("Indexing completed");

            Console.WriteLine(index.Select(t => t.Value.Count).Sum());

            Console.ReadKey();
        }

        static DB database = new DB();

        static void test()
        {
            DB db = new DB();

            db.clearPages();
            db.insertNew("www.reddit.com", "<p>bla bla bla</p>");


            Page page = db.getPageOnUrl("www.reddit.com");

            Console.WriteLine(page.id + " " + page.url + " " + page.html);
        }
    }
}
