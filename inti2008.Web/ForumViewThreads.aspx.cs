using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class ForumViewThreads : IntiPage
    {
        public override bool AuthenticationIsRequired
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAddNewThread.Visible = (!String.IsNullOrEmpty(SessionProps.UserName));


            //delete thread?
            if (SessionProps.HasPermission("ADMIN_FORUM"))
            {
                var deleteThreadGuid = this.GetRedirectParameter("deleteGUID", true);
                if (deleteThreadGuid != null)
                {
                    using (var db = Global.GetConnection())
                    {
                        //do the delete

                        var thread =
                            db.Ext_Forum.SingleOrDefault(f => f.GUID == new Guid(deleteThreadGuid.ToString()));

                        if (thread != null)
                        {
                            db.Ext_Forum.DeleteOnSubmit(thread);

                            db.SubmitChanges();

                            Response.Redirect("ForumViewThreads.aspx");
                        }
                    }
                }
            }

            if (!IsPostBack)
            {
                LoadCategories();
                LoadThreads();
            }
                
        }

        
        protected void drpForumCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadThreads();
        }

        private void LoadThreads()
        {
            using (var db = Global.GetConnection())
            {
                var threads = from t in db.Ext_Forum
                              where t.ForumCategoryGUID == new Guid(drpForumCategory.SelectedValue)
                                    && t.ResponseToGUID == null
                              select new
                              {
                                  t.GUID,
                                  t.Header,
                                  t.PostedDate,
                                  IsDeletable = SessionProps.HasPermission("ADMIN_FORUM")
                              };

                rptForum.DataSource = threads.OrderByDescending(t => t.PostedDate).ToList();
                rptForum.DataBind();
            }
        }

        private void LoadCategories()
        {
            using(var db = Global.GetConnection())
            {
                var categories = from c in db.Ext_ForumCategory
                                 where c.TournamentGUID == SessionProps.SelectedTournament.GUID
                                 select c;

                drpForumCategory.DataValueField = "GUID";
                drpForumCategory.DataTextField = "Name";
                drpForumCategory.DataSource = categories.ToList();
                drpForumCategory.DataBind();

                if (drpForumCategory.Items.Count == 1)
                {
                    pnlChooseCategory.Visible = false;
                }
            }
        }

        
        protected void btnAddNewThread_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("ForumViewThread.aspx", new string[] {"categoryGUID"}, new string[]{drpForumCategory.SelectedValue} );
        }
    }
}
