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
            Console.WriteLine("Started");

            int sitesToCrawl = 100;
            int numFrontQueues = 10;
            int numBackQueues = 3;
            TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
            TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
            var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("reddit.com"), new PrettyURL("politikken.dk") };

            Crawler.Crawler crawler = new Crawler.Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed);
            var sites = crawler.CrawlTheWeb(sitesToCrawl);
            Console.WriteLine("Completed downloading");

            Console.WriteLine("Inserting into db");

            Console.WriteLine("db insertion completed");

            Console.WriteLine("Creating index");
            var stopWords = new string[] { };
            MainIndexer indexer = new MainIndexer(stopWords);
            indexer.CreateInverseIndex();
            Console.WriteLine("Indexing completed");

            Console.ReadKey();
        }

        static void test()
        {
            DB db = new DB();

            db.insertNew("www.reddit.com", "<p>bla bla bla</p>");

            DBContextDataContext dbCon = new DBContextDataContext();

            Page page = db.getPageOnUrl("www.reddit.com");

            Console.WriteLine(page.id + " " + page.url + " " + page.html);

            //var result = from u in dbCon.Urls
            //             select u;


            //foreach (Url u in result)
            //{
            //    Console.WriteLine(u.id + " " + u.url1);
            //}

        }
    }
}
