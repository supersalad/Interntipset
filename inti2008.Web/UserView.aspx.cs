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
    public partial class UserView : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var userGUIDParm = this.GetRedirectParameter("UserGUID", true);
                if (userGUIDParm == null)
                    throw new Exception("Ingen användare vald");

                var userGUID = new Guid(userGUIDParm.ToString());

                ViewState["userGUID"] = userGUID;

                GetUserHeader();
                GetUserTeams();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //make full row clickable?
            foreach (GridViewRow r in UserTeams.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            base.Render(writer);
        }

        private void GetUserHeader()
        {
            var user = new UserManagement(Global.ConnectionString, SessionProps).GetUserByGuid((Guid)ViewState["userGUID"]);

            UserName.Text = String.Format("{0} {1}", user.FirstName, user.LastName);
        }

        private void GetUserTeams()
        {
            using (var db=Global.GetConnection())
            {
                IQueryable<Inti_Team> teamsQ;
                //for admins and the same users, show also non-paid teams
                if (SessionProps.UserGuid.Equals((Guid)ViewState["userGUID"]))
                {
                    teamsQ = from t in db.Inti_Team
                             where t.Sys_User.GUID == (Guid)ViewState["userGUID"]
                                   && t.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                             select t;    
                }
                else
                {
                    if (SessionProps.HasPermission("ADMIN_SYSTEM"))
                    {
                        teamsQ = from t in db.Inti_Team
                                 where t.Sys_User.GUID == (Guid)ViewState["userGUID"]
                                       && t.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                                 select t;
                    }
                    else
                    {
                        teamsQ = from t in db.Inti_Team
                                 where t.Sys_User.GUID == (Guid) ViewState["userGUID"]
                                       && t.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                                       && t.IsPaid == true
                                       && t.IsActive == true
                                 select t;
                    }
                }
                

                UserTeams.DataKeyNames = new string[]{"GUID"};
                UserTeams.DataSource = teamsQ.ToList();
                UserTeams.DataBind();
            }
        }

        protected void UserTeams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
             var gridView = (GridView)sender;
             switch (e.CommandName)
             {
                 case ("SingleClick"):
                     //set selection from command argument
                     gridView.SelectedIndex = int.Parse(e.CommandArgument.ToString());
                     WebControlManager.RedirectWithQueryString("UserTeamView.aspx", new string[] { "TeamGUID" }, new string[] { gridView.SelectedValue.ToString() });
                     break;
             }
        }
    }
}
