using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using URLStuff;

namespace Peter
{
    class Mercator
    {
        public Mercator(int frontQueues, int backQueues, TimeSpan timeBetweenVisits, IEnumerable<PrettyURL> seed)
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

            AllURLS = new List<int>();
            

            foreach (var s in seed)
            {
                var x = BackQueues.Where(q => q.Value.Count > 0);

                if (BackQueues.Count == backQueues)
                {
                    break;
                }

                BackQueues[s.GetDomain] = new Queue<PrettyURL>();
                BackQueues[s.GetDomain].Enqueue(s);
                BackQueueHeapSimulator[s.GetDomain] = DateTime.MinValue;

                // We don't want to visit sites in the seed again -.-
                AllURLS.Add(s.GetPrettyURL.GetHashCode());
            }
        }

        public TimeSpan TimeBetweenVisits { get; set; }
        private int MaxNumberOfBackQueues { get; set; }
        private List<Queue<PrettyURL>> FrontQueues { get; set; }
        private Dictionary<string, Queue<PrettyURL>> BackQueues { get; set; }
        private Dictionary<string, DateTime> BackQueueHeapSimulator { get; set; }
        
        /// <summary>
        /// All URLS, in fq, bq or visited.
        /// </summary>
        private List<int> AllURLS { get; set; }

        public bool AddURLToFrontQueue(PrettyURL url)
        {
            // tjek om allerede besøgt
            if (AllURLS.Contains(url.GetPrettyURL.GetHashCode()))
            {
                return false;
            }

            int rand = new Random().Next(0, FrontQueues.Count);
            FrontQueues[rand].Enqueue(url);
            AllURLS.Add(url.GetPrettyURL.GetHashCode());

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
            var empty = BackQueues.Where(b => b.Value.Count == 0).Select(b => b.Key).ToArray();
            foreach (var q in empty)
            {
                BackQueues.Remove(q);
            }

            while (BackQueues.Count < MaxNumberOfBackQueues && FrontQueues.Where(q => q.Count > 0).Count() > 0)
            {
                var url = FrontQueueSelector();

                if (BackQueues.ContainsKey(url.GetDomain))
                {
                    // We already have a backqueue for this domain.
                    BackQueues[url.GetDomain].Enqueue(url);
                }
                else
                {
                    // Create a new backqueue for this domain.
                    BackQueues[url.GetDomain] = new Queue<PrettyURL>();
                    BackQueues[url.GetDomain].Enqueue(url);

                    BackQueueHeapSimulator[url.GetDomain] = DateTime.Now;
                }
            }
        }

        public PrettyURL GetURLToCrawl()
        {
            var oldDomains = BackQueueHeapSimulator.Where(h => BackQueues.Keys.Contains(h.Key));
            var a = oldDomains.OrderBy(q => q.Value);
            var oldDomain = a.First();

            // Wait until old enough.
            if (DateTime.Now - oldDomain.Value < TimeBetweenVisits)
            {
                //Debug.WriteLine("Sleeping for " + oldDomain);
                while (DateTime.Now - oldDomain.Value < TimeBetweenVisits)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            var url = BackQueues[oldDomain.Key].Dequeue();
            //Debug.WriteLine("GetURLToCrawl: " + url);

            BackQueueRouter();
            BackQueueHeapSimulator[url.GetDomain] = DateTime.Now;

            return url;
        }

        private void CleanupBackQueueHeap()
        {
            // Get domains not in a backqueue
            var domainsNotInBackQueue = BackQueueHeapSimulator.Where(h => !BackQueues.ContainsKey(h.Key)).Select(h => h.Key).ToArray();

            foreach (var domain in domainsNotInBackQueue)
            {
                // Remove if old enough
                if (DateTime.Now - BackQueueHeapSimulator[domain] > TimeBetweenVisits)
                {
                    BackQueueHeapSimulator.Remove(domain);
                }
            }
        }
    }
}
