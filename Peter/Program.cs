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
            Crawler.Crawler craw = new Crawler.Crawler(10);

            string url = "httP://rDddit.com/r/stupid/%a0%05%7e/";

            string res = craw.MakeURLPretty(url);

            Console.WriteLine(res);

            Console.ReadKey();
        }
    }
}
