using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;
using URLStuff;
using PetersWeb;

namespace Peter
{
    class Program
    {
        static DBContextDataContext dbCon = new DBContextDataContext();
        static void Main(string[] args)
        {
                
            //Crawler.Crawler craw = new Crawler.Crawler(10);

            //string url = "httP://rDddit.com/r/stupid/%a0%05%7e/";

            //string res = craw.MakeURLPretty(url);

            //Console.WriteLine(res);
            Console.WriteLine("Started");
            test();
            

            //int numFrontQueues = 10;
            //int numBackQueues = 3;
            //TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
            //TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
            //var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("reddit.com"), new PrettyURL("politikken.dk") };
            //var stopWords = new string[] { };

            //Crawler.Crawler crawler = new Crawler.Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed, stopWords);
            //crawler.CrawlTheWeb();

            Console.WriteLine("Completed");
            Console.ReadKey();
        }

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
