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
    public partial class ForumViewThread : IntiPage
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

            //hide forum if no thread id
            var threadGUID = this.GetRedirectParameter("threadGUID", false);
            var categoryGUID = this.GetRedirectParameter("categoryGUID", false);
            pnlViewForumItems.Visible = (threadGUID != null);

            //hide possibility to write if not signed in
            pnlAddForumItem.Visible = !(String.IsNullOrEmpty(SessionProps.UserName));

            //which button to show?
            if (pnlAddForumItem.Visible)
            {
                btnReply.Visible = (threadGUID != null);
                btnCreateForumItem.Visible = (categoryGUID != null);
            }

            if (threadGUID != null)
                LoadThread(new Guid(threadGUID.ToString()));
        }

        private void LoadThread(Guid guid)
        {
            
            using (var db = Global.GetConnection())
            {
                var forumItems = from ft in db.Ext_Forum
                                 where ft.GUID == guid || ft.ResponseToGUID == guid
                                 select new
                                            {
                                                ft.GUID,
                                                ft.Header,
                                                ft.Body,
                                                ft.PostedDate,
                                                Author =
                                     String.Format("{0} {1}", ft.Sys_User.FirstName, ft.Sys_User.LastName).Trim()
                                            };

                var theList = forumItems.OrderBy(ft => ft.PostedDate).ToList();

                rptForum.DataSource = theList;
                rptForum.DataBind();

                //suggest header for reply
                Header.Text = "Sv: " + theList[theList.Count - 1].Header;
            }
        }

        protected void btnCreateForumItem_Click(object sender, EventArgs e)
        {
            var categoryGUID = this.GetRedirectParameter("categoryGUID", false);

            if (categoryGUID != null && !String.IsNullOrEmpty(SessionProps.UserName)
                && SessionProps.HasPermission("USER")
                && Header.Text.Trim().Length > 0  && Body.Text.Trim().Length > 0)
            {
                using(var db = Global.GetConnection())
                {
                    var forum = new Ext_Forum();
                    forum.Header = Header.Text;
                    forum.Body = Body.Text;
                    forum.PostedDate = DateTime.Now;
                    forum.ForumCategoryGUID = new Guid(categoryGUID.ToString());
                    forum.UserGUID = SessionProps.UserGuid;
                    
                    db.Ext_Forum.InsertOnSubmit(forum);

                    db.SubmitChanges();

                    WebControlManager.RedirectWithQueryString("ForumViewThread.aspx", new string[]{"threadGUID"}, new string[]{forum.GUID.ToString()} );
                }
            }
            else
            {
                lblMessage.Text =
                    "Det gick inte att spara inlägget. Är du inloggad? Har du fyllt i både rubrik och brödtext?";
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            var threadGUID = this.GetRedirectParameter("threadGUID", false);

            if (threadGUID != null && !String.IsNullOrEmpty(SessionProps.UserName)
                && SessionProps.HasPermission("USER")
                && Header.Text.Trim().Length > 0 && Body.Text.Trim().Length > 0)
            {
                using (var db = Global.GetConnection())
                {
                    var replyForum = db.Ext_Forum.Single(f => f.GUID == new Guid(threadGUID.ToString()));

                    var forum = new Ext_Forum();
                    forum.Header = Header.Text;
                    forum.Body = Body.Text;
                    forum.PostedDate = DateTime.Now;
                    forum.ResponseToGUID = replyForum.GUID;
                    forum.ForumCategoryGUID = replyForum.ForumCategoryGUID;
                    forum.UserGUID = SessionProps.UserGuid;

                    db.Ext_Forum.InsertOnSubmit(forum);

                    db.SubmitChanges();

                    WebControlManager.RedirectWithQueryString("ForumViewThread.aspx", new string[] { "threadGUID" }, new string[] { replyForum.GUID.ToString() });
                }
            }
            else
            {
                lblMessage.Text =
                    "Det gick inte att spara inlägget. Är du inloggad? Har du fyllt i både rubrik och brödtext?";
            }
        }
    }
}
