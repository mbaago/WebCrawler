using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peter;
using URLStuff;
using PetersWeb;
using System.Threading;
using System.Collections.Concurrent;

namespace Peter
{
    class Program
    {
        //static DBContextDataContext dbCon = new DBContextDataContext();
        static DB database = new DB();
        private static ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> ToBeIndexedQueue = new ConcurrentQueue<Tuple<PrettyURL, string, DateTime>>();
        private static tf_idf_searcher Searcher = new tf_idf_searcher();

        private static int numFrontQueues = 10;
        private static int numBackQueues = 3;
        private static TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
        private static TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
        private static string[] stopWords = new string[] { "og", "i", "jeg", "det", "at", "en", "den", "til", "er", "som", "på", "de", "med", "han", "af", "for", "ikke", "der", "var", "mig", "sig", "men", "et", "har", "om", "vi", "min", "havde", "ham", "hun", "nu", "over", "da", "fra", "du", "ud", "sin", "dem", "os", "op", "man", "hans", "hvor", "eller", "hvad", "skal", "selv", "her", "alle", "vil", "blev", "kunne", "ind", "når", "være", "dog", "noget", "ville", "jo", "deres", "efter", "ned", "skulle", "denne", "end", "dette", "mit", "også", "under", "have", "dig", "anden", "hende", "mine", "alt", "meget", "sit", "sine", "vor", "mod", "disse", "hvis", "din", "nogle", "hos", "blive", "mange", "ad", "bliver", "hendes", "været", "thi", "jer", "sådan" };
        private static char[] charsToRemove = new char[] { ',', '.', '?', ':', '\"' };

        static void Main(string[] args)
        {
            char s = default(char);
            bool cont = true;

            while (cont)
            {
                Console.WriteLine("Press s to search, c to crawl ~100 sites (broken after first time, w/e), anything else to exit");
                s = Console.ReadKey().KeyChar;

                switch (s)
                {
                    case 's':
                        DoSomeSearching();
                        break;
                    case 'c':
                        DoSomeCrawlingAndIndexing(100);
                        break;
                    default:
                        cont = false;
                        break;
                }
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        private static void IndexOnPagesInDB_IAMLAZY()
        {
            foreach (var page in database.GetAllPages())
            {
                ToBeIndexedQueue.Enqueue(new Tuple<PrettyURL, string, DateTime>(new PrettyURL(page.url), page.html, DateTime.Now));
            }

            CountdownEvent CTE = new CountdownEvent(1);
            var indexer = new MainIndexer(stopWords, charsToRemove, ToBeIndexedQueue, CTE);
            Thread indexerThread = new Thread(() => indexer.CreateInverseIndexWriteToDB(true));
            CTE.AddCount();
            indexerThread.Start();
            CTE.Signal();
            CTE.Wait();
        }

        private static void DoSomeSearching()
        {
            Console.WriteLine("\nSearch string:");
            string input = Console.ReadLine();

            var x = new MainIndexer(null, null, null, null).DoStuffOnInputString(input);
            input = string.Join(" ", x);

            var result = Searcher.SearchAndGetURLs(input).Take(10);

            if (result.Count() > 0)
            {
                foreach (var res in result)
                {
                    Console.WriteLine(res);
                }
            }
            else
            {
                Console.WriteLine("No results");
            }


        }

        private static void DoSomeCrawlingAndIndexing(int approxSites)
        {
            var dbPages = database.GetAllPages().
                Select(p => new PrettyURL(p.url));

            var seed = dbPages.Count() > 0 ?
                dbPages :
                new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("aau.dk"), new PrettyURL("politikken.dk") };

            Crawler.SitesToCrawl = approxSites;
            CountdownEvent CTE = new CountdownEvent(1);

            var crawler = new Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed, ToBeIndexedQueue, CTE);
            var indexer = new MainIndexer(stopWords, charsToRemove, ToBeIndexedQueue, CTE);

            Thread crawlerThread = new Thread(crawler.Crawl);
            CTE.AddCount();

            Thread indexerThread = new Thread(() => indexer.CreateInverseIndexWriteToDB(false));
            CTE.AddCount();

            crawlerThread.Start();
            indexerThread.Start();

            CTE.Signal();
            CTE.Wait();
        }




        //static void Main(string[] args)
        //{
        //    //database.DeleteAllPages();
        //    //return;

        //    if (database.GetAllPages().Count() == 0)
        //    {

        //        Console.WriteLine("Started");
        //        int sitesToCrawl = 25;
        //        int numFrontQueues = 10;
        //        int numBackQueues = 3;
        //        TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
        //        TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
        //        var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("aau.dk"), new PrettyURL("politikken.dk") };

        //        Indexer.Indexer crawler = new Indexer.Indexer(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed);
        //        var sites = crawler.CrawlTheWebAndAddToDB(sitesToCrawl);
        //        Console.WriteLine("Completed downloading");
        //    }

        //    Console.WriteLine("Creating index");
        //    var stopWords = new string[] { "og", "i", "jeg", "det", "at", "en", "den", "til", "er", "som", "på", "de", "med", "han", "af", "for", "ikke", "der", "var", "mig", "sig", "men", "et", "har", "om", "vi", "min", "havde", "ham", "hun", "nu", "over", "da", "fra", "du", "ud", "sin", "dem", "os", "op", "man", "hans", "hvor", "eller", "hvad", "skal", "selv", "her", "alle", "vil", "blev", "kunne", "ind", "når", "være", "dog", "noget", "ville", "jo", "deres", "efter", "ned", "skulle", "denne", "end", "dette", "mit", "også", "under", "have", "dig", "anden", "hende", "mine", "alt", "meget", "sit", "sine", "vor", "mod", "disse", "hvis", "din", "nogle", "hos", "blive", "mange", "ad", "bliver", "hendes", "været", "thi", "jer", "sådan" };
        //    var charsToRemove = new char[] { ',', '.', '?' };
        //    MainIndexer indexer = new MainIndexer(stopWords, charsToRemove);
        //    var index = indexer.CreateInverseIndexWriteToDB();
        //    Console.WriteLine("Indexing completed");

        //    Console.WriteLine(index.Select(t => t.Value.Count).Sum());

        //    Console.ReadKey();
        //}

    }
}
