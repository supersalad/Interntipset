using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class TeamView : IntiPage
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
            if(!IsPostBack)
            {
                var selectedDayParam = this.GetRedirectParameter("tourDay", true);
                var selectedDay = 0;
                if (selectedDayParam != null)
                    selectedDay = int.Parse(selectedDayParam.ToString());

                daySelector.LoadTournamentDays(TeamId, selectedDay);

                btnToggleFavorite.Visible = (SessionProps.UserGuid != Guid.Empty);

                LoadTeam();

                
            }
        }

        private void LoadTeam()
        {
            if (TeamId != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);

                    lblHeader.Text = team.Name;
                    lblDescription.Text = team.Presentation;
                    if (!String.IsNullOrEmpty(team.Picture))
                    {
                        imgTeamImage.ImageUrl = "~/img/user/" + team.Picture;
                        pnlTeamImage.Visible = true;
                    }
                    
                    //visa lineup
                    var totalLineUp = from ta in db.Inti_TeamAthlete
                                 where ((ta.Inti_TeamVersion.ValidFrom < DateTime.Now
                                       && (ta.Inti_TeamVersion.ValidTo ?? DateTime.Now) >= DateTime.Now)
                                       || 
                                       ta.Inti_TeamVersion.ValidTo < DateTime.Now)
                                       && (ta.Inti_TeamVersion.Inti_Team.IsActive ?? false)
                                       && (ta.Inti_TeamVersion.Inti_Team.IsPaid ?? false)
                                       && ta.Inti_TeamVersion.Inti_Team.GUID == TeamId
                                 select new LineUp
                                            {
                                                GUID=ta.Inti_AthleteClub.GUID,
                                                Name=ta.Inti_AthleteClub.Inti_Athlete.FirstName + " " + ta.Inti_AthleteClub.Inti_Athlete.LastName,
                                                Club=ta.Inti_AthleteClub.Inti_Club.Name,
                                                Position=ta.Inti_AthleteClub.Inti_Position.Name,
                                                PositionShort = ta.Inti_AthleteClub.Inti_Position.ShortName,
                                                PositionSortOrder = ta.Inti_AthleteClub.Inti_Position.SortOrder,
                                                Price=ta.Inti_AthleteClub.Price,
                                                Points = (ta.Inti_TeamVersion.Inti_Team.Inti_TeamPointEvents.Where(tpe => tpe.Inti_MatchPointEvent.AthleteClubGUID == ta.AthleteGUID && (daySelector.SelectedDay == 0 || tpe.Inti_MatchPointEvent.Inti_Match.TourDay == daySelector.SelectedDay))
                                                    .Count() > 0 ? ta.Inti_TeamVersion.Inti_Team.Inti_TeamPointEvents.Where(tpe => tpe.Inti_MatchPointEvent.AthleteClubGUID == ta.AthleteGUID && (daySelector.SelectedDay == 0 || tpe.Inti_MatchPointEvent.Inti_Match.TourDay == daySelector.SelectedDay)).Sum(tpe => tpe.Inti_MatchPointEvent.Points) : 0),
                                                IsActive = ta.Inti_AthleteClub.IsActive,
                                                TradeOut = ta.Inti_TeamVersion.ValidTo
                                            };

                    //todo: remove duplicates                    
                    var currentLineUpAndTradeOuts = from a in totalLineUp
                                                    group a by
                                                        new
                                                            {
                                                                a.GUID,
                                                                a.Name,
                                                                a.Club,
                                                                a.Position,
                                                                a.PositionShort,
                                                                a.PositionSortOrder,
                                                                a.Price,
                                                                a.Points,
                                                                a.IsActive
                                                            }
                                                    into grouping
                                                        select new LineUp
                                                                   {
                                                                       GUID = grouping.Key.GUID,
                                                                       Name = grouping.Key.Name,
                                                                       Club = grouping.Key.Club,
                                                                       Position = grouping.Key.Position,
                                                                       PositionShort = grouping.Key.PositionShort,
                                                                       PositionSortOrder = grouping.Key.PositionSortOrder,
                                                                       Price = grouping.Key.Price,
                                                                       Points = grouping.Key.Points,
                                                                       IsActive = grouping.Key.IsActive,
                                                                       TradeOut = (grouping.Max(l => l.TradeOut ?? DateTime.MaxValue) == DateTime.MaxValue ? null : grouping.Max(l => l.TradeOut))
                                                                   };

                    var dataSource = currentLineUpAndTradeOuts
                                                .OrderBy(l => l.PositionSortOrder)
                                                .OrderBy(l=>l.TradeOut)
                                                .ToList();
                    foreach (var lineUp in dataSource.Where(lineUp => lineUp.TradeOut != null))
                    {
                        lineUp.PositionShort = "SUB";
                    }

                    //mark first and last in each group
                    for (int i = 0; i < dataSource.Count; i++)
                    {
                        if (i == 0 || dataSource[i-1].PositionShort != dataSource[i].PositionShort)
                            dataSource[i].FirstInGroup = true;

                        if (i == (dataSource.Count-1) || dataSource[i].PositionShort != dataSource[i + 1].PositionShort)
                            dataSource[i].LastInGroup = true;

                    }

                    
                    rptPlayers.DataSource = dataSource;
                    rptPlayers.DataBind();

                    var averagePrice = currentLineUpAndTradeOuts.Average(l => l.Price);
                    var activeTeamPriceSum = currentLineUpAndTradeOuts.Where(l => l.TradeOut == null).Sum(l => l.Price);

                    

                    lblTeamInfo.Text = String.Format("<table><tr><td>Lagkapten:</td><td>{0} {1}</td></tr>" +
                        "<tr><td>Spelarkostnad:</td><td>{2}</td></tr>" +
                        "<tr><td>Snittkostnad:</td><td>{3}</td></tr>" +
                        "{4}" +
                        "<tr><td>Totalt antal poäng:</td><td>{5}</td></tr></table>",
                        team.Sys_User.FirstName, team.Sys_User.LastName,
                        activeTeamPriceSum,
                        averagePrice,
                        (daySelector.SelectedDay == 0 ? String.Format("<tr><td>Bonuspoäng:</td><td>{0}</td></tr>", team.BonusPoints.ToString()) : ""),
                        (currentLineUpAndTradeOuts.Sum(l => l.Points) + (daySelector.SelectedDay == 0 ? team.BonusPoints : 0)).ToString());

                    //gjorda byten?
                    var teamTransfers = from tt in db.Inti_TeamTransfer
                                        where tt.TeamGUID == TeamId
                                              && tt.TransferDate < DateTime.Now
                                        select tt;

                    if (teamTransfers.ToList().Count > 0)
                    {
                        lblTeamTransfers.Text = "Gjorda byten:<br>";

                        foreach(var teamTransfer in teamTransfers.OrderByDescending(tt=>tt.TransferDate).ToList())
                        {
                            var outPlayer =
                                String.Format("{0} {1}", teamTransfer.AthleteOutGU.Inti_Athlete.FirstName,
                                              teamTransfer.AthleteOutGU.Inti_Athlete.LastName).Trim();
                            var inPlayer =
                                String.Format("{0} {1}", teamTransfer.Inti_AthleteClub.Inti_Athlete.FirstName,
                                              teamTransfer.Inti_AthleteClub.Inti_Athlete.LastName).Trim();

                            lblTeamTransfers.Text += String.Format("{0}: {1} ut, {2} in<br>",
                                                                   teamTransfer.TransferDate.Value.ToShortDateString(),
                                                                   outPlayer, inPlayer);
                                
                                
                        }
                    }

                    if (SessionProps.UserGuid != Guid.Empty)
                    {
                        var userFavoriteTeam = from uft in db.Ext_UserFavoriteTeam
                                               where uft.UserGUID == SessionProps.UserGuid && 
                                               uft.TeamGUID == TeamId
                                               select uft;

                        btnToggleFavorite.Text = (userFavoriteTeam.ToList().Count > 0) ? "Avmarkera laget" : "Markera laget";
                    }
                }

            }
            else
            {
                lblHeader.Text = "Kunde inte hitta laget";
            }
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
            public DateTime? TradeOut { get; set; }
            public bool FirstInGroup { get; set; }
            public bool LastInGroup { get; set; }

            public string CustomClass
            {
                get
                {
                    return TradeOut.HasValue ? "list-group-item-warning" : IsActive ?? true ? String.Empty
                        : "list-group-item-danger";
                }
            }
        }

        private Guid TeamId
        {
            get
            {
                var guid = this.GetRedirectParameter("teamGUID", false);
                if (guid != null)
                    return new Guid(guid.ToString());

                return Guid.Empty;
            }
        }

        
        
        
        protected void daySelector_SelectedDayChanged()
        {
            LoadTeam();
        }

        protected void btnToggleFavorite_Click(object sender, EventArgs e)
        {
            var trans = new UserTeamManagement(Global.ConnectionString, SessionProps);

            trans.ToggleUserFavoriteTeam(TeamId);

            LoadTeam();
        }

        protected Color GetAthleteForeColor(object eval)
        {
            var color = Color.Black;
            if (eval is DateTime)
            {
                if (((DateTime)eval).CompareTo(DateTime.Today) < 0)
                    color = Color.Gray;
            }

            return color;
        }
    }
}
