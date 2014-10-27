using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class TournamentStanding : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Properties
        private int _tournament;
        private int _nmbrOfReturns;
        private DateTime _resultSince = DateTime.MinValue;
        private int _tourDayParam = 0;

        private SessionProperties SessionProps
        {
            get
            {
                if (Page is IntiPage)
                    return (Page as IntiPage).SessionProps;

                throw new IntiGeneralException("TournamentStanding on a non IntiPage page");
            }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Load top ten teams of current tournament
        /// </summary>
        public void LoadTopTen()
        {
            lblHeader.Text = "Topp 10";

            BindStanding(GetStanding(0, 0, 10));
        }

        /// <summary>
        /// Load top teams for a match
        /// </summary>
        /// <param name="matchGuid"></param>
        public void LoadMatch(Guid matchGuid)
        {
            lblHeader.Text = "Bästa poängplockarna";

            BindStanding(GetStanding(0, 0, 0, matchGuid));
        }

        /// <summary>
        /// Load top teams for the current tournament for the last 30 days
        /// </summary>
        public void LoadTopLastMonth()
        {
            lblHeader.Text = "Bäst sen " + DateTime.Today.AddDays(-30).ToShortDateString();

            BindStanding(GetStanding(0, 30, 10));
        }

        /// <summary>
        /// Load the full standing of the current tournament
        /// </summary>
        /// <param name="tourDay"></param>
        public void LoadFullTournament(int tourDay  =0)
        {
            _tourDayParam = tourDay;
            lblHeader.Text = SessionProps.SelectedTournament.Name;
            lblDescription.Text = SessionProps.SelectedTournament.Description;
            BindStanding(GetStanding(tourDay));
        }

        /// <summary>
        /// Load the standing of a private tournamnet
        /// </summary>
        /// <param name="pvtTournament"></param>
        public void LoadPrivateTournamentStanding(Ext_PrivateTournament pvtTournament)
        {
            using (var db = Global.GetConnection())
            {
                lblHeader.Text = pvtTournament.Name;
                var description = Server.HtmlEncode(pvtTournament.Description).Replace("\n", "<BR/>");
                description += " ";
                description += (pvtTournament.IsLimitedInTime ?? false)
                    ? String.Format("Gäller matcher mellan {0} och {1}",
                        (pvtTournament.StartDate ?? SessionProps.SelectedTournament.EndRegistration).ToShortDateString(),
                        (pvtTournament.EndDate ?? SessionProps.SelectedTournament.EndRegistration.AddYears(1))
                            .ToShortDateString())
                    : String.Format("Gäller matcher mellan omgång {0} och {1}", (pvtTournament.StartDay ?? 1),
                        (pvtTournament.EndDay ?? SessionProps.SelectedTournament.NmbrOfDays));
                lblDescription.Text = description.Trim();


                BindStanding(GetPrivateTournamentStanding(pvtTournament));
            }

        }

        #endregion

        private List<StandingDto> GetPrivateTournamentStanding(Ext_PrivateTournament pvtTournament)
        {
            using (var db = Global.GetConnection())
            {
                var standing = from ptp in db.Ext_PrivateTournamentParticipant
                               where ptp.PrivateTournamentGUID == pvtTournament.GUID
                                     && (ptp.IsAccepted ?? false)
                                     && (ptp.Inti_Team.IsActive ?? false)
                                     && (ptp.Inti_Team.IsPaid ?? false)
                               select new StandingDto
                               {
                                   GUID = ptp.Inti_Team.GUID,
                                   TeamName = ptp.Inti_Team.Name,
                                   Manager = ptp.Inti_Team.Sys_User.FirstName + " " + ptp.Inti_Team.Sys_User.LastName,
                                   Points = (ptp.Inti_Team.Inti_TeamPointEvents.Where(tpe =>
                                       ((pvtTournament.IsLimitedInTime ?? false)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.MatchDate >= pvtTournament.StartDate
                                             && tpe.Inti_MatchPointEvent.Inti_Match.MatchDate <= (pvtTournament.EndDate ?? tpe.Inti_MatchPointEvent.Inti_Match.MatchDate))
                                         ||
                                         (!(pvtTournament.IsLimitedInTime ?? false)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.TourDay >= (pvtTournament.StartDay ?? tpe.Inti_MatchPointEvent.Inti_Match.TourDay)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.TourDay <= (pvtTournament.EndDay ?? tpe.Inti_MatchPointEvent.Inti_Match.TourDay)))
                                             .Count() > 0 ? ((ptp.Inti_Team.Inti_TeamPointEvents.Where(tpe =>
                                       ((pvtTournament.IsLimitedInTime ?? false)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.MatchDate >= pvtTournament.StartDate
                                             && tpe.Inti_MatchPointEvent.Inti_Match.MatchDate <= (pvtTournament.EndDate ?? tpe.Inti_MatchPointEvent.Inti_Match.MatchDate))
                                         ||
                                         (!(pvtTournament.IsLimitedInTime ?? false)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.TourDay >= (pvtTournament.StartDay ?? tpe.Inti_MatchPointEvent.Inti_Match.TourDay)
                                             && tpe.Inti_MatchPointEvent.Inti_Match.TourDay <= (pvtTournament.EndDay ?? tpe.Inti_MatchPointEvent.Inti_Match.TourDay)))
                                             .Sum(tpe => tpe.Inti_MatchPointEvent.Points))) : 0)
                               };
                return standing.OrderByDescending(x=>x.Points).ToList();
            }
        }

        private List<StandingDto> GetStanding(int tourDay, int sinceDays = 0, int nmbrOfReturns = 0, Guid matchGuid = new Guid())
        {
            var sinceDate = DateTime.MinValue;
            if (sinceDays != 0)
                sinceDate = DateTime.Today.AddDays(-sinceDays);


            using (var db = Global.GetConnection())
            {

                var userFavorites = from utf in db.Ext_UserFavoriteTeam
                                    where utf.UserGUID == SessionProps.UserGuid
                                    select utf;

                var standing = from t in db.Inti_Team
                               join u in userFavorites
                                    on t equals u.Inti_Team into oj
                               from utf in oj.DefaultIfEmpty()
                               where t.TournamentGUID == SessionProps.SelectedTournament.GUID
                                     && (t.IsActive ?? false) && (t.IsPaid ?? false)
                               select new StandingDto
                               {
                                   GUID = t.GUID,
                                   TeamName = t.Name,
                                   Manager = t.Sys_User.FirstName + " " + t.Sys_User.LastName,
                                   Points =
                                     (tourDay == 0 && matchGuid == Guid.Empty ? t.BonusPoints : 0)
                                   + (t.Inti_TeamPointEvents.Count(tpe => (tourDay == 0 && matchGuid == Guid.Empty ||
                                                  tpe.Inti_MatchPointEvent.Inti_Match.TourDay == tourDay
                                                  || tpe.Inti_MatchPointEvent.Inti_Match.GUID == matchGuid)) > 0 ? 
                                             t.Inti_TeamPointEvents.Where(tpe => (tourDay == 0 ||
                                                          tpe.Inti_MatchPointEvent.Inti_Match.TourDay == tourDay)
                                                          && (sinceDate == DateTime.MinValue || tpe.Inti_MatchPointEvent.Inti_Match.MatchDate >= sinceDate)
                                                          && matchGuid == Guid.Empty || tpe.Inti_MatchPointEvent.Inti_Match.GUID == matchGuid)
                                   .Sum(tpe => tpe.Inti_MatchPointEvent.Points) : 0),
                                   CustomClass = (SessionProps.UserGuid == utf.UserGUID) ? "active" : ""
                               };

                return nmbrOfReturns != 0 ? standing.OrderByDescending(s => s.Points).Take(nmbrOfReturns).ToList() : standing.OrderByDescending(s => s.Points).ToList();
                
            }
        }

        private void BindStanding(List<StandingDto> list)
        {
            rptStanding.DataSource = list;
            rptStanding.DataBind();
        }

        protected string GetPosition()
        {
            return (rptStanding.Items.Count + 1).ToString();
        }

        protected string GetTourDayParam()
        {
            //todo: get tour day if any
            return _tourDayParam.ToString();
        }

    }

    internal class StandingDto
    {
        public Guid GUID { get; set; }
        public string TeamName { get; set; }
        public string Manager { get; set; }
        public int? Points { get; set; }
        public string CustomClass { get; set; }
    }
}