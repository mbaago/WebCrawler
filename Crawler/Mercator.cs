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
            FrontQueues = new List<string>(frontQueues);
            BackQueues = new List<string>(backQueues);
            BackQueueHeap = new Dictionary<DateTime, string>();
        }
        
        private List<string> FrontQueues { get; set; }
        private List<string> BackQueues { get; set; }
        private Dictionary<DateTime, string> BackQueueHeap { get; set; }



        public bool AddURLToFrontQueue(string prettyURL)
        {
            throw new NotImplementedException();
        }

        public string GetURLToCrawl()
        {
            throw new NotImplementedException();
        }
    }
}
