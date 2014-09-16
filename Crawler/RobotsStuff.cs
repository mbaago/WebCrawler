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
            Robots = new Dictionary<string, Tuple<DateTime, string[]>>();
            CachedRegexes = new Dictionary<string, Tuple<DateTime, Regex>>();
        }

        /// <summary>
        /// key: domain
        /// value:
        ///     key: timestamp
        ///     value: file contents
        /// </summary>
        private Dictionary<string, Tuple<DateTime, string[]>> Robots { get; set; }

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
        public Regex IsURLInDisallowedList(PrettyURL url)
        {
            DownloadRobotsIfTooOld(url);
            var robotsContent = Robots[url.GetDomain].Item2;
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
            var robotContent = GetRobotContent_DownloadsIfNecessary(url);
            return !IsURLInDisallowedList(url).IsMatch(url.GetPrettyURL);
        }

        private IEnumerable<string> GetRobotContent_DownloadsIfNecessary(PrettyURL url)
        {
            if (!Robots.ContainsKey(url.GetDomain))
            {
                DownloadRobot_AddToRobots(url);
            }

            return Robots[url.GetDomain].Item2;
        }

        public void DownloadRobotsIfTooOld(PrettyURL url)
        {
            if (Robots.ContainsKey(url.GetDomain))
            {
                if (DateTime.Now - Robots[url.GetDomain].Item1 > MaxAge)
                {
                    DownloadRobot_AddToRobots(url);
                }
            }
            else
            {
                DownloadRobot_AddToRobots(url);
            }
        }

        private void DownloadRobot_AddToRobots(PrettyURL url)
        {
            string robot = url.GetDomain + "/" + "robots.txt";
            try
            {
                string content = new System.Net.WebClient().DownloadString(robot);
                Robots[url.GetDomain] = new Tuple<DateTime, string[]>(DateTime.Now, robot.Split('\n'));
            }
            catch (Exception ex)
            {
                // tough luck
                System.Diagnostics.Debug.WriteLine("Robot: Error downloading " + url);
            }
        }

        public void RemoveRobot(PrettyURL url)
        {
            Robots.Remove(url.GetDomain);
            CachedRegexes.Remove(url.GetDomain);
        }
    }
}
