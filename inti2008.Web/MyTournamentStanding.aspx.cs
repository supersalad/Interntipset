using System;
using System.Linq;

namespace inti2008.Web
{
    public partial class MyTournamentStanding : IntiPage
    {
        private string _tournamentName;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadPvtTournament();
        }

        private void LoadPvtTournament()
        {
            var inGuid = this.GetRedirectParameter("pvtTournamentGUID", false);
            if (inGuid != null)
            {
                var guid = new Guid(inGuid.ToString());
                using (var db = Global.GetConnection())
                {
                    var pvtTournament = db.Ext_PrivateTournament.Single(ptv => ptv.GUID == guid);
                    _tournamentName = pvtTournament.Name;
                    TournamentStanding.LoadPrivateTournamentStanding(pvtTournament);
                }
                
            }
        }

        protected string GetTournamentName()
        {
            return _tournamentName;
        }
    }
}
