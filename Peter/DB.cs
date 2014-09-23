using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peter
{
    class DB
    {


        public void insertNew(string www, string html)
        {
            DBContextDataContext dbCon = new DBContextDataContext();
            Page page = new Page();
            page.url = www;
            page.html = html;

            dbCon.Pages.InsertOnSubmit(page);
            dbCon.SubmitChanges();
        }

        public Page getPageOnUrl(string url)
        {
            DBContextDataContext dbCon = new DBContextDataContext();
            var pages = from p in dbCon.Pages
                        where p.url.ToString() == url
                        select p;

            return pages.FirstOrDefault();
        }
    }

   
}
