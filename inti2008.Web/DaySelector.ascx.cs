using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class DaySelector : System.Web.UI.UserControl
    {
        public delegate void SelectedDayChangedHandler();

        public event SelectedDayChangedHandler SelectedDayChanged;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadTournamentDays()
        {
            //Load tournament days
            LoadTournamentDays(Guid.Empty, 0);
        }

        public void LoadTournamentDays(Guid teamGUID, int preSelectedDay)
        {
            //store team
            if (teamGUID != Guid.Empty)
                ViewState["TeamGUID"] = teamGUID;

            //Load tournament days
            drpTournamentDay.Items.Add(new ListItem("--total--", "0"));
            for (var i = 1; i <= SessionProperties.SelectedTournament.NmbrOfDays; i++)
            {
                drpTournamentDay.Items.Add(new ListItem(String.Format("Omgång {0}", i.ToString()), i.ToString()));
            }

            if (preSelectedDay > 0 && drpTournamentDay.Items.FindByValue(preSelectedDay.ToString()) != null)
            {
                drpTournamentDay.ClearSelection();
                drpTournamentDay.Items.FindByValue(preSelectedDay.ToString()).Selected = true;

                LoadMatchPointEventsForSelectedDay();
            }
                
        }

        private Guid TeamGUID
        {
            get
            {
                if (ViewState["TeamGUID"] != null && (ViewState["TeamGUID"] is Guid))
                    return (Guid)ViewState["TeamGUID"];

                return Guid.Empty;
            }
        }

        private SessionProperties SessionProperties
        {
            get
            {
                if (Page is IntiPage)
                    return (Page as IntiPage).SessionProps;

                throw new InvalidOperationException("DaySelector run on a non IntiPage page");
            }
        }

        public int SelectedDay
        {
            get
            {
                if (drpTournamentDay.Items.Count > 0)
                    return int.Parse(drpTournamentDay.SelectedValue);
                
                return 0;
            }
        }

        protected void drpTournamentDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedDayChanged != null)
                SelectedDayChanged();


            LoadMatchPointEventsForSelectedDay();
        }

        private void LoadMatchPointEventsForSelectedDay()
        {
            //visa matcher som ska uppdateras inom det närmaste eller som har uppdaterats 
            lblMatchUpdates.Text = "";
            if (SelectedDay != 0)
            {
                using (var db = Global.GetConnection())
                {
                    

                    var matches = from m in db.Inti_Match
                                  where m.TournamentGUID == SessionProperties.SelectedTournament.GUID
                                        && (m.TourDay == SelectedDay)
                                  select m;

                    var usersQ = from u in db.Sys_User
                                select u;


                    

                    foreach (var match in matches.OrderBy(m => m.IsUpdated).OrderByDescending(m => m.MatchDate))
                    {
                        //updatername?
                        var updaterName = "";
                        if (match.Updater != null)
                        {
                            Inti_Match intiMatch = match;
                            var user = usersQ.Single(u => u.GUID == intiMatch.Updater);
                            updaterName = String.Format("{0} {1}", user.FirstName, user.LastName).Trim();
                        }
                        

                        lblMatchUpdates.Text += String.Format("<a href='\\MatchView.aspx?id={4}'><b>{0} - {1}</b></a><br>({2}, {3})<br><ul>",
                                                              match.HomeClubInti_Club.Name, match.Inti_Club.Name,
                                                              match.MatchDate.HasValue
                                                                  ? match.MatchDate.Value.ToShortDateString()
                                                                  : "-",
                                                              (match.IsUpdated ?? false)
                                                                  ? String.Format("{0} - {1}, Uppdaterad av: {2}", match.HomeScore ?? 0, match.AwayScore ?? 0,updaterName)
                                                                  : "Ej uppdaterad",
                                                                  match.GUID);

                        foreach (var matchPointEvent in match.Inti_MatchPointEvent.OrderBy(mpe => mpe.Inti_PointEvent.Name).OrderBy(mpe => mpe.Inti_AthleteClub.AthleteGUID).OrderBy(mpe => mpe.Inti_AthleteClub.Inti_Club.Name).ToList())
                        {
                            var isPointForTeam = (TeamGUID != Guid.Empty &&
                                                  matchPointEvent.Inti_TeamPointEvents.Where(
                                                      tpe => tpe.TeamGUID == TeamGUID).ToList().Count > 0);

                    
                            lblMatchUpdates.Text += String.Format("<li {0}>{1} ({2}), {3}</li>",
                                isPointForTeam ? String.Format("class=\"highlight\"") : "",
                                                                  String.Format("<a href='\\PlayerView.aspx?acGUID={2}'>{0} {1}</a>",
                                                                                matchPointEvent.Inti_AthleteClub.
                                                                                    Inti_Athlete.FirstName,
                                                                                matchPointEvent.Inti_AthleteClub.
                                                                                    Inti_Athlete.LastName,matchPointEvent.Inti_AthleteClub.GUID).Trim(),
                                                                  matchPointEvent.Inti_AthleteClub.Inti_Club.ShortName,
                                                                  matchPointEvent.Inti_PointEvent.Name);
                        }
                        lblMatchUpdates.Text += "</ul><br>";
                    }
                }
            }
        }
    }
}