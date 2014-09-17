using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Indexer;
using URLStuff;

namespace Crawler
{
    public class Crawler
    {
        public Crawler(int numFrontQueues, int numBackQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed, IEnumerable<string> stopWords)
        {
            IAMAROBOTHANDLER = new RobotsStuff(maxAgeOfRobots);
            IAMTHEMERCATOR = new Mercator(numFrontQueues, numBackQueues, timebetweenVisits, seed);
            IAMTHEINDEXER = new MainIndexer(stopWords);
        }

        private RobotsStuff IAMAROBOTHANDLER { get; set; }
        private Mercator IAMTHEMERCATOR { get; set; }
        private MainIndexer IAMTHEINDEXER { get; set; }


        public void CrawlTheWeb()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Console.WriteLine("Downloading");
            var siteContents = DoTheCrawl_GetSitesContents(10, false);

            watch.Stop();
            //Console.WriteLine(watch.Elapsed);


            watch.Restart();

            var index = IAMTHEINDEXER.CreateInverseIndex(siteContents);

            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }

        private Dictionary<string, string> DoTheCrawl_GetSitesContents(int numberOfSitesToVisit, bool print)
        {
            Dictionary<string, string> SitesContents = new Dictionary<string, string>();
            for (int i = 0; i < numberOfSitesToVisit; i++)
            {
                var url = IAMTHEMERCATOR.GetURLToCrawl();

                // check with robot
                if (IAMAROBOTHANDLER.IsVisitAllowed(url))
                {
                    if (print)
                    {
                        Console.WriteLine(i + "\t" + url);
                    }

                    var html = DownloadHTML(url);
                    if (html == null)
                    {
                        continue;
                    }

                    var links = ExtractLinksFromHTML(url, html);

                    SitesContents[url.GetPrettyURL] = html;

                    foreach (var link in links)
                    {
                        IAMTHEMERCATOR.AddURLToFrontQueue(link);
                    }
                }
            }

            return SitesContents;
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
            var hrefs = html.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

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
