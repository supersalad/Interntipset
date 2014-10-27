using System;
using System.Collections;
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
    public partial class PlayerView : IntiPage
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
            
            if (!IsPostBack)
                LoadPlayer();
        }

        
        private void LoadPlayer()
        {
            var acGUID = this.GetRedirectParameter("acGUID", false);
            if (acGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    var athleteClub = db.Inti_AthleteClub.Single(ac => ac.GUID == new Guid(acGUID.ToString()));

                    lblPlayerName.Text =
                        String.Format("{0} {1}", athleteClub.Inti_Athlete.FirstName, athleteClub.Inti_Athlete.LastName).Trim();
                    lblClub.Text = String.Format("<a href='/Club/{0}'>{1}</a>", athleteClub.Inti_Club.GUID, athleteClub.Inti_Club.Name);
                    if (!(athleteClub.IsActive ?? false))
                    {
                        lblClub.Text += " sparkad";
                    }
                    lblPos.Text = athleteClub.Inti_Position.Name;
                    lblPrice.Text = athleteClub.Price.ToString();

                    var points = from mpe in db.Inti_MatchPointEvent
                                 where mpe.AthleteClubGUID == athleteClub.GUID
                                 orderby mpe.Inti_PointEvent.Name, mpe.Inti_Match.MatchDate
                                 select mpe;

                    var currentPointEventGUID = Guid.Empty;
                    var currentPointString = "";
                    var currentHeader = "";
                    int sumPoints = 0;
                    int totalPoints = 0;
                    foreach(var mpe in points)
                    {
                        if (mpe.PointEventGUID != currentPointEventGUID)
                        {
                            //new type of point event
                            if (currentPointEventGUID != Guid.Empty)
                            {
                                //sum up this point event type
                                lblPoints.Text += currentHeader + currentPointString + "</div>";
                            }

                            //reset values
                            sumPoints = 0;
                            currentPointString = "";
                        }

                        sumPoints += mpe.Points;
                        totalPoints += mpe.Points;
                        currentHeader = GetPointEventHeader(mpe.Inti_PointEvent.Name, sumPoints);

                        currentPointString += GetPointsString(mpe);

                        currentPointEventGUID = mpe.PointEventGUID;
                    }

                    if (currentPointEventGUID != Guid.Empty)
                    {
                        lblPoints.Text += currentHeader + currentPointString + "</div>";
                    }

                    lblPoints.Text = "<p>Totalt: " + totalPoints + " poäng</p>" + lblPoints.Text;

                    //interntips lag
                    if (SessionProps.SelectedTournament.StartRegistration < DateTime.Now)
                    {
                        var userFavorites = from utf in db.Ext_UserFavoriteTeam
                                            where utf.UserGUID == SessionProps.UserGuid
                                            select utf;

                        var teams = from ta in db.Inti_TeamAthlete
                                    where (ta.Inti_TeamVersion.Inti_Team.IsActive ?? false)
                                          && (ta.Inti_TeamVersion.Inti_Team.IsPaid ?? false)
                                          && ta.Inti_TeamVersion.ValidFrom < DateTime.Now &&
                                          (ta.Inti_TeamVersion.ValidTo ?? DateTime.Now) >= DateTime.Now
                                          && ta.AthleteGUID == athleteClub.GUID
                                    select new TeamReturnClass
                                               {
                                                   TeamGUID = ta.Inti_TeamVersion.Inti_Team.GUID,
                                                   TeamName = ta.Inti_TeamVersion.Inti_Team.Name,
                                                   TeamManager =
                                        ta.Inti_TeamVersion.Inti_Team.Sys_User.FirstName + " " +
                                        ta.Inti_TeamVersion.Inti_Team.Sys_User.LastName,
                                                    CustomClass = ""
                                               };

                        //loop through user favorites to set the CustomClass if any
                        var theList = teams.ToList();
                        foreach (var extUserFavoriteTeam in userFavorites.ToList())
                        {
                            var team = theList.SingleOrDefault(t => t.TeamGUID == extUserFavoriteTeam.TeamGUID);
                            if (team == null) continue;
                            team.CustomClass = "active";
                        }

                        rptIntiTeams.DataSource = theList;
                        rptIntiTeams.DataBind();

                        //grdIntiTeams.DataKeyNames = new string[]{"TeamGUID"};
                        //grdIntiTeams.DataSource = teams.ToList();
                        //grdIntiTeams.DataBind();

                    }
                    

                }    
            }
            
        }

        private string GetPointEventHeader(string pointEventName, int points)
        {
            return String.Format("<div class='panel panel-default'><div class='panel-heading'>{0} ({1})</div>", pointEventName, points);

        }

        private string GetPointsString(Inti_MatchPointEvent mpe)
        {
            return String.Format("<div class='panel-body'><a href='/Match/{0}'>{1} - {2}, {3}</a></div>", 
                                 mpe.Inti_Match.GUID,
                                 mpe.Inti_Match.HomeClubInti_Club.ShortName,
                                 mpe.Inti_Match.Inti_Club.ShortName,
                                 mpe.Inti_Match.MatchDate.HasValue
                                     ? mpe.Inti_Match.MatchDate.Value.ToShortDateString()
                                     : ""
                                     );
        }

    }

    internal class TeamReturnClass
    {
        public Guid TeamGUID { get; set; }
        public string TeamName { get; set; }
        public string TeamManager { get; set; }
        public string CustomClass { get; set; }
    }
}
