using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class ViewChangeLog : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private SessionProperties SessionProperties
        {
            get
            {
                if (Page is IntiPage)
                    return (Page as IntiPage).SessionProps;

                throw new InvalidOperationException("ViewChangeLog run on a non IntiPage page");
            }
        }

        public void LoadChangeLog(Guid objectID)
        {
            using(var db = Global.GetConnection())
	        {
                var matchManagement = new MatchManagement(Global.ConnectionString, SessionProperties);
                var q = from cl in matchManagement.GetChangeLog(db, objectID)
                        select new
                        {
                            cl.LogDate,
                            cl.Action,
                            UserName = cl.Sys_User.FirstName + " " + cl.Sys_User.LastName,
                            cl.Client,
                            cl.Message
                        };
	            grdChangeLog.DataSource = q.Take(25).OrderByDescending(cl=>cl.LogDate);
                grdChangeLog.DataBind();
	        }
        }

        public void LoadChangeLog(Type objectType)
        {
            using (var db = Global.GetConnection())
            {
                var matchManagement = new MatchManagement(Global.ConnectionString, SessionProperties);
                var q = from cl in matchManagement.GetChangeLog(db, objectType)
                        select new
                                   {
                                       cl.LogDate,
                                       cl.Action,
                                       UserName = cl.Sys_User.FirstName + " " + cl.Sys_User.LastName,
                                       cl.Client,
                                       cl.Message
                                   };

                grdChangeLog.DataSource = q.Take(25).OrderByDescending(cl=>cl.LogDate);
                grdChangeLog.DataBind();
            }
        }

        public void ClearLog()
        {
            grdChangeLog.DataSource = null;
            grdChangeLog.DataBind();
        }
    }
}