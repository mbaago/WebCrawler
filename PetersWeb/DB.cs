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
            // slet all shingles
        }

        public Page getPageOnUrl(string url)
        {
            var pages = from p in dbCon.Pages
                        where p.url.ToString() == url
                        select p;

            return pages.FirstOrDefault();
        }

        public void clearPages()
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
            var page = getPageOnUrl(prettyURL);
            if (page == null)
            {
                return Enumerable.Empty<int>();
            }

            var shingles = from p in dbCon.Shingles
                           where p.url == page.id
                           select p.shingle1;

            return shingles;
        }

        public string GetSiteContentFromPrettyURL(string prettyURL)
        {
            var page = getPageOnUrl(prettyURL);
            //return dbCon.PageContents
            //    .Where(p => p.id == page.id)
            //    .Select(p => p.pageContent1)
            //    .FirstOrDefault();


            throw new NotImplementedException();
        }
    }
}
