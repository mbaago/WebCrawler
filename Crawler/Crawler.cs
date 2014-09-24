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
        public Crawler(int numFrontQueues, int numBackQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed)
        {
            IAMAROBOTHANDLER = new RobotsStuff(maxAgeOfRobots);
            IAMTHEMERCATOR = new Mercator(numFrontQueues, numBackQueues, timebetweenVisits, seed);
        }

        private RobotsStuff IAMAROBOTHANDLER { get; set; }
        private Mercator IAMTHEMERCATOR { get; set; }
        private MainIndexer IAMTHEINDEXER { get; set; }

        private DB DataBase = new DB();

        public Dictionary<string, string> CrawlTheWeb(int getSites)
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            Console.WriteLine("Downloading");
            return DoTheCrawl_SaveToDB(getSites, false);

            //watch.Stop();
        }

        private Dictionary<string, string> DoTheCrawl_SaveToDB(int numberOfSitesToVisit, bool print)
        {
            Stopwatch watch = new Stopwatch();
            TimeSpan elapsed = new TimeSpan();
            Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>();

            Dictionary<string, string> result = new Dictionary<string, string>();
            int sitesVisited = 0;

            while (sitesVisited < numberOfSitesToVisit)
            {
                watch.Restart();

                var url = IAMTHEMERCATOR.GetURLToCrawl();

                // check with robot
                if (IAMAROBOTHANDLER.IsVisitAllowed(url))
                {
                    if (print)
                    {
                        Console.WriteLine(sitesVisited + "\t" + url);
                    }

                    var html = DownloadHTML(url);
                    if (html == null)
                    {
                        continue;
                    }

                    //DataBase.insertNew(url.GetPrettyURL, html);
                    result[url.GetPrettyURL] = html;

                    var links = ExtractLinksFromHTML(url, html);
                    foreach (var link in links)
                    {
                        IAMTHEMERCATOR.AddURLToFrontQueue(link);
                    }
                }

                watch.Stop();
                times[url.GetDomain] = watch.Elapsed;
                elapsed += watch.Elapsed;
                Console.WriteLine(sitesVisited + "\t" + (int)watch.Elapsed.TotalMilliseconds + "\t" + url);
                Console.Title = elapsed.ToString();
                sitesVisited++;
            }

            return result;
        }


        public string DownloadHTML(PrettyURL url)
        {
            WebClient web = new WebClient();

            if (IAMAROBOTHANDLER.IsVisitAllowed(url))
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
                    urls.Add(new PrettyURL(fullPath));
                }
            }

            return urls;
        }
    }
}
