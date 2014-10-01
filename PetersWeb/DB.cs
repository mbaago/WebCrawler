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

        public void DeleteAllPages()
        {
            dbCon.DeleteAllPages();
        }

        public void InsertNewDownloadedPage(string prettyURL, string html)
        {
            Page page = new Page();
            page.url = prettyURL;
            page.html = html;

            dbCon.Pages.InsertOnSubmit(page);
            dbCon.SubmitChanges();
        }

        //public void InsertTokens(string prettyURL, IEnumerable<string> tokens)
        //{
        //    foreach (var t in tokens)
        //    {
        //        dbCon.insertToken(t, prettyURL);
        //    }
        //}

        // slow slow slow, but good enough for now
        public void ManualTokenInserter(string prettyURL, IEnumerable<string> tokens)
        {
            Page page = GetPageFromURL(prettyURL);

            var groupedTokens = from tt in tokens
                                group tt by tt into grouped
                                select grouped;

            foreach (var group in groupedTokens)
            {
                var dbTermToPage = dbCon.TermToPages
                    .Where(t => t.Term.term1 == group.Key)
                    .FirstOrDefault();

                if (dbTermToPage == null)
                {
                    TermToPage ttp = new TermToPage()
                    {
                        count = group.Count(),
                        Page = page,
                        Term = new Term() { term1 = group.Key }
                    };
                    dbCon.TermToPages.InsertOnSubmit(ttp);
                }
                else
                {
                    dbTermToPage.count += group.Count();
                }
            }

            dbCon.SubmitChanges();
        }

        public void InsertShingles(string prettyURL, IEnumerable<int> shingles)
        {
            var page = GetPageFromURL(prettyURL);

            var toInsert = shingles
                .Select(s => new Shingle()
                {
                    Page = page,
                    shingle1 = s
                });

            dbCon.Shingles.InsertAllOnSubmit(toInsert);
        }

        public Page GetPageFromURL(string prettyURL)
        {
            var pages = from p in dbCon.Pages
                        where p.url.ToString() == prettyURL
                        select p;

            return pages.FirstOrDefault();
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


        public IEnumerable<IEnumerable<int>> GetAllShingleSets()
        {
            var shingles = from p in dbCon.Pages
                           select p.Shingles.Select(s => s.shingle1);

            return shingles;
        }
    }
}
