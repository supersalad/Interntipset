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
    public partial class MyTournamentEdit : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnCancel.Attributes.Add("onclick", "window.history.go(-1)");

            if (!IsPostBack)
            {
                
                //init day selectors
                for (var i = 1; i < 39; i++ )
                {
                    drpStartDay.Items.Add(i.ToString());
                    drpEndDay.Items.Add(i.ToString());
                }

                LoadTeamsToChallenge();

                LoadTournament();


            }
        }

        private void LoadTeamsToChallenge()
        {
            //load all teams in the tournament
            using (var db = Global.GetConnection())
            {
                var teams = from t in db.Inti_Team
                            where t.TournamentGUID == SessionProps.SelectedTournament.GUID
                                  && (t.IsPaid ?? false)
                            select new
                                       {
                                           t.GUID,
                                           Name = t.Sys_User.FirstName + " " + t.Sys_User.LastName + "(" + t.Name + ")"
                                       };

                drpTeamsToChallenge.DataValueField = "GUID";
                drpTeamsToChallenge.DataTextField = "Name";
                drpTeamsToChallenge.DataSource = teams.OrderBy(t => t.Name).ToList();
                drpTeamsToChallenge.DataBind();
                
            }
        }

        private void LoadTournament()
        {
            var tournamentGUID = this.GetRedirectParameter("tournamentGUID", false);
            if(tournamentGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    var tournament = db.Ext_PrivateTournament.Single(t => t.GUID == new Guid(tournamentGUID.ToString()));

                    //verify tournament owner
                    if (tournament.Sys_User.GUID != SessionProps.UserGuid && !SessionProps.HasPermission("ADMIN"))
                    {
                        //log the attempted breach
                        MailAndLog.SendMessage("Försök att sabba turnering",
                            String.Format("Användaren: {0} med guid: {1} försökte öppna turneringen: {2} med guid: {3}", SessionProps.UserName, SessionProps.UserGuid.ToString(), tournament.Name, tournament.GUID),
                            Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                        throw new AccessViolationException("Attempt to open other users tournament");
                    }

                    Name.Text = tournament.Name;
                    Description.Text = tournament.Description;
                    IsVisibleForAll.Checked = (tournament.IsLimitedInTime ?? false);

                    if (tournament.IsLimitedInTime ?? true)
                    {
                        rblDateLimitation.SelectedValue = "datum";
                        pnlDateLimitation.Visible = true;
                        pnlDayLimitation.Visible = false;
                        StartDate.Text = (tournament.StartDate ?? DateTime.Now).ToShortDateString();
                        EndDate.Text = (tournament.EndDate ?? DateTime.Now).ToShortDateString();
                    }
                    else
                    {
                        rblDateLimitation.SelectedValue = "omg";
                        pnlDateLimitation.Visible = false;
                        pnlDayLimitation.Visible = true;

                        drpStartDay.SelectedIndex = (tournament.StartDay ?? 2) - 1;
                        drpEndDay.SelectedIndex = (tournament.EndDay ?? 2) - 1;
                    }

                    LoadParticipants(tournament.GUID, db);


                }
            }
        }

        private void LoadParticipants(Guid tournamentGUID, IntiDataContext db)
        {

            //load participants
            pnlAddParticipants.Visible = true;

            var participants = from p in db.Ext_PrivateTournamentParticipant
                               where p.PrivateTournamentGUID == tournamentGUID
                               select new
                               {
                                   GUID = p.Inti_Team.GUID,
                                   TeamName =
                        p.Inti_Team.Sys_User.FirstName + " " + p.Inti_Team.Sys_User.LastName + "(" +
                        p.Inti_Team.Name + ")",
                                   p.IsAccepted
                               };


            grdChallengedTeams.DataSource = participants.OrderBy(t => t.TeamName).ToList();
            grdChallengedTeams.DataBind();
        }

        protected void rblDateLimitation_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlDateLimitation.Visible = (rblDateLimitation.SelectedValue == "datum");
            pnlDayLimitation.Visible = (rblDateLimitation.SelectedValue == "omg");

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //validering

            //måste ha ett namn
            if(Name.Text.Trim().Length == 0)
            {
                lblMessage.Text = "Din utmaning måste ha ett namn";
                return;
            }

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            //om datumavgränsning, måste vara giltiga datum
            if(rblDateLimitation.SelectedValue == "datum")
            {
                
                if (!DateTime.TryParse(StartDate.Text, out startDate))
                {
                    lblMessage.Text = "Startdatum måste vara ett giltigt datum";
                    return;
                }

                if (!DateTime.TryParse(EndDate.Text, out endDate))
                {
                    lblMessage.Text = "Slutdatum måste vara ett giltigt datum";
                    return;
                }

                if(startDate >= endDate)
                {
                    lblMessage.Text = "Startdatum måste vara före slutdatum";
                    return;
                }

            }


            //insert or update?
            var tournamentGUID = this.GetRedirectParameter("tournamentGUID", false);
            if (tournamentGUID == null)
            {
                //insert
                using(var db=Global.GetConnection())
                {
                    var myTournament = new Ext_PrivateTournament();
                    myTournament.UserGUID = SessionProps.UserGuid;
                    myTournament.TournamentGUID = SessionProps.SelectedTournament.GUID;
                    myTournament.Name = Name.Text;
                    myTournament.Description = Description.Text;
                    myTournament.IsVisibleForAll = IsVisibleForAll.Checked;
                    myTournament.IsOpenForAll = false;

                    if(rblDateLimitation.SelectedValue == "datum")
                    {
                        myTournament.IsLimitedInTime = true;
                        myTournament.StartDate = startDate;
                        myTournament.EndDate = endDate;
                    }
                    else
                    {
                        myTournament.IsLimitedInTime = false;
                        myTournament.StartDay = drpStartDay.SelectedIndex + 1;
                        myTournament.EndDay = drpEndDay.SelectedIndex + 1;
                    }

                    db.Ext_PrivateTournament.InsertOnSubmit(myTournament);

                    db.SubmitChanges();

                    WebControlManager.RedirectWithQueryString("MyTournamentEdit.aspx", new string[]{"tournamentGUID"}, new string[]{myTournament.GUID.ToString()} );
                }
            }
            else
            {
                using (var db = Global.GetConnection())
                {
                    var myTournament = db.Ext_PrivateTournament.Single(t => t.GUID == new Guid(tournamentGUID.ToString()));

                    myTournament.Name = Name.Text;
                    myTournament.Description = Description.Text;
                    myTournament.IsVisibleForAll = IsVisibleForAll.Checked;
                    
                    if (rblDateLimitation.SelectedValue == "datum")
                    {
                        myTournament.IsLimitedInTime = true;
                        myTournament.StartDate = startDate;
                        myTournament.EndDate = endDate;
                    }
                    else
                    {
                        myTournament.IsLimitedInTime = false;
                        myTournament.StartDay = drpStartDay.SelectedIndex + 1;
                        myTournament.EndDay = drpEndDay.SelectedIndex + 1;
                    }

                    db.SubmitChanges();

                }
            }

        }

        protected void btnChallenge_Click(object sender, EventArgs e)
        {
            var tournamentGUID = this.GetRedirectParameter("tournamentGUID", false);
            if (tournamentGUID != null)
            {
                using(var db = Global.GetConnection())
                {
                    //check so this participant isn't added already
                    var participantcheck = from p in db.Ext_PrivateTournamentParticipant
                                           where p.PrivateTournamentGUID == new Guid(tournamentGUID.ToString())
                                                 && p.TeamGUID == new Guid(drpTeamsToChallenge.SelectedValue)
                                           select p;

                    if (participantcheck.ToList().Count > 0)
                    {
                        lblChallengeMessage.Text = "Det laget är redan utmanat";
                        return;
                    }

                    var participant = new Ext_PrivateTournamentParticipant();
                    participant.TeamGUID = new Guid(drpTeamsToChallenge.SelectedValue);
                    participant.PrivateTournamentGUID = new Guid(tournamentGUID.ToString());
                    participant.IsAccepted = false;

                    db.Ext_PrivateTournamentParticipant.InsertOnSubmit(participant);

                    db.SubmitChanges();

                    LoadParticipants(new Guid(tournamentGUID.ToString()),db );
                }
            }
        }
    }
}
