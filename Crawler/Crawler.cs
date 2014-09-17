using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Indexer;

namespace Crawler
{
    public class Crawler
    {
        public Crawler(int numFrontQueues, int numBackQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed, IEnumerable<string> stopWords)
        {
            TotalVisits = 1000;
            IAMAROBOTHANDLER = new RobotsStuff(maxAgeOfRobots);
            IAMTHEMERCATOR = new Mercator(numFrontQueues, numBackQueues, timebetweenVisits, seed);
            IAMTHEINDEXER = new MainIndexer(stopWords);
        }

        public int TotalVisits { get; set; }
        private RobotsStuff IAMAROBOTHANDLER { get; set; }
        private Mercator IAMTHEMERCATOR { get; set; }
        private MainIndexer IAMTHEINDEXER { get; set; }


        public void CrawlTheWeb()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var siteContents = DoTheCrawl_GetSitesContents(1);

            watch.Stop();
            Console.WriteLine(watch.Elapsed);


            watch.Restart();

            var a = siteContents.First();
            var b = IAMTHEINDEXER.Tokenizer(a.Value);
            var c = IAMTHEINDEXER.StopWordRemover(b);
            var d = IAMTHEINDEXER.CaseFolder(c);
            var e = IAMTHEINDEXER.Stemmer(d);

            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }

        private Dictionary<string, string> DoTheCrawl_GetSitesContents(int numberOfSitesToVisit)
        {
            Dictionary<string, string> SitesContents = new Dictionary<string, string>();
            for (int i = 0; i < numberOfSitesToVisit; i++)
            {
                var url = IAMTHEMERCATOR.GetURLToCrawl();

                // check with robot
                if (IAMAROBOTHANDLER.IsVisitAllowed(url))
                {
                    Console.WriteLine(i + "\t" + url);

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
