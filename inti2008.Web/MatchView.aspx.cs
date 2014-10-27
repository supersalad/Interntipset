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
using Newtonsoft.Json;

namespace inti2008.Web
{
    public partial class MatchView : IntiPage
    {
        public override bool AuthenticationIsRequired
        {
            get
            {
                return false;
            }
        }

        public string MatchSummary { get; set; }
        public string MatchHeader { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var header = "Hittade inte matchen";
            MatchSummary = header;
            lblHeader.Text = header;
            var matchId = this.GetRedirectParameter("id", false);
            if (!String.IsNullOrEmpty(matchId))
            {
                try
                {
                    var matchGuid = new Guid(matchId);
                    using (var db = Global.GetConnection())
                    {
                        var match = db.Inti_Match.Single(m => m.GUID == matchGuid);

                        header = match.HomeClubInti_Club.Name + " - " + match.Inti_Club.Name;
                        MatchSummary = header;
                        MatchHeader = header;
                        lblHeader.Text = String.Format("<a href='\\Club\\{0}'>{1}</a> - <a href='\\Club\\{2}'>{3}</a>", match.HomeClubInti_Club.GUID, match.HomeClubInti_Club.Name, match.Inti_Club.GUID, match.Inti_Club.Name);
                        if (match.IsUpdated ?? false)
                        {
                            lblResult.Text = (match.HomeScore ?? 0) + "-" + (match.AwayScore ?? 0);

                            MatchSummary += " " + lblResult.Text;

                            lblPointEvents.Text = GetPointEventText(match);

                            //lblMatchFactsTest.Text = new MatchManagement(Global.ConnectionString, SessionProps).GetRandomMatchFact(matchGuid);

                            if (match.Updater.HasValue)
                            {
                                var user = db.Sys_User.Single(u => u.GUID == match.Updater.Value);
                                lblUpdater.Text = "Uppdaterad av: " +  String.Format("{0} {1}", user.FirstName, user.LastName);
                            }



                            //show standing
                            ucStanding.LoadMatch(match.GUID);

                            //hide stakeholders
                            ucTeamBreakDown.Visible = false;
                        }
                        else
                        {
                            lblResult.Text = match.MatchDate.HasValue &&
                                             match.MatchDate.Value.CompareTo(DateTime.Today) > 0
                                ? "Ej spelad"
                                : "Ej uppdaterad";

                            //hide standing
                            ucStanding.Visible = false;

                            //show stakeholders
                            ucTeamBreakDown.LoadPreMatchEvaluation(match, db);
                        }
                        lblMatchDate.Text = match.MatchDate.HasValue
                            ? match.MatchDate.Value.ToShortDateString()
                            : "Inget datum satt för matchen.";

                        
                        
                        //LoadStanding(match);
                    }
                    
                }
                catch (Exception)
                {

                    //do nothing - bad input data - can't find news item
                }

            }

            
        }


        private IList<Ext_UserFavoriteTeam> userFavoriteTeam = null;

        private string GetPointEventText(Inti_Match match)
        {
            var goalScorers = new List<string>(4);
            var redCarders = new List<string>(4);
            var ownGoalers = new List<string>(4);
            var pointEventsText = "<ul>";
            foreach (var matchPointEvent in match.Inti_MatchPointEvent.OrderBy(mpe => mpe.Inti_PointEvent.Name).ThenBy(mpe => mpe.Inti_AthleteClub.AthleteGUID).ThenBy(mpe => mpe.Inti_AthleteClub.Inti_Club.Name).ToList())
            {
                //get athletes name
                var athleteName =
                    String.Format("{0} {1}", matchPointEvent.Inti_AthleteClub.Inti_Athlete.FirstName,
                        matchPointEvent.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                
                pointEventsText += String.Format("<li>{0} ({1}), {2}</li>",
                                                                  String.Format("<a href='/Player/{0}'>{1}</a>",
                                                                                matchPointEvent.Inti_AthleteClub.GUID, athleteName),
                                                                  matchPointEvent.Inti_AthleteClub.Inti_Club.ShortName,
                                                                  matchPointEvent.Inti_PointEvent.Name);
                //get info for match summary
                switch (matchPointEvent.Inti_PointEvent.Name.ToLower())
                {
                    case "mål":
                    case "straffmål":
                        if (!goalScorers.Contains(athleteName)) goalScorers.Add(athleteName);
                        break;
                    case "rött kort":
                        if (!redCarders.Contains(athleteName)) redCarders.Add(athleteName);
                        break;
                    case "självmål":
                        if (!ownGoalers.Contains(athleteName)) ownGoalers.Add(athleteName);
                        break;
                }

                
            }

            //while we're at it - set the goal scorers etc in the match summary
            if (goalScorers.Count > 0)
            {
                MatchSummary += ConcatinateAthletes(goalScorers, "Målgörare");
            }
            if (redCarders.Count > 0)
            {
                MatchSummary += ConcatinateAthletes(redCarders, "Utvisade");
            }
            if (ownGoalers.Count > 0)
            {
                MatchSummary += ConcatinateAthletes(ownGoalers, "Självmål");
            }

            pointEventsText += "</ul>";
            return pointEventsText;
        }

        private string ConcatinateAthletes(List<string> athletes, string header)
        {
            var returnValue = "\n" + header + ":";
            var first = true;
            foreach (var athlete in athletes)
            {
                if (!first)
                {
                    returnValue += ",";
                }
                first = false;
                returnValue += " " + athlete;
            }

            return returnValue;
        }

        
    }
}
