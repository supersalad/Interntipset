using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;
using Newtonsoft.Json;

namespace inti2008.Web
{
    public partial class TeamBreakDown : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private SessionProperties SessionProps
        {
            get
            {
                if (Page is IntiPage)
                    return (Page as IntiPage).SessionProps;

                throw new IntiGeneralException("TeamBreakDown on a non IntiPage page");
            }
        }

        public string PreviewData { get; set; }

        public void LoadPreMatchEvaluation(Inti_Match match, IntiDataContext db)
        {
            //get a list of athletes represented in teams, sorted by total scores
            var highestScoringHomeAthletes = GetAthletesForClub(match.HomeClub, db);
            var highestScoringAwayAthletes = GetAthletesForClub(match.AwayClub, db);

            LoadStakeholders(highestScoringHomeAthletes.Union(highestScoringAwayAthletes).ToList(), db);

            lblHeader.Text = "Lag med intressen";
        }

        public void LoadClubEvaluation(Guid clubGuid, IntiDataContext db)
        {
            var athletes = GetAthletesForClub(clubGuid, db);

            LoadStakeholders(athletes.ToList(), db);

            lblHeader.Text = "Lag med intressen";
        }


        private void LoadStakeholders(IList<MatchPreviewAthleteDTO> athleteDtos, IntiDataContext db)
        {


            var userFavorites = from utf in db.Ext_UserFavoriteTeam
                                where utf.UserGUID == SessionProps.UserGuid
                                select utf;

            //for each of these players, get a reference to the teams that has selected them
            var teams = new Dictionary<Guid, MatchPreviewTeamDTO>(10);
            foreach (var athlete in athleteDtos)
            {
                var teamVersions = (from ta in db.Inti_TeamAthlete
                                    join u in userFavorites
                                    on ta.Inti_TeamVersion.Inti_Team equals u.Inti_Team into oj
                                    from utf in oj.DefaultIfEmpty()
                                    where
                                        (ta.Inti_TeamVersion.Inti_Team.IsPaid ?? false) &&
                                        (ta.Inti_TeamVersion.Inti_Team.IsActive ?? false)
                                        && ta.Inti_TeamVersion.ValidFrom <= DateTime.Today &&
                                        (ta.Inti_TeamVersion.ValidTo ?? DateTime.Today) >= DateTime.Today
                                        && ta.AthleteGUID == athlete.AthleteClubGuid
                                    select new MatchPreviewTeamDTO()
                                    {
                                        TeamGuid = ta.Inti_TeamVersion.TeamGUID,
                                        TeamName = ta.Inti_TeamVersion.Inti_Team.Name,
                                        Manager =
                                            String.Format("{0} {1}", ta.Inti_TeamVersion.Inti_Team.Sys_User.FirstName,
                                                ta.Inti_TeamVersion.Inti_Team.Sys_User.LastName).Trim(),
                                        CustomClass = (SessionProps.UserGuid == utf.UserGUID) ? "active" : "list-group-item-info"
                                    }).ToList();


                foreach (var matchPreviewTeamDTO in teamVersions.Where(matchPreviewTeamDTO => !teams.ContainsKey(matchPreviewTeamDTO.TeamGuid)))
                {
                    teams.Add(matchPreviewTeamDTO.TeamGuid, matchPreviewTeamDTO);
                }

                foreach (var matchPreviewTeamDTO in teamVersions)
                {
                    teams[matchPreviewTeamDTO.TeamGuid].Athletes.Add(new AthleteSubDTO() { AthleteClubGuid = athlete.AthleteClubGuid, Name = athlete.Name });
                }
            }


            var allData = new AllDataDTO();
            allData.Athletes = athleteDtos.ToArray();
            allData.Teams = teams.Values.OrderByDescending(t => t.Athletes.Count).ToArray();

            //set PreviewData to the Json object for jsRender to work with
            PreviewData = JsonConvert.SerializeObject(allData);
        }


        private IList<MatchPreviewAthleteDTO> GetAthletesForClub(Guid clubGuid, IntiDataContext db)
        {
            //get a list of athletes represented in teams, sorted by total scores
            var highestScoringAthletes = from ac in db.Inti_AthleteClub
                                         where ac.ClubGUID == clubGuid
                                         select new MatchPreviewAthleteDTO()
                                         {
                                             AthleteGuid = ac.AthleteGUID,
                                             AthleteClubGuid = ac.GUID,
                                             Name = HttpUtility.HtmlDecode(String.Format("{0} {1}", ac.Inti_Athlete.FirstName, ac.Inti_Athlete.LastName).Trim()),
                                             Points = ac.Inti_MatchPointEvent.Any() ? ac.Inti_MatchPointEvent.Sum(mpe => mpe.Points) : 0,
                                             Price = ac.Price ?? 0
                                         };

            //sort by points, then by price
            return highestScoringAthletes.OrderByDescending(a => a.Points).OrderByDescending(a => a.Price).ToList();
        }

        private class AllDataDTO
        {
            public MatchPreviewTeamDTO[] Teams { get; set; }
            public MatchPreviewAthleteDTO[] Athletes { get; set; }
        }

        private class MatchPreviewTeamDTO
        {

            public Guid TeamGuid { get; set; }
            public string TeamName { get; set; }
            public string Manager { get; set; }
            public string CustomClass { get; set; }

            private IList<AthleteSubDTO> _athletes = new List<AthleteSubDTO>(4);
            public IList<AthleteSubDTO> Athletes
            {
                get { return _athletes; }
                set { _athletes = value; }
            }
        }

        private class AthleteSubDTO
        {
            public Guid AthleteClubGuid { get; set; }
            public string Name { get; set; }
        }

        private class MatchPreviewAthleteDTO
        {
            public Guid AthleteGuid { get; set; }
            public Guid AthleteClubGuid { get; set; }
            public int Points { get; set; }
            public int Price { get; set; }
            public string Name { get; set; }

        }
    }
}