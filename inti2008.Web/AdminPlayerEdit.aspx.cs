using System;
using System.Collections.Generic;
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
    public partial class AdminPlayerEdit : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN", "USER_ATHLETEUPDATE");

            if (!IsPostBack)
            {
                LoadTournaments();

                LoadPlayer();    
            }
            
        }

        private void LoadPlayer()
        {
            var playerGUID = this.GetRedirectParameter("athleteGUID", false);
            if (playerGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    try
                    {
                        var player = db.Inti_Athlete.Single(a => a.GUID == new Guid(playerGUID.ToString()));

                        FirstName.Text = player.FirstName;
                        LastName.Text = player.LastName;
                    }
                    catch (Exception exception)
                    {
                        
                        PlayerEditMessage.Text = exception.GetType().Name + "<BR>" + exception.Message;
                    }

                }
            }
        }

        private void LoadTournaments()
        {
            using (var db = Global.GetConnection())
            {
                var tournaments = (from t in db.Inti_Tournament
                                   select t).OrderByDescending(t => t.EndRegistration);

                drpTournament.DataValueField = "GUID";
                drpTournament.DataTextField = "Name";
                drpTournament.DataSource = tournaments.ToList();
                drpTournament.DataBind();

                drpTournament.SelectedValue = SessionProps.SelectedTournament.GUID.ToString();

                LoadTournamentDetails();
            }
        }

        private void LoadTournamentDetails()
        {

            //get clubs in this tournament
            using (var db = Global.GetConnection())
            {
                var tournament = db.Inti_Tournament.Single(t => t.GUID == new Guid(drpTournament.SelectedValue));

                lblTournament.Text = tournament.Name;

                //get clubs
                var clubs = from c in db.Inti_Club
                            where c.TournamentGUID == new Guid(drpTournament.SelectedValue)
                            select c;

                drpClubs.DataValueField = "GUID";
                drpClubs.DataTextField = "ShortName";
                drpClubs.DataSource = clubs.OrderBy(c=>c.ShortName).ToList();
                drpClubs.DataBind();
                var defaultClubItem = new ListItem("---");
                drpClubs.Items.Add(defaultClubItem);
                drpClubs.SelectedIndex = drpClubs.Items.IndexOf(defaultClubItem);

                //get positions
                var positions = from p in db.Inti_Position
                                where p.ShortName != "MGR" || tournament.IncludeManager
                                select p;

                drpPosition.DataValueField = "GUID";
                drpPosition.DataTextField = "Name";
                drpPosition.DataSource = positions.ToList();
                drpPosition.DataBind();
                var defaultPositionItem = new ListItem("---");
                drpPosition.Items.Add(defaultPositionItem);
                drpPosition.SelectedIndex = drpPosition.Items.IndexOf(defaultPositionItem);

                //get players settings
                var playerGUID = this.GetRedirectParameter("athleteGUID", false);
                if (playerGUID != null)
                {
                    var athleteClubs = from ac in db.Inti_AthleteClub
                                      where ac.AthleteGUID == new Guid(playerGUID.ToString()) &&
                                        ac.Inti_Club.TournamentGUID == new Guid(drpTournament.SelectedValue)
                                      select ac;

                    if (athleteClubs.Count() == 1)
                    {
                        var athleteClub = athleteClubs.ToList()[0];
                        AthleteClubGUID.Text = athleteClub.GUID.ToString();

                        drpClubs.SelectedValue = athleteClub.Inti_Club.GUID.ToString();

                        drpPosition.SelectedValue = athleteClub.Inti_Position.GUID.ToString();

                        Price.Text = athleteClub.Price.ToString();

                        IsActive.Checked = athleteClub.IsActive ?? true;
                    }
                    else
                    {
                        //todo: introducera ej valda värden i drop-boxarna
                        AthleteClubGUID.Text = "";
                        Price.Text = "";
                        IsActive.Checked = false;
                    }
                }

            }

            

        }

        
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("AdminPlayerList.aspx", new string[]{"dummy"}, new string[] {"dummy"});
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var athleteManagement = new AthleteManagement(Global.ConnectionString, SessionProps);

            var playerGUIDvar = this.GetRedirectParameter("athleteGUID", false);
            Guid playerGUID = Guid.Empty;
            Guid newPlayerGUID = Guid.Empty;
            var newPlayer = playerGUIDvar == null;

            if(playerGUIDvar != null)
            {
                playerGUID = new Guid(playerGUIDvar.ToString());
            }

            try
            {
                Action<string, Guid> TweetAthlete = (tweet, athleteClubGuid) =>
                                                    {
                                                        var url = "http://interntipset.com/Player/" + athleteClubGuid.ToString();
                                                        Global.SendTweet(tweet, url, SessionProps);                                     
                                                    };

                newPlayerGUID = athleteManagement.SaveAthlete(FirstName.Text, LastName.Text, playerGUID, new Guid(drpTournament.SelectedValue),
                        (drpClubs.SelectedItem.Text == "---" ? Guid.Empty : new Guid(drpClubs.SelectedValue)),
                        (drpPosition.SelectedItem.Text == "---" ? Guid.Empty : new Guid(drpPosition.SelectedValue)),
                        int.Parse(Price.Text), IsActive.Checked, drpClubs.SelectedItem.Text, TweetAthlete);
                

            }
            catch (DuplicateNameException duplicateNameException)
            {
                PlayerEditMessage.Text =
                    "Det finns redan en spelare med detta namnet. Gå tillbaka till söksidan och editera den spelaren om det är samma person. Gör annars namnet unikt.";
            }
            catch(Exception exception)
            {
                PlayerEditMessage.Text = exception.Message;
            }

            if (newPlayerGUID != playerGUID)
                WebControlManager.RedirectWithQueryString("AdminPlayerEdit.aspx", new string[] { "athleteGUID" }, new string[] { newPlayerGUID.ToString() });
            
        }

        private bool AthleteHasNameDuplicate(IntiDataContext db, Guid athleteGUID)
        {
            var athleteTest = from a in db.Inti_Athlete
                              where a.FirstName.ToLower().Trim() == FirstName.Text.ToLower().Trim()
                                    && a.LastName.ToLower().Trim() == LastName.Text.ToLower().Trim()
                                    && (a.GUID != athleteGUID || athleteGUID == Guid.Empty)
                              select a;

            if(athleteTest.ToList().Count > 0)
            {
                

                return true;
            }


            PlayerEditMessage.Text = "";
            return false;
        }

        protected void drpTournament_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpTournament.SelectedValue = drpTournament.Items[drpTournament.SelectedIndex].Value;
            LoadTournamentDetails();
        }
    }
}
