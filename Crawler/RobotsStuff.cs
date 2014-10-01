using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using URLStuff;
using System.Diagnostics;

namespace Peter
{
    class RobotsStuff
    {
        private static string ROBOT = "robot";

        public RobotsStuff(TimeSpan maxAge)
        {
            MaxAge = maxAge;
            CachedRegexes = new Dictionary<string, Tuple<DateTime, Regex>>();
        }

        private Dictionary<string, Tuple<DateTime, Regex>> CachedRegexes { get; set; }

        private Stopwatch regexCalcWatch = new Stopwatch();
        public Dictionary<string, int> regexCalcTimes = new Dictionary<string, int>();

        private Stopwatch robotDownloadWatch = new Stopwatch();
        public Dictionary<string, int> robotDownloadTimes = new Dictionary<string, int>();

        /// <summary>
        /// How old can a robot be before it gets removed.
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        /// <summary>
        /// Parse robots.txt from a specific prettyURL.
        /// Assumes robots.txt is properly formatted.
        /// </summary>
        /// <param name="prettyURL"></param>
        /// <param name="robotsContent"></param>
        /// <returns>A regex to determine if a prettyURL is disallowed.</returns>
        private Regex CalcRobotRegexForDomain(PrettyURL url, IEnumerable<string> robotsContent)
        {
            if (CachedRegexes.ContainsKey(url.GetDomain) && DateTime.Now - CachedRegexes[url.GetDomain].Item1 < MaxAge)
            {
                return CachedRegexes[url.GetDomain].Item2;
            }

            regexCalcWatch.Restart();

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
                //regPattern.Append(prettyURL);
                regPattern.Append(s);
            }
            regPattern.Append(')');
            regPattern.Replace("*", ".*").Replace(".", "\\.").Replace("+", "\\+");

            CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, new Regex(regPattern.ToString()));

            regexCalcWatch.Stop();
            regexCalcTimes[url.GetDomain] = (int)regexCalcWatch.Elapsed.TotalMilliseconds;

            return CachedRegexes[url.GetDomain].Item2;
        }

        public bool IsVisitAllowed(PrettyURL url)
        {
            if (CachedRegexes.ContainsKey(url.GetDomain))
            {
                // Too old?
                if (DateTime.Now - CachedRegexes[url.GetDomain].Item1 > MaxAge)
                {
                    var robotContent = DownloadRobotContent(url).Split('\n');

                    var regex = CalcRobotRegexForDomain(url, robotContent);
                    CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, regex);
                }
            }
            else
            {
                var robotContent = DownloadRobotContent(url).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var regex = CalcRobotRegexForDomain(url, robotContent);
                CachedRegexes[url.GetDomain] = new Tuple<DateTime, Regex>(DateTime.Now, regex);
            }

            return !CachedRegexes[url.GetDomain].Item2.IsMatch(url.GetPrettyURL);
        }

        private string DownloadRobotContent(PrettyURL url)
        {
            string content = "";
            robotDownloadWatch.Restart();

            try
            {
                string robot = "http://" + url.GetDomain + "/" + "robots.txt";
                //System.Diagnostics.Debug.WriteLine("Downloading " + robot);
                content = new System.Net.WebClient().DownloadString(robot);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine("Robot: Could not download " + url);
            }

            robotDownloadWatch.Stop();
            robotDownloadTimes[url.GetDomain] = (int)robotDownloadWatch.Elapsed.TotalMilliseconds;

            return content;
        }

        public void RemoveRobot(PrettyURL url)
        {
            CachedRegexes.Remove(url.GetDomain);
        }
    }
}
