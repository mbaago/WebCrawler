using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crawler
{
    class RobotsStuff
    {
        public RobotsStuff(TimeSpan maxAge)
        {
            MaxAge = maxAge;
            CachedRegexes = new Dictionary<string, Tuple<DateTime, Regex>>();
        }

        private Dictionary<string, Tuple<DateTime, Regex>> CachedRegexes { get; set; }

        /// <summary>
        /// How old can a robot be before it gets removed.
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        /// <summary>
        /// Parse robots.txt from a specific url.
        /// Assumes robots.txt is properly formatted.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="robotsContent"></param>
        /// <returns>A regex to determine if a url is disallowed.</returns>
        private Regex CalcRobotRegexForDomain(PrettyURL url, IEnumerable<string> robotsContent)
        {
            if (CachedRegexes.ContainsKey(url.GetDomain) && DateTime.Now - CachedRegexes[url.GetDomain].Item1 < MaxAge)
            {
                return CachedRegexes[url.GetDomain].Item2;
            }

            // No restrictions
            // 
            if (robotsContent == null || robotsContent.Count() == 0)
            {
                return new Regex(@"@.");
            }

            // Find what is disallowed.
            var disallow = robotsContent.SkipWhile(s => !Regex.IsMatch(s, @"User-Agent: \*", RegexOptions.IgnoreCase)) // Start from user agent *.
                .TakeWhile(s => !string.IsNullOrWhiteSpace(s)) // Read until blank line (where allow/disallow hopefully ends).
                .Skip(1) // Skip the user agent string.
                .Where(s => s.StartsWith("Disallow")) // We only need disallowed stuff.
                .Select(s => s.Split(':').Last().Trim()); // Select the disallowed stuff.

            if (disallow.Count() == 0)
            {
                return new Regex(@"$.");
            }

            // Build the regex string
            StringBuilder regPattern = new StringBuilder(url + "(" + disallow.First());
            foreach (var s in disallow.Skip(1))
            {
                regPattern.Append('|');
                //regPattern.Append(url);
                regPattern.Append(s);
            }
            regPattern.Append(')');
            regPattern.Replace("*", ".*").Replace(".", "\\.");

            CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, new Regex(regPattern.ToString()));
            return CachedRegexes[url.GetDomain].Item2;
        }

        public bool IsVisitAllowed(PrettyURL url)
        {
            if (CachedRegexes.ContainsKey(url.GetDomain))
            {
                // Too old?
                if (DateTime.Now - CachedRegexes[url.GetDomain].Item1 > MaxAge)
                {
                    System.Diagnostics.Debug.WriteLine("Old robot: " + url, "ROBOT");
                    var robotContent = DownloadRobotContent(url).Split('\n');
                    var regex = CalcRobotRegexForDomain(url, robotContent);
                    CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, regex);
                }
            }
            else
            {
                var robotContent = DownloadRobotContent(url).Split('\n');
                var regex = CalcRobotRegexForDomain(url, robotContent);
                CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, regex);
            }

            return !CachedRegexes[url.GetDomain].Item2.IsMatch(url.GetPrettyURL);
        }

        private string DownloadRobotContent(PrettyURL url)
        {
            try
            {
                string robot = "http://" + url.GetDomain + "/" + "robots.txt";
                System.Diagnostics.Debug.WriteLine("Robot: Downloading " + robot);
                string robotContent = new System.Net.WebClient().DownloadString(robot);
                return robotContent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Robot: Could not download " + url);
                throw;
            }
        }

        public void RemoveRobot(PrettyURL url)
        {
            CachedRegexes.Remove(url.GetDomain);
        }
    }
}
