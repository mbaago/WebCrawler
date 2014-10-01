using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetersWeb
{
    public class DB
    {
        DBContextDataContext dbCon = new DBContextDataContext();

        public void InsertNewDownloadedPage(string www, string html)
        {
            Page page = new Page();
            page.url = www;
            page.html = html;

            dbCon.Pages.InsertOnSubmit(page);
            dbCon.SubmitChanges();
        }

        public void InsertShingles(string prettyURL, IEnumerable<int> shingles)
        {
            throw new NotImplementedException();
        }

        public Page GetPageFromURL(string prettyURL)
        {
            var pages = from p in dbCon.Pages
                        where p.url.ToString() == prettyURL
                        select p;

            return pages.FirstOrDefault();
        }

        public void DeleteAllPages()
        {
            dbCon.DeleteAllPages();
        }

        public IEnumerable<Page> GetAllPages()
        {
            var pages = dbCon.Pages;
            return pages;
        }

        public IEnumerable<int> GetShinglesFromPrettyURL(string prettyURL)
        {
            var page = GetPageFromURL(prettyURL);
            if (page == null)
            {
                return Enumerable.Empty<int>();
            }
            var shingles = page.Shingles
                .Select(s => s.shingle1);
            return shingles;
        }

        public void InsertTokens(string prettyURL, IEnumerable<string> tokens)
        {
            foreach (var t in tokens)
            {
                dbCon.insertToken(t, prettyURL);
            }
        }
    }
}
