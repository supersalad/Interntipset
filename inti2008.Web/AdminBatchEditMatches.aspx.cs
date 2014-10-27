using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminBatchEditMatches : IntiPage
    {
        private const string HtmlLineBreak = "<br>";

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN");

            using (var db = Global.GetConnection())
            {
                var tournament = db.Inti_Tournament.Single(t => t.GUID == TourId);
                tournamentName.Text = tournament.Name;
            }    
        }

        protected Guid TourId
        {
            get
            {
                var value = this.GetRedirectParameter("GUID", false);
                if (value == null)
                    return Guid.Empty;

                return new Guid(value.ToString());
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //get clubs
            using (var db = Global.GetConnection())
            {
                var clubs = db.Inti_Club.Where(c => c.TournamentGUID == TourId).ToList();

                var day = int.Parse(drpDay.SelectedValue);
                var matchDate = DateTime.Parse(txtDate.Text);
                var matches = txtMatches.Text;

                var rejectedEntries = String.Empty;
                var approvedEntries = String.Empty;

                var matchArray = matches.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var match in matchArray)
                {
                    var matchClubs = match.Split(new string[] {"-", " "}, StringSplitOptions.RemoveEmptyEntries);
                    //måste vara ett hemma och ett bortalag
                    if (matchClubs.GetUpperBound(0) != 1)
                    {
                        rejectedEntries += HtmlLineBreak + match + " (fel antal lag)";
                        continue;
                    }

                    var homeTeamShort = matchClubs[0].Trim().ToUpper();
                    var awayTeamShort = matchClubs[1].Trim().ToUpper();

                    if (homeTeamShort == awayTeamShort)
                    {
                        rejectedEntries += HtmlLineBreak + match + " (hemmalag får inte vara samma som bortalag)";
                        continue;
                    }

                    //hemmalag måste finnas
                    var homeClub = clubs.SingleOrDefault(c => c.ShortName == homeTeamShort);
                    if (homeClub == null)
                    {
                        rejectedEntries += HtmlLineBreak + match + " (hittar inte hemmalaget)";
                        continue;
                    }

                    //bortalag måste finnas
                    var awayClub = clubs.SingleOrDefault(c => c.ShortName == awayTeamShort);
                    if (awayClub == null)
                    {
                        rejectedEntries += HtmlLineBreak + match + " (hittar inte bortalaget)";
                        continue;
                    }

                    //finns matchen?
                    var dbMatch =
                        db.Inti_Match.SingleOrDefault(
                            m =>
                            m.TournamentGUID == TourId && m.HomeClub == homeClub.GUID && m.AwayClub == awayClub.GUID);

                    approvedEntries += HtmlLineBreak + match;
                    var isNew = false;
                    if (dbMatch == null)
                    {
                        isNew = true;
                        //create new match
                        dbMatch = new Inti_Match();
                        dbMatch.HomeClub = homeClub.GUID;
                        dbMatch.AwayClub = awayClub.GUID;
                        dbMatch.TournamentGUID = TourId;

                        approvedEntries += " (la till ny match)";
                    }
                    else
                    {
                        approvedEntries += " (ändrade befintlig match)";
                    }

                    dbMatch.TourDay = day;
                    dbMatch.MatchDate = matchDate;
                    if (isNew)
                        db.Inti_Match.InsertOnSubmit(dbMatch);

                    db.SubmitChanges();
                }

                lblOutput.Text = (approvedEntries + HtmlLineBreak + rejectedEntries);
            } 

        }


    }
}