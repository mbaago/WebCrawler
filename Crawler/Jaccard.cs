using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class Jaccard
    {
        public Jaccard(int shingleSize, double howCloseForDup)
        {
            ShingleSize = shingleSize;
            HowCloseBeforeDuplicate = howCloseForDup;
        }

        int ShingleSize { get; set; }
        double HowCloseBeforeDuplicate { get; set; }

        public bool IsNearDuplicate(string s1, string s2)
        {
            double jaccard = GetJaccardSimilarity(s1, s2);
            return jaccard >= HowCloseBeforeDuplicate;
        }

        public bool IsNearDuplicate(string[] s1, string[] s2)
        {
            double jaccard = GetJaccardSimilarity(s1, s2);
            return jaccard >= HowCloseBeforeDuplicate;
        }

        public double GetJaccardSimilarity(string[] s1, string[] s2)
        {
            if (s1.Length < ShingleSize || s2.Length < ShingleSize)
            {
                return double.NaN;
            }

            var shingles1 = GetShinglesHashes(s1);
            var shingles2 = GetShinglesHashes(s2);

            int cap = shingles1.Intersect(shingles2).Count();
            int cup = shingles1.Union(shingles2).Count();

            return (double)cap / cup;
        }

        /// <summary>
        /// Calculate the Jaccard similarity between two strings.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <returns>The Jaccard similarity between the two input strings.</returns>
        public double GetJaccardSimilarity(string s1, string s2)
        {
            var wordsInS1 = getWordsInSentence(s1);
            var wordsInS2 = getWordsInSentence(s2);

            return GetJaccardSimilarity(wordsInS1, wordsInS2);
        }


        /// <summary>
        /// Convert a string to an array of the words in the string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string[] getWordsInSentence(string s)
        {
            char[] sep = { ' ', ',', '.' };
            //var words = s.Split(sep).Except(new string[] { "" }).Distinct().ToArray();
            var words = s.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToArray();
            return words;
        }

        /// <summary>
        /// Create a set of shingles from a set of words.
        /// </summary>
        /// <param name="words">The words to create the shingles from.</param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <returns></returns>
        private IEnumerable<int> GetShinglesHashes(string[] words)
        {
            var shingles = new List<int>();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < words.Count() - ShingleSize + 1; i++)
            {
                for (int j = 0; j < ShingleSize; j++)
                {
                    builder.Append(words[j + i]);
                }

                shingles.Add(builder.ToString().GetHashCode());

                // We reuse the StringBuilder, so it must be cleared first.
                builder.Clear();
            }

            return shingles;
        }
    }
}
