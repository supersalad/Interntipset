using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class Club : IntiPage
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
            var header = "Hittade inte laget";
            var clubId = this.GetRedirectParameter("id", false);

            if (!String.IsNullOrEmpty(clubId))
            {
                try
                {
                    var clubGuid = new Guid(clubId);
                    using (var db = Global.GetConnection())
                    {
                        //get club name
                        var club = db.Inti_Club.Single(c => c.GUID == clubGuid);
                        header = club.Name;

                        //load players
                        LoadPlayers(clubGuid, db);

                        //load matches
                        LoadMatches(clubGuid, db);

                        //load stakeholders
                        TeamBreakDown.LoadClubEvaluation(clubGuid, db);
                    }
                }
                catch (Exception)
                {
                    
                    //bad input or whatever, just let it slide
                }
            }

            lblHeader.Text = header;
        }

        private void LoadMatches(Guid clubGuid, IntiDataContext db)
        {
            var matches = from m in db.Inti_Match
                where m.AwayClub == clubGuid || m.HomeClub == clubGuid
                orderby m.MatchDate ?? DateTime.Now.AddYears(1)
                select new MatchDTO(m);

            rptMatches.DataSource = matches;
            rptMatches.DataBind();


        }

        private class MatchDTO
        {
            public MatchDTO(Inti_Match match)
            {
                GUID = match.GUID;
                MatchLabel = match.HomeClubInti_Club.ShortName + " - " + match.Inti_Club.ShortName;

                if (match.IsUpdated ?? false)
                    MatchLabel += " (" + match.HomeScore + "-" + match.AwayScore + ")";

                if (match.MatchDate.HasValue)
                    MatchLabel += " " + match.MatchDate.Value.ToShortDateString();
            }

            public Guid GUID { get; set; }
            public string MatchLabel { get; set; }
        }

        private void LoadPlayers(Guid clubGuid, IntiDataContext db)
        {
            //visa lineup
            var totalLineUp = from ac in db.Inti_AthleteClub
                              where ac.Inti_Club.GUID == clubGuid
                              && ((ac.IsActive ?? false) || ac.Inti_Position.ShortName == "MGR")
                              select new LineUp
                              {
                                  GUID = ac.GUID,
                                  Name = ac.Inti_Athlete.FirstName + " " + ac.Inti_Athlete.LastName,
                                  Club = ac.Inti_Club.Name,
                                  Position = ac.Inti_Position.Name,
                                  PositionShort = ac.Inti_Position.ShortName,
                                  PositionSortOrder = ac.Inti_Position.SortOrder,
                                  Price = ac.Price,
                                  Points = ac.Inti_MatchPointEvent.Any() ? ac.Inti_MatchPointEvent.Sum(p=>p.Points) : 0,
                                  IsActive = ac.IsActive
                              };


            var dataSource = totalLineUp.OrderBy(l => l.PositionSortOrder).ThenByDescending(l=>l.Points).ToList();
            
            //mark first and last in each group
            for (int i = 0; i < dataSource.Count; i++)
            {
                if (i == 0 || dataSource[i - 1].PositionShort != dataSource[i].PositionShort)
                    dataSource[i].FirstInGroup = true;

                if (i == (dataSource.Count - 1) || dataSource[i].PositionShort != dataSource[i + 1].PositionShort)
                    dataSource[i].LastInGroup = true;

            }

            rptPlayers.DataSource = dataSource;
            rptPlayers.DataBind();
        }


        private class LineUp
        {
            public Guid GUID { get; set; }
            public string Name { get; set; }
            public string Club { get; set; }
            public string Position { get; set; }
            public string PositionShort { get; set; }
            public int PositionSortOrder { get; set; }
            public int? Price { get; set; }
            public int? Points { get; set; }
            public bool? IsActive { get; set; }
            public bool FirstInGroup { get; set; }
            public bool LastInGroup { get; set; }

            public string CustomClass
            {
                get
                {
                    return IsActive ?? true ? String.Empty
                        : "list-group-item-danger";
                }
            }
        }
    }
}