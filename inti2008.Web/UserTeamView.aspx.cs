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
    public partial class UserTeamView : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //visa dessa länkar om det är den aktuella användarens sida
            pnlUserSettings.Visible = (SessionProps.UserGuid == Id);

            //hide this button for now
            btnNewTournament.Visible = (SessionProps.UserGuid == Id);


            LoadHeader();

            AddButtonsToAddTeams();

            LoadTeams();

            LoadMyTournaments();

            LoadPrivateTournaments();
        }

        private void LoadPrivateTournaments()
        {
            if (SessionProps.UserGuid == Id)
            {
                using (var db = Global.GetConnection())
                {
                    var tournaments = from ptp in db.Ext_PrivateTournamentParticipant
                                      where ptp.Inti_Team.UserGUID == Id
                                        && ptp.Ext_PrivateTournament.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                                            &&
                                            (((ptp.IsAccepted ?? false) &&
                                              (ptp.Ext_PrivateTournament.IsVisibleForAll ?? false)) ||
                                             SessionProps.UserGuid == Id)
                                      select new
                                                 {
                                                     ptp.PrivateTournamentGUID,
                                                     ptp.TeamGUID,
                                                     ptp.Ext_PrivateTournament.Name,
                                                     ptp.Ext_PrivateTournament.Description,
                                                     TeamName = ptp.Inti_Team.Name,
                                                     ptp.IsAccepted
                                                 };

                    grdPrivateTournaments.DataKeyNames = new string[] { "PrivateTournamentGUID", "TeamGUID" };
                    grdPrivateTournaments.DataSource = tournaments.OrderBy(p => p.IsAccepted).ToList();
                    grdPrivateTournaments.DataBind();
                } 
            }
        }

        private void LoadMyTournaments()
        {
            if(SessionProps.UserGuid == Id)
            {
                using (var db = Global.GetConnection())
                {
                    var myTournaments = from mt in db.Ext_PrivateTournament
                                        where mt.UserGUID == Id
                                        && mt.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                                        select mt;

                    grdMyTournaments.DataKeyNames = new string[]{"GUID"};
                    grdMyTournaments.DataSource = myTournaments.OrderBy(mt => mt.Name).ToList();
                    grdMyTournaments.DataBind();
                }
            }
        }

        private Guid Id
        {
            get
            {
                var value = this.GetRedirectParameter("GUID", false);
                if (value != null)
                    return new Guid(value.ToString());
                

                return SessionProps.UserGuid;
            }
        }

        private void LoadHeader()
        {
            using (var db = Global.GetConnection())
            {
                var users = from u in db.Sys_User
                           where u.GUID == SessionProps.UserGuid
                           select u;

                var user = users.ToList()[0];

                Header.Text = String.Format("{0} {1}", user.FirstName, user.LastName);
            }
        }

        private void AddButtonsToAddTeams()
        {

            if (Id == SessionProps.UserGuid)
            {
                //loopa tournaments - en knapp för varje tournament som är öppen
                using (var db = Global.GetConnection())
                {

                    var tours = from t in db.Inti_Tournament
                                select t;

                    foreach (var tour in tours.ToList())
                    {
                        //är den öppen för registrering
                        if (tour.PublicateDate <= DateTime.Now &&
                            tour.StartRegistration <= DateTime.Now &&
                            tour.EndRegistration >= DateTime.Now)
                        {
                            var btn = new Button();
                            btn.Text = "Lägg till lag i " + tour.Name;
                            btn.ID = "addTeam" + tour.GUID.ToString();
                            btn.Click += AddTeam_Click;
                            btn.CssClass = "btn btn-primary";

                            plhAddTeams.Controls.Add(btn);
                        }

                    }

                }    
            }
        }

        private void LoadTeams()
        {
            using (var db = Global.GetConnection())
            {
                var userTeams = from ut in db.Inti_Team
                                where ut.Sys_User.GUID == Id
                                      && ut.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID
                                select new
                                           {
                                               ut.GUID,
                                               ut.Name,
                                               ut.IsActive,
                                               ut.IsPaid
                                           };

                UsersTeams.DataKeyNames = new string[]{"GUID"};
                UsersTeams.DataSource = userTeams.ToList();
                UsersTeams.DataBind();

            }
        }

        protected void AddTeam_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var senderButton = (sender as Button);
                if (senderButton.ID.StartsWith("addTeam"))
                {
                    var tourGUID = new Guid(senderButton.ID.Replace("addTeam", ""));

                    WebControlManager.RedirectWithQueryString("UserTeamEdit.aspx", new string[]{"tourGUID"}, new string[]{tourGUID.ToString()} );

                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in UsersTeams.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            //register rowcommands for event validation
            foreach (GridViewRow r in grdMyTournaments.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            foreach (GridViewRow r in grdPrivateTournaments.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl05");
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl06");
                }
            }

            //todo: register page browsing for event validation


            base.Render(writer);
        }

        protected void UsersTeams_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                var _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                // Add this javascript to the onclick Attribute of the row

                e.Row.Attributes["onclick"] = _jsSingle;
            }
        }

        protected void UsersTeams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    
                    if (Id == SessionProps.UserGuid)
                    {
                        WebControlManager.RedirectWithQueryString("UserTeamEdit.aspx", new string[] { "teamGUID" }, new string[] { _gridView.SelectedValue.ToString() });
                        //WebControlManager.RedirectWithQueryString("/UserTeam/UserTeamEdit_js.aspx", new string[] { "teamGUID" }, new string[] { _gridView.SelectedValue.ToString() });
                    }
                    else
                    {
                        WebControlManager.RedirectWithQueryString("TeamView.aspx", new string[] { "teamGUID" }, new string[] { _gridView.SelectedValue.ToString() });
                    }
                    
                    break;
            }
        }
 
        protected void grdMyTournaments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                var _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                // Add this javascript to the onclick Attribute of the row

                e.Row.Attributes["onclick"] = _jsSingle;
            }
        }

        protected void grdMyTournaments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;

                    WebControlManager.RedirectWithQueryString("MyTournamentEdit.aspx", new string[] { "tournamentGUID" }, new string[] { _gridView.SelectedValue.ToString() });

                    break;
            }
        }

        protected void btnNewTournament_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("MyTournamentEdit.aspx", new string[]{"dummy"}, new string[]{"dummy"});
        }

        protected void grdPrivateTournaments_RowDataBound(object sender, GridViewRowEventArgs e)
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
                e.Row.Cells[3].Attributes["onclick"] = _jsSingle;
                e.Row.Cells[4].Attributes["onclick"] = _jsSingle;

                //is accepted already?
                if ((e.Row.Cells[4].Controls[0] as DataBoundLiteralControl).Text.Contains("Utmaningen är accepterad"))
                {
                    e.Row.Cells[5].Controls.Clear();
                    e.Row.Cells[6].Controls.Clear();
                }
                    

            }
        }

        protected void grdPrivateTournaments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            _gridView.SelectedIndex = _selectedIndex;

            var pvtTournamentGUID = new Guid(_gridView.SelectedDataKey[0].ToString());
            var teamGUID = new Guid(_gridView.SelectedDataKey[1].ToString());

            switch (_commandName)
            {
                case ("Click"):
                    //Gå till tävlingen
                    WebControlManager.RedirectWithQueryString("MyTournamentStanding.aspx", new string[]{"pvtTournamentGUID"}, new string[]{pvtTournamentGUID.ToString()} );
                    break;
                case ("Accept"):
                    //ändra utmaningen till accepterad
                    AcceptChallenge(pvtTournamentGUID, teamGUID);
                    break;
                case ("Deny"):
                    //ta bort utmaningen och skicka meddelande till den som utmanade
                    DenyChallenge(pvtTournamentGUID, teamGUID);
                    break;
            }
        }

        private void DenyChallenge(Guid tournamentGUID, Guid teamGUID)
        {
            using (var db = Global.GetConnection())
            {
                var challenge =
                    db.Ext_PrivateTournamentParticipant.Single(
                        ptp => ptp.PrivateTournamentGUID == tournamentGUID && ptp.TeamGUID == teamGUID);

                db.Ext_PrivateTournamentParticipant.DeleteOnSubmit(challenge);

                //skicka meddelande till den som utmanat
                var message = new Ext_Message();

                message.Header = "Utmaning nekad";
                message.Body = String.Format("{0} ({1} {2}) vill inte vara med i din utmaning \"{3} \"",
                                               challenge.Inti_Team.Name, challenge.Inti_Team.Sys_User.FirstName,
                                               challenge.Inti_Team.Sys_User.LastName,
                                               challenge.Ext_PrivateTournament.Name);

                message.FromUserGUID = SessionProps.UserGuid;
                message.SentDate = DateTime.Now;

                var recipient = new Ext_MessageRecipient();
                recipient.MessageGUID = message.GUID;
                recipient.RecipientUserGUID = challenge.Ext_PrivateTournament.UserGUID;

                db.Ext_MessageRecipient.InsertOnSubmit(recipient);
                db.Ext_Message.InsertOnSubmit(message);

                lblPrivateTournaments.Text = String.Format("Du har tackat nej till utmaningen (ett meddelande har skickats till {0} {1}).", challenge.Ext_PrivateTournament.Sys_User.FirstName, challenge.Ext_PrivateTournament.Sys_User.LastName);

                db.SubmitChanges();

                LoadPrivateTournaments();
                
            }
        }

        private void AcceptChallenge(Guid tournamentGUID, Guid teamGUID)
        {
            using (var db = Global.GetConnection())
            {
                var challenge =
                    db.Ext_PrivateTournamentParticipant.Single(
                        ptp => ptp.PrivateTournamentGUID == tournamentGUID && ptp.TeamGUID == teamGUID);

                challenge.IsAccepted = true;

                lblPrivateTournaments.Text = "Du har accepterat utmaningen.";

                db.SubmitChanges();

                LoadPrivateTournaments();
            }
        }


        protected void lnkMessages_OnClick(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("UserMessages.aspx", new[] { "GUID" }, new[] { SessionProps.UserGuid.ToString() });
        }
    }
}