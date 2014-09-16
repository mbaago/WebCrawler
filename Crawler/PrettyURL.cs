using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class PrettyURL
    {
        public PrettyURL(string url)
        {
            MakePrettyURL = new Lazy<string>(() => URLStuff.MakeURLPretty(url));
            ExtractDomain = new Lazy<string>(() => URLStuff.ExtractDomain(url));
        }

        private Lazy<string> MakePrettyURL { get; set; }
        private Lazy<string> ExtractDomain { get; set; }

        public string GetPrettyURL { get { return MakePrettyURL.Value; } }
        public string GetDomain { get { return ExtractDomain.Value; } }


        public override string ToString()
        {
            return GetPrettyURL;
        }
    }
}
