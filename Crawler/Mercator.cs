using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Mercator
    {
        public Mercator(int frontQueues, int backQueues, TimeSpan timeBetweenVisits)
        {
            FrontQueues = new List<Queue<PrettyURL>>(frontQueues);
            for (int i = 0; i < frontQueues; i++)
            {
                FrontQueues.Add(new Queue<PrettyURL>());
            }

            MaxNumberOfBackQueues = backQueues;
            BackQueues = new Dictionary<string, Queue<PrettyURL>>();
            BackQueueHeapSimulator = new Dictionary<string, DateTime>();

            TimeBetweenVisits = timeBetweenVisits;

            AllURLS = new List<PrettyURL>();
        }

        public TimeSpan TimeBetweenVisits { get; set; }

        private int MaxNumberOfBackQueues { get; set; }

        private List<Queue<PrettyURL>> FrontQueues { get; set; }
        private Dictionary<string, Queue<PrettyURL>> BackQueues { get; set; }
        private Dictionary<string, DateTime> BackQueueHeapSimulator { get; set; }

        /// <summary>
        /// All URLS, in fq, bq or visited.
        /// </summary>
        private List<PrettyURL> AllURLS { get; set; }

        public bool AddURLToFrontQueue(PrettyURL url)
        {
            // tjek om allerede besøgt
            foreach (var u in AllURLS)
            {
                if (u.GetPrettyURL == url.GetPrettyURL)
                {
                    return false;
                }
            }

            int rand = new Random().Next(0, FrontQueues.Count);
            FrontQueues[rand].Enqueue(url);
            AllURLS.Add(url);

            return true;
        }

        private PrettyURL FrontQueueSelector()
        {
            int rand = new Random().Next(0, FrontQueues.Count);

            for (int i = 0; i < FrontQueues.Count; i++)
            {
                if (FrontQueues[rand].Count > 0)
                {
                    return FrontQueues[rand].Dequeue();
                }

                rand = ++rand % FrontQueues.Count;
            }

            throw new Exception("FrontQueues were empty");
        }

        private void BackQueueRouter()
        {
            while (BackQueues.Count < MaxNumberOfBackQueues)
            {
                var url = FrontQueueSelector();

                if (BackQueues.Keys.Contains(url.GetDomain))
                {
                    // We already have a backqueue for this domain.
                    BackQueues[url.GetDomain].Enqueue(url);
                }
                else
                {
                    // Create a new backqueue for this domain.
                    BackQueues[url.GetDomain] = new Queue<PrettyURL>();
                    BackQueues[url.GetDomain].Enqueue(url);

                    BackQueueHeapSimulator[url.GetDomain] = DateTime.MinValue;
                }
            }
        }

        public PrettyURL GetURLToCrawl()
        {

            BackQueueRouter();

            string domain;
            // Have visited nothing.
            if (BackQueueHeapSimulator.Count > 0)
            {
                var oldestDomain = BackQueueHeapSimulator.OrderBy(h => h.Value).First();
                domain = oldestDomain.Key;

                // Wait until old enough.
                while (DateTime.Now - oldestDomain.Value < TimeBetweenVisits)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            var a = BackQueues.Values.OrderBy(q => q.Count);
            var b = a.First();
            var c = b.Dequeue();

            domain = BackQueues.Values.OrderBy(q => q.Count).First().Dequeue().GetDomain;

            var url = BackQueues[domain].Dequeue();

            // Update BackQueue if necessary.
            if (BackQueues[domain].Count == 0)
            {
                BackQueueRouter();
            }

            BackQueueHeapSimulator[url.GetDomain] = DateTime.Now;

            return url;
        }
    }
}
