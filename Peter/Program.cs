﻿using System;
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
        //static DBContextDataContext dbCon = new DBContextDataContext();
        static DB database = new DB();
        private static Queue<Tuple<PrettyURL, string, DateTime>> ToBeIndexedQueue = new Queue<Tuple<PrettyURL, string, DateTime>>();


        static void Main(string[] args)
        {
            int sitesToCrawl = 5;
            int numFrontQueues = 10;
            int numBackQueues = 3;
            TimeSpan timeBetweenHits = TimeSpan.FromSeconds(1);
            TimeSpan maxRobotAge = TimeSpan.FromMinutes(5);
            var seed = new PrettyURL[] { new PrettyURL("newz.dk"), new PrettyURL("aau.dk"), new PrettyURL("politikken.dk") };
            Crawler.Crawler crawler = new Crawler.Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed, ToBeIndexedQueue);

            System.Threading.Thread thread = new System.Threading.Thread(crawler.Crawl);
            thread.Start();

            while (ToBeIndexedQueue.Count < sitesToCrawl)
            {
                System.Threading.Thread.Sleep(100);
            }

            crawler.ContinueCrawling = false;

            Console.WriteLine("yo");

            Console.ReadLine();
        }

        //static void Main(string[] args)
        //{
        //    //database.clearPages();
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

        //        Crawler.Crawler crawler = new Crawler.Crawler(numFrontQueues, numBackQueues, timeBetweenHits, maxRobotAge, seed);
        //        var sites = crawler.CrawlTheWebAndAddToDB(sitesToCrawl);
        //        Console.WriteLine("Completed downloading");
        //    }

        //    Console.WriteLine("Creating index");
        //    var stopWords = new string[] { "og", "i", "jeg", "det", "at", "en", "den", "til", "er", "som", "på", "de", "med", "han", "af", "for", "ikke", "der", "var", "mig", "sig", "men", "et", "har", "om", "vi", "min", "havde", "ham", "hun", "nu", "over", "da", "fra", "du", "ud", "sin", "dem", "os", "op", "man", "hans", "hvor", "eller", "hvad", "skal", "selv", "her", "alle", "vil", "blev", "kunne", "ind", "når", "være", "dog", "noget", "ville", "jo", "deres", "efter", "ned", "skulle", "denne", "end", "dette", "mit", "også", "under", "have", "dig", "anden", "hende", "mine", "alt", "meget", "sit", "sine", "vor", "mod", "disse", "hvis", "din", "nogle", "hos", "blive", "mange", "ad", "bliver", "hendes", "været", "thi", "jer", "sådan" };
        //    var charsToRemove = new char[] { ',', '.', '?' };
        //    MainIndexer indexer = new MainIndexer(stopWords, charsToRemove);
        //    var index = indexer.CreateInverseIndex();
        //    Console.WriteLine("Indexing completed");

        //    Console.WriteLine(index.Select(t => t.Value.Count).Sum());

        //    Console.ReadKey();
        //}

    }
}
