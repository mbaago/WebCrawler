using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peter
{
    public static class Danish
    {
        public static string MakeDanish(this string input)
        {
            return input
                .Replace("&aring;", "å")
                .Replace("Ã¥", "å")
                .Replace("&aelig;", "æ")
                .Replace("Ã¦", "æ")
                .Replace("&oslash;", "ø")
                .Replace("&#039;", "'");
            //return input;
        }
    }
}
