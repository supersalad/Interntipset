using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
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
    public partial class AdminApproveUserTeams : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_SYSTEM");

            lblTournament.Text = SessionProps.DefaultTournament.Name;
            if (!IsPostBack)
                LoadTeams();
        }

        private void LoadTeams()
        {
            using (var db =  Global.GetConnection())
            {
                var teams = from t in db.Inti_Team
                            where t.TournamentGUID == SessionProps.DefaultTournament.GUID
                            select new
                                       {
                                           GUID = t.GUID,
                                           Name = t.Name,
                                           TeamManager = t.Sys_User.FirstName + " " + t.Sys_User.LastName,
                                           TeamManagerMail = t.Sys_User.UserName,
                                           t.IsActive,
                                           t.IsPaid,
                                           t.BonusPoints,
                                           t.Picture
                                       };

                grdUserTeams.DataKeyNames = new string[]{"GUID"};
                grdUserTeams.DataSource = teams.OrderBy(t => t.Name);
                grdUserTeams.DataBind();

                var countList = teams.ToList();

                lblTournament.Text += ". " + countList.Count() + " lag varav " + countList.Count(t => t.IsPaid ?? false) + " betalda.";
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdUserTeams.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl09");
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl10");
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl11");
                }
            }

            //todo: register page browsing for event validation


            base.Render(writer);
        }

        protected void grdUserTeams_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                var _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                // Add this javascript to the onclick Attribute of the row

                e.Row.Cells[1].Attributes["onclick"] = _jsSingle;
                e.Row.Cells[2].Attributes["onclick"] = _jsSingle;
                //e.Row.Cells[3].Attributes["onclick"] = _jsSingle;

                //non marked style
                e.Row.Cells[9].BackColor = Color.LightYellow;
                e.Row.Cells[10].BackColor = Color.LightYellow;
                e.Row.Cells[11].BackColor = Color.LightYellow;

                //is activated already?
                if(e.Row.Cells[5].Text == "1" || e.Row.Cells[5].Text.ToLower() == "true")
                {
                    (e.Row.Cells[9].Controls[0] as LinkButton).Text = "Avaktivera";
                    e.Row.Cells[9].BackColor = Color.LightGreen;
                }
                    

                //is marked as paid?
                if (e.Row.Cells[6].Text == "1" || e.Row.Cells[6].Text.ToLower() == "true")
                {
                    (e.Row.Cells[10].Controls[0] as LinkButton).Text = "Markera som obetalt";
                    e.Row.Cells[10].BackColor = Color.LightGreen;
                }
                    

                //has bonus points?
                if (e.Row.Cells[7].Text == "2")
                {
                    (e.Row.Cells[11].Controls[0] as LinkButton).Text = "Ta bort bonus";
                    e.Row.Cells[11].BackColor = Color.LightGreen;
                }
                    
            }
        }

        protected void grdUserTeams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    //WebControlManager.RedirectWithQueryString("UserTeam/UserTeamEdit_js.aspx", new string[] { "teamGUID" }, new string[] { _gridView.SelectedValue.ToString() });
                    WebControlManager.RedirectWithQueryString("UserTeamEdit.aspx", new string[] { "teamGUID" }, new string[] { _gridView.SelectedValue.ToString() });
                    break;
                case ("Activate"):
                    _gridView.SelectedIndex = _selectedIndex;
                    new UserTeamManagement(Global.ConnectionString, SessionProps).ActivateTeam(new Guid(_gridView.SelectedValue.ToString()));
                    LoadTeams();
                    break;
                case ("MarkAsPaid"):
                    _gridView.SelectedIndex = _selectedIndex;
                    new UserTeamManagement(Global.ConnectionString, SessionProps).MarkTeamAsPaid(new Guid(_gridView.SelectedValue.ToString()));
                    LoadTeams();
                    break;
                case ("Bonus"):
                    _gridView.SelectedIndex = _selectedIndex;
                    new UserTeamManagement(Global.ConnectionString, SessionProps).ToggleBonus(new Guid(_gridView.SelectedValue.ToString()));
                    LoadTeams();
                    break;
            }
        }
    }
}
