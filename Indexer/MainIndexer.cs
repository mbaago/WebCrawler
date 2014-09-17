using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public class MainIndexer
    {

        public IEnumerable<string> WordExtractor(string html)
        {
            var tagStartSplitted = html.Split('<');
            var notTags = tagStartSplitted.Where(s => !s.EndsWith(">"));


            return notTags;
        }
    }
}
