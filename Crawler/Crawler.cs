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
        public Crawler(int numFrontQueues)
        {
            //MaxNumberOfBackQueues = 3;
            //FrontQueues = new Queue<string>[numFrontQueues];
            //for (int i = 0; i < FrontQueues.Length; i++)
            //{
            //    FrontQueues[i] = new Queue<string>();
            //}
            //BackQueues = new Dictionary<string, Queue<string>>();
            //DomainsVisited = new Dictionary<string, DateTime>();

            //VisitedURLS = new List<string>();

            //RobotStuff = new RobotsStuff(TimeSpan.FromSeconds(60));
        }

        public int TotalVisits { get; set; }
        private RobotsStuff RobotStuff { get; set; }
        private Mercator Queuer { get; set; }

        public void CrawlTheWeb(IEnumerable<string> seed)
        {
            //foreach (var item in seed)
            //{
            //    AddURLToQueue(item);
            //}

            //foreach (var url in seed)
            //{

            //    foreach (var link in ExtractLinksFromHTML(url))
            //    {
            //        AddURLToQueue(link);
            //    }
            //}
        }

        //private int MaxNumberOfBackQueues { get; set; }
        //private Queue<string>[] FrontQueues { get; set; }
        //private Dictionary<string, Queue<string>> BackQueues { get; set; }
        //private Dictionary<string, DateTime> DomainsVisited { get; set; }
        //private List<string> VisitedURLS { get; set; }

        //public void AddURLToQueue(string url)
        //{
        //    url = URLStuff.MakeURLPretty(url);

        //    if (!VisitedURLS.Contains(url))
        //    {
        //        int q = new Random().Next(0, FrontQueues.Length);
        //        FrontQueues[q].Enqueue(url);
        //    }
        //}

        //public string BackQueueSelector()
        //{
        //    // første gang, alle tomme
        //    while (BackQueues.Count < MaxNumberOfBackQueues)
        //    {
        //        string url = FrontQueueSelector();
        //        string dom = URLStuff.ExtractDomain(url);
        //        if (BackQueues.Keys.Contains(dom))
        //        {
        //            BackQueues[dom].Enqueue(url);
        //        }
        //        else
        //        {
        //            BackQueues.Add(dom, new Queue<string>());
        //            BackQueues[dom].Enqueue(url);
        //        }
        //    }

        //    // Simulate the heap
        //    var orderedTimes = DomainsVisited
        //        .Where(d => BackQueues.Keys.Contains(d.Key))
        //        .OrderByDescending(t => t.Value);

        //    var oldest = orderedTimes.First();

        //    while (DateTime.Now - oldest.Value < TimeSpan.FromSeconds(1))
        //    {
        //        System.Threading.Thread.Sleep(10);
        //    }

        //    var returnURL = BackQueues[oldest.Key].Dequeue();

        //    while (BackQueues[oldest.Key].Count == 0)
        //    {
        //        var item = FrontQueueSelector();
        //        var dom = URLStuff.ExtractDomain(item);

        //        if (BackQueues.ContainsKey(dom))
        //        {
        //            BackQueues[dom].Enqueue(item);
        //        }
        //        else
        //        {
        //            BackQueues[dom] = new Queue<string>();
        //            BackQueues[dom].Enqueue(item);
        //        }
        //    }

        //    return returnURL;
        //}

        //private string FrontQueueSelector()
        //{
        //    int rand = new Random().Next(0, FrontQueues.Length);

        //    for (int i = 0; i < FrontQueues.Length; i++)
        //    {
        //        if (FrontQueues[rand].Count > 0)
        //        {
        //            return FrontQueues[rand].Dequeue();
        //        }

        //        rand = rand++ % FrontQueues.Length;
        //    }

        //    throw new Exception("FrontQueues were empty");
        //}

        //private void VisitDomain(string url)
        //{
        //    string dom = URLStuff.ExtractDomain(url);
        //    if (DomainsVisited.ContainsKey(dom))
        //    {
        //        DomainsVisited[dom] = DateTime.Now;
        //    }
        //    else
        //    {
        //        DomainsVisited.Add(dom, DateTime.Now);
        //    }
        //}

        //public string DownloadHTML(string url)
        //{
        //    url = URLStuff.MakeURLPretty(url);
        //    WebClient web = new WebClient();
        //    string dom = URLStuff.ExtractDomain(url);

        //    RobotStuff.DownloadRobotsIfTooOld(url);

        //    if (RobotStuff.IsVisitAllowed(url))
        //    {
        //        VisitDomain(url);
        //        VisitedURLS.Add(url);
        //        return web.DownloadString(url);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public IEnumerable<string> ExtractLinksFromHTML(string url)
        //{
        //    var html = DownloadHTML(url);

        //    if (html == null)
        //    {
        //        return Enumerable.Empty<string>();
        //    }

        //    var hrefs = html.Split(new string[] { "<a href=\"" }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

        //    List<string> urls = new List<string>();

        //    foreach (var href in hrefs)
        //    {
        //        var link = href.Split('\"').First();
        //        string fullPath = (link.StartsWith("/") ? url : "") + link;
        //        if (URLStuff.IsValidURL(fullPath))
        //        {
        //            string ur = URLStuff.MakeURLPretty(fullPath);

        //            urls.Add(ur);
        //        }
        //    }

        //    return urls;
        //}
    }
}
