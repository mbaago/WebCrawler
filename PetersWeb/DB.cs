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

        public void insertNew(string www, string html)
        {
            
            Page page = new Page();
            page.url = www;
            page.html = html;

            dbCon.Pages.InsertOnSubmit(page);
            dbCon.SubmitChanges();
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
    }


}
