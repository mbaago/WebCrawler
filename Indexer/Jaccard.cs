using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peter
{
    public class Jaccard
    {
        public Jaccard(int shingleSize, double howCloseForDup, char[] splitChars)
        {
            ShingleSize = shingleSize;
            HowCloseBeforeDuplicate = howCloseForDup;
            SplitChars = splitChars;
        }

        int ShingleSize { get; set; }
        double HowCloseBeforeDuplicate { get; set; }
        char[] SplitChars { get; set; }

        public bool IsNearDuplicate(IEnumerable<int> s1, IEnumerable<int> s2)
        {
            double jaccard = GetJaccardSimilarity(s1, s2);
            return jaccard >= HowCloseBeforeDuplicate;
        }

        public double GetJaccardSimilarity(IEnumerable<int> s1, IEnumerable<int> s2)
        {
            if (s1.Count() < ShingleSize || s2.Count() < ShingleSize)
            {
                return double.NaN;
            }

            int cap = s1.Intersect(s2).Count();
            int cup = s1.Union(s2).Count();

            return (double)cap / cup;
        }

        public IEnumerable<int> GetShinglesFromSpaceSeperatedString(string s)
        {
            var words = getWordsInSentence(s).ToArray();
            var shingles = GetShinglesHashes(words);
            return shingles;
        }

        /// <summary>
        /// Convert a string to an array of the words in the string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string[] getWordsInSentence(string s)
        {
            var words = s.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            return words;
        }

        /// <summary>
        /// Create a set of shingles from a set of words.
        /// </summary>
        /// <param name="words">The words to create the shingles from.</param>
        /// <param name="shingleSize">How many words in a shingle.</param>
        /// <returns></returns>
        public IEnumerable<int> GetShinglesHashes(string[] words)
        {
            var shingles = new List<int>(words.Length);
            if (words.Length < ShingleSize)
            {
                return Enumerable.Empty<int>();
            }

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

            return shingles.Distinct();
        }
    }
}
