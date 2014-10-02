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

        public void ManualTokenInserter(string prettyURL, IEnumerable<string> tokens)
        {
            Page page = GetPageFromURL(prettyURL);

            var groupedTokens = from tt in tokens
                                group tt by tt into grouped
                                orderby grouped.Count() descending
                                select grouped;

            foreach (var group in groupedTokens)
            {
                var tokenID = dbCon.Terms.Where(t => t.term1 == group.Key).FirstOrDefault();

                ////Does it already exist?
                //if (tokenID != null)
                //{
                //    var termPage = dbCon.TermToPages
                //                .Where(t => t.termID == tokenID.id && t.pageID == page.id)
                //                .FirstOrDefault();

                //    if (termPage != null)
                //    {
                //        termPage.count = group.Count();
                //    }

                //    continue;
                //}
                //else // did not exist
                {
                    if (tokenID == null)
                    {
                        Term t = new Term()
                        {
                            term1 = group.Key
                        };
                        dbCon.Terms.InsertOnSubmit(t);
                        dbCon.SubmitChanges();
                    }

                    tokenID = dbCon.Terms.Where(t => t.term1 == group.Key).FirstOrDefault();
                    TermToPage tp = new TermToPage()
                    {
                        termID = tokenID.id,
                        count = group.Count(),
                        pageID = page.id
                    };

                    dbCon.TermToPages.InsertOnSubmit(tp);
                }
            }

            dbCon.SubmitChanges();
        }

        public void InsertShingles(string prettyURL, IEnumerable<int> shingles)
        {
            var page = GetPageFromURL(prettyURL);

            var dbSHingles = shingles.Select(s => new Shingle()
            {
                url = page.id,
                shingle1 = s
            });

            dbCon.Shingles.InsertAllOnSubmit(dbSHingles);
            dbCon.SubmitChanges(); // latest change
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

        public IEnumerable<TermToPage> GetInvertedIndexForSingleToken(string token)
        {
            var term = dbCon.Terms.Where(t => t.term1 == token).FirstOrDefault();
            if (term == null)
            {
                return Enumerable.Empty<TermToPage>();
            }

            var pages = dbCon.TermToPages.Where(tp => tp.termID == term.id);
            return pages;
        }

        public int TokenCountInURL(string prettyURL, string token)
        {
            var x = dbCon.TermToPages.Where(t => t.Page.url == prettyURL && t.Term.term1 == token).FirstOrDefault();
            return x.count ?? 0;
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


        public int TotalSitesInDB()
        {
            var siteCount = dbCon.Pages.Count();
            return siteCount;
        }

        public int TermFrequencyInDocument(string term, string prettyURL)
        {
            var tf = GetPageFromURL(prettyURL).TermToPages
                .Where(d => d.Term.term1 == term)
                .Count();

            return tf;
        }

        public int DocumentFrequency(string term)
        {
            var df = dbCon.TermToPages
                .Where(t => t.Term.term1 == term)
                .Count();

            return df;
        }
    }
}
