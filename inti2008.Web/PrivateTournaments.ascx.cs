using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class PrivateTournaments : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        #region Properties
        private SessionProperties SessionProps
        {
            get
            {
                if (Page is IntiPage)
                    return (Page as IntiPage).SessionProps;

                throw new IntiGeneralException("PrivateTournaments on a non IntiPage page");
            }
        }

        #endregion


        public void LoadPrivateTournaments()
        {
            LoadPrivateTournaments(0);
        }

        public void LoadPrivateTournaments(int nmbrOfItemsToShow)
        {
            lblHeader.Text = "Interna uppgörelser";

            using (var db = Global.GetConnection())
            {
                var matches = from m in db.Inti_Match
                              where m.TournamentGUID == SessionProps.SelectedTournament.GUID &&
                                    (m.IsUpdated ?? false)
                              select new
                                         {
                                             m.TourDay
                                         };

                int currentDay;

                if (matches.ToList().Count > 0)
                    currentDay = matches.Max(mt => mt.TourDay);
                else
                    currentDay = 0;


                var tournaments = from mt in db.Ext_PrivateTournament
                                  where mt.Inti_Tournament.GUID == SessionProps.SelectedTournament.GUID &&
                                        ((mt.IsVisibleForAll ?? false) ||
                                        (SessionProps.UserGuid != Guid.Empty && mt.Ext_PrivateTournamentParticipant.Where(ptp=>ptp.Inti_Team.UserGUID == SessionProps.UserGuid).Count() > 0)) 
                                        &&
                                                              (((mt.IsLimitedInTime ?? false) &&
                                                                (mt.StartDate <= DateTime.Today)) ||
                                                               !(mt.IsLimitedInTime ?? false) &&
                                                               (mt.StartDay <= currentDay))
                                  select new
                                             {
                                                 mt.GUID,
                                                 mt.Name,
                                                 mt.StartDay,
                                                 mt.EndDay,
                                                 mt.IsLimitedInTime,
                                                 mt.StartDate,
                                                 mt.EndDate,
                                                 mt.Description,
                                                 OrderDate = (!(mt.IsLimitedInTime ?? false))
                                                                 ? mt.Inti_Tournament.Inti_Match.Where(m=> m.TourDay == mt.EndDay).Max(m=>m.MatchDate).Value
                                                                 : mt.EndDate,
                                                                 NumberOfContestants = mt.Ext_PrivateTournamentParticipant.Where(ptp=> (ptp.IsAccepted ?? false)).Count()
                                             };

                var totalNumberOfTournaments = tournaments.ToList().Count;

                var theList = (nmbrOfItemsToShow > 0 && nmbrOfItemsToShow > totalNumberOfTournaments) ? 
                    tournaments.OrderBy(mt => mt.OrderDate).Where(mt => mt.OrderDate >= DateTime.Today.AddDays(-7)).Take(nmbrOfItemsToShow).ToList() :
                    tournaments.OrderBy(mt => mt.OrderDate).ToList();

                rptPrivateTournaments.DataSource = theList;
                rptPrivateTournaments.DataBind();

                lblShowAllTournaments.Visible = nmbrOfItemsToShow > 0;
            }
        }
    }
}