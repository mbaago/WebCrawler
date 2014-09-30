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

namespace Crawler
{
    public class Crawler
    {
        public Crawler(int numFrontQueues, int numBackQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed, Queue<Tuple<PrettyURL, string, DateTime>> queue)
        {
            Robot_IAm = new RobotsStuff(maxAgeOfRobots);
            Mercator_IAm = new Mercator(numFrontQueues, numBackQueues, timebetweenVisits, seed);
            CrawledQueue = queue;

            ContinueCrawling = true;
        }

        public void Crawl()
        {
            while (ContinueCrawling)
            {
                var site = Mercator_IAm.GetURLToCrawl();
                var html = DownloadHTML(site);
                var toQueue = Tuple.Create(site, html, DateTime.Now);
                CrawledQueue.Enqueue(toQueue);

                Debug.WriteLine("Crawler: downloaded " + site.GetPrettyURL);

                var links = ExtractLinksFromHTML(site, html);
                foreach (var link in links)
                {
                    Mercator_IAm.AddURLToFrontQueue(link);
                }
            }
        }

        private Queue<Tuple<PrettyURL, string, DateTime>> CrawledQueue { get; set; }

        private RobotsStuff Robot_IAm { get; set; }
        private Mercator Mercator_IAm { get; set; }
        //private MainIndexer IAMTHEINDEXER { get; set; }
        //private Jaccard IAMJACCARD = new Jaccard(4, 0.9);
        private DB DataBase = new DB();
        public bool ContinueCrawling { get; set; }

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

        //        var url = Mercator_IAm.GetURLToCrawl();

        //        // check with robot
        //        if (Robot_IAm.IsVisitAllowed(url))
        //        {
        //            var html = DownloadHTML(url);
        //            if (html == null)
        //            {
        //                continue;
        //            }

        //            // check for near dups
        //            dupTime.Restart();
        //            if (IsNearDuplicateOfAlreadyAddedSite(html))
        //            {
        //                hasNearDuplicate.Add(url);
        //                continue;
        //            }
        //            dupTime.Stop();

        //            DataBase.InsertNewDownloadedPage(url.GetPrettyURL, html);

        //            var links = ExtractLinksFromHTML(url, html);
        //            foreach (var link in links)
        //            {
        //                Mercator_IAm.AddURLToFrontQueue(link);
        //            }

        //            if (print)
        //            {
        //                Console.WriteLine(sitesVisited + "\t" + url);
        //            }
        //        }

        //        watch.Stop();
        //        times[url.GetDomain] = watch.Elapsed;
        //        Console.WriteLine(sitesVisited + "\t" + (int)watch.Elapsed.TotalMilliseconds + "\t" + (int)dupTime.Elapsed.TotalMilliseconds + "\t" + url);
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
