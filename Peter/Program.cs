using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;

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

            test();
            Console.WriteLine("Started");

            Crawler.Crawler craw = new Crawler.Crawler(10, TimeSpan.FromSeconds(60)) { TotalVisits = 1000 };
            var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("reddit.com"), new PrettyURL("politikken.dk") };
            craw.CrawlTheWeb(seed);

            Console.WriteLine("Completed");
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
