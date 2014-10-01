using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Indexer;
using URLStuff;
using System.Diagnostics;
using PetersWeb;
using System.Collections.Concurrent;

namespace Crawler
{
    public class Crawler
    {
        private static object _siteCrawledLock = new object();
        private static object _continueCrawlLock = new object();
        private static int _SitesCrawledSoFar = 0;
        private static bool _ContinueCrawling = true;
        private static int _SitesToCrawl = 1000;

        public static int SitesCrawledSoFar
        {
            get
            {
                lock (_siteCrawledLock)
                {
                    return Crawler._SitesCrawledSoFar;
                }
            }
            set
            {
                lock (_siteCrawledLock)
                {
                    Crawler._SitesCrawledSoFar = value;
                }
            }
        }
        /// <summary>
        /// continue && sitescrawled < tocrawl
        /// </summary>
        public static bool ContinueCrawling
        {
            get
            {
                lock (_continueCrawlLock)
                {
                    return _ContinueCrawling && SitesCrawledSoFar < SitesToCrawl;
                }
            }
            set
            {
                lock (_continueCrawlLock)
                {
                    _ContinueCrawling = value;
                }
            }
        }
        public static int SitesToCrawl
        {
            get { return Crawler._SitesToCrawl; }
            set { Crawler._SitesToCrawl = value; }
        }

        private DB DataBase = new DB();
        private ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> CrawledQueue { get; set; }
        private RobotsStuff Robot_IAm { get; set; }
        private Mercator Mercator_IAm { get; set; }
        private System.Threading.CountdownEvent CTE { get; set; }

        public Crawler(int numFrontQueues, int numBackQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed, ConcurrentQueue<Tuple<PrettyURL, string, DateTime>> queue, System.Threading.CountdownEvent cte)
        {
            Robot_IAm = new RobotsStuff(maxAgeOfRobots);
            Mercator_IAm = new Mercator(numFrontQueues, numBackQueues, timebetweenVisits, seed);
            CrawledQueue = queue;
            CTE = cte;
        }

        public void Crawl()
        {
            while (ContinueCrawling)
            {
                var site = Mercator_IAm.GetURLToCrawl();
                var html = DownloadHTML(site);
                var toQueue = Tuple.Create(site, html, DateTime.Now);
                CrawledQueue.Enqueue(toQueue);
                SitesCrawledSoFar++;

                Debug.WriteLine("Crawler: downloaded " + site.GetPrettyURL);

                var links = ExtractLinksFromHTML(site, html);
                foreach (var link in links)
                {
                    Mercator_IAm.AddURLToFrontQueue(link);
                }
            }

            CTE.Signal();
        }

        public string DownloadHTML(PrettyURL url)
        {
            WebClient web = new WebClient();

            if (Robot_IAm.IsVisitAllowed(url))
            {
                try
                {
                    return web.DownloadString(url.GetPrettyURL);
                }
                catch (Exception ex)
                {
                    // tough luck
                    System.Diagnostics.Debug.WriteLine("Crawler: Error downloading " + url);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<PrettyURL> ExtractLinksFromHTML(PrettyURL url, string html)
        {
            if (html == null)
            {
                return Enumerable.Empty<PrettyURL>();
            }

            var hrefs = html.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Where(s => !s.StartsWith("feed") && !s.StartsWith("javascript"));

            var urls = new List<PrettyURL>();

            foreach (var href in hrefs)
            {
                var link = href.Split('\"').First();
                string fullPath = (link.StartsWith("/") ? url.GetPrettyURL : "") + link;
                if (PrettyURL.IsValidURL(fullPath))
                {
                    var pretty = new PrettyURL(fullPath);

                    if (pretty.GetDomain.EndsWith(".dk"))
                    {
                        urls.Add(new PrettyURL(fullPath));
                    }
                }
            }

            return urls;
        }

        //private Dictionary<string, string> DoTheCrawl_SaveToDB(int numberOfSitesToVisit, bool print)
        //{
        //    Stopwatch watch = new Stopwatch();
        //    Stopwatch totalTime = new Stopwatch();
        //    Stopwatch dupTime = new Stopwatch();

        //    Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>();
        //    List<PrettyURL> hasNearDuplicate = new List<PrettyURL>();

        //    Dictionary<string, string> result = new Dictionary<string, string>();
        //    int sitesVisited = 0;

        //    totalTime.Start();

        //    while (sitesVisited < numberOfSitesToVisit)
        //    {
        //        watch.Restart();

        //        var prettyURL = Mercator_IAm.GetURLToCrawl();

        //        // check with robot
        //        if (Robot_IAm.IsVisitAllowed(prettyURL))
        //        {
        //            var html = DownloadHTML(prettyURL);
        //            if (html == null)
        //            {
        //                continue;
        //            }

        //            // check for near dups
        //            dupTime.Restart();
        //            if (IsNearDuplicateOfAlreadyAddedSite(html))
        //            {
        //                hasNearDuplicate.Add(prettyURL);
        //                continue;
        //            }
        //            dupTime.Stop();

        //            DataBase.InsertNewDownloadedPage(prettyURL.GetPrettyURL, html);

        //            var links = ExtractLinksFromHTML(prettyURL, html);
        //            foreach (var link in links)
        //            {
        //                Mercator_IAm.AddURLToFrontQueue(link);
        //            }

        //            if (print)
        //            {
        //                Console.WriteLine(sitesVisited + "\t" + prettyURL);
        //            }
        //        }

        //        watch.Stop();
        //        times[prettyURL.GetDomain] = watch.Elapsed;
        //        Console.WriteLine(sitesVisited + "\t" + (int)watch.Elapsed.TotalMilliseconds + "\t" + (int)dupTime.Elapsed.TotalMilliseconds + "\t" + prettyURL);
        //        Console.Title = totalTime.Elapsed.ToString();
        //        sitesVisited++;
        //    }

        //    return result;
        //}

        //private bool IsNearDuplicateOfAlreadyAddedSite(string html)
        //{
        //    // smid html i db
        //    var input = GetHTMLContent(html);

        //    // smid shingle hashes i db?
        //    foreach (var site in DataBase.GetAllPages())
        //    {
        //        var compareWith = GetHTMLContent(site.html);

        //        if (IAMJACCARD.IsNearDuplicate(compareWith, input))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private string GetHTMLContent(string html)
        //{
        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(html);

        //    var a = doc.DocumentNode.SelectNodes("//p");
        //    if (a == null)
        //    {
        //        return "";
        //    }
        //    var b = a.Select(p => p.InnerText);


        //    var content = string.Join(" ", b);

        //    return content;
        //}


    }
}
