using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Mercator
    {
        public Mercator(int frontQueues, int backQueues)
        {
            FrontQueues = new List<PrettyURL>(frontQueues);
            BackQueues = new Dictionary<string, Queue<PrettyURL>>();
            BackQueueHeap = new Dictionary<DateTime, string>();
        }

        private int MaxNumberOfBackQueues { get; set; }

        private List<PrettyURL> FrontQueues { get; set; }
        private Dictionary<string, Queue<PrettyURL>> BackQueues { get; set; }
        private Dictionary<DateTime, string> BackQueueHeap { get; set; }



        public bool AddURLToFrontQueue(PrettyURL url)
        {


            throw new NotImplementedException();
        }

        public PrettyURL GetURLToCrawl()
        {
            throw new NotImplementedException();
        }
    }
}
