using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class ForumEntries : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadLatestEntries(int resultNumber)
        {

            using (var db = Global.GetConnection())
            {
                var forumItems = from ft in db.Ext_Forum
                                 select new
                                 {
                                     ft.GUID,
                                     ThreadGUID = ft.ResponseToGUID ?? ft.GUID,
                                     ft.Header,
                                     ft.Body,
                                     ft.PostedDate,
                                     Author =
                          String.Format("{0} {1}", ft.Sys_User.FirstName, ft.Sys_User.LastName).Trim()
                                 };

                var theList = forumItems.OrderByDescending(ft => ft.PostedDate).Take(resultNumber).ToList();
                
                rptForum.DataSource = theList;
                rptForum.DataBind();

            }
        }

        

    }
}