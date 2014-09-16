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
        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            Crawler.Crawler craw = new Crawler.Crawler(10, TimeSpan.FromSeconds(60)) { TotalVisits = 1000 };
            var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("reddit.com"), new PrettyURL("politikken.dk") };
            craw.CrawlTheWeb(seed);

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
