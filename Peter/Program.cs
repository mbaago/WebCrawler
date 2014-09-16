using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            Crawler.Crawler craw = new Crawler.Crawler(10) { TotalVisits = 1000 };

            craw.CrawlTheWeb(new string[] { "newz.dk", "reddit.com" });

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
