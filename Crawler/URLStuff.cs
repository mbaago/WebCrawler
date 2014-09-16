using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    static class URLStuff
    {
        public static string MakeURLPretty(string url)
        {
            var lowURL = url.ToLower();

            if (lowURL.StartsWith("http://www"))
            {
                // Everything's good.
            }
            else if (lowURL.StartsWith("http://"))
            {
                var s = url.Split(new string[] { "//" }, StringSplitOptions.None);
                url = s[0] + "//www." + s[1];
            }
            else if (lowURL.StartsWith("www"))
            {
                url = "http://" + url;
            }
            else
            {
                url = "http://www." + url;
            }

            url = new Uri(url).ToString();

            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        public static string ExtractDomain(string url)
        {
            url = MakeURLPretty(url);
            var splitter = url.Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries);
            return splitter[1];
        }

        public static bool IsValidURL(string url)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(url, @"http://www\..*");
        }
    }
}
