using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace Crawler
{
    public class Crawler
    {
        public Crawler(int numFrontQueues, TimeSpan timebetweenVisits, TimeSpan maxAgeOfRobots, IEnumerable<PrettyURL> seed)
        {
            TotalVisits = 1000;
            IAMAROBOTHANDLER = new RobotsStuff(maxAgeOfRobots);
            IAMTHEMERCATOR = new Mercator(numFrontQueues, 3, timebetweenVisits, seed);
        }

        public int TotalVisits { get; set; }
        private RobotsStuff IAMAROBOTHANDLER { get; set; }
        private Mercator IAMTHEMERCATOR { get; set; }

        public void CrawlTheWeb()
        {
            var visits = new List<PrettyURL>();

            for (int i = 0; i < 100; i++)
            {
                var url = IAMTHEMERCATOR.GetURLToCrawl();
                visits.Add(url);

                // check with robot
                if (IAMAROBOTHANDLER.IsVisitAllowed(url))
                {
                    var links = ExtractLinksFromHTML(url);

                    foreach (var link in links)
                    {
                        IAMTHEMERCATOR.AddURLToFrontQueue(link);
                    }
                }
            }
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

        public IEnumerable<PrettyURL> ExtractLinksFromHTML(PrettyURL url)
        {
            var html = DownloadHTML(url);

            if (html == null)
            {
                return Enumerable.Empty<PrettyURL>();
            }

            var hrefs = html.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

            var urls = new List<PrettyURL>();

            foreach (var href in hrefs)
            {
                var link = href.Split('\"').First();
                string fullPath = (link.StartsWith("/") ? url.GetPrettyURL : "") + link;
                if (URLStuff.IsValidURL(fullPath))
                {
                    urls.Add(new PrettyURL(fullPath));
                }
            }

            return urls;
        }
    }
}
