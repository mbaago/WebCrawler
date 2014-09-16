using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peter
{
    class Program
    {
        static DataClasses1DataContext db = new DataClasses1DataContext();
        static void Main(string[] args)
        {
                
            //Crawler.Crawler craw = new Crawler.Crawler(10);

            //string url = "httP://rDddit.com/r/stupid/%a0%05%7e/";

            //string res = craw.MakeURLPretty(url);

            //Console.WriteLine(res);

            test();
            Console.WriteLine("Started");

            Crawler.Crawler craw = new Crawler.Crawler(10) { TotalVisits = 1000 };

            craw.CrawlTheWeb(new string[] { "newz.dk", "reddit.com" });

            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        static void test()
        {
            Page[] pagesA = { new Page { url = "www", html = "www" }, new Page { url = "www.reddit.com", html = "www" } };
            Page page = new Page { url = "www", html = "www" };
            //Page page2 = new Page { url = "www.reddit.com", html = "www" };
            //db.Pages.InsertAllOnSubmit(pagesA);
            db.Pages.InsertOnSubmit(page);

            db.SubmitChanges();

            IEnumerable<Page> pages = (from p in db.Pages
                                       select p);

            foreach (Page p in pages)
            {
                Console.WriteLine(p.Id + " " + p.url);
            }
            
        }
    }
}
