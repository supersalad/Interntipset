using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using inti2008.Data;
using inti2008.Web.api;

namespace inti2008.Web
{
    public partial class RulesShow : IntiPage
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
                LoadRules();

                LoadTransferWindows();
            }
        }

        private void LoadTransferWindows()
        {
            using (var db = Global.GetConnection())
            {
                var transferPeriods = from t in db.Inti_TransferPeriod
                            where t.TournamentGUID == SessionProps.SelectedTournament.GUID
                            select new
                                   {
                                       Header = t.Name,
                                       Body = String.Format("<p>Öppnar: {0} {1} {2}</p><p>Stänger: {3} {4} {5}</p>", 
                                       t.StartDate.ToShortDateString(), t.StartDate.ToShortTimeString(),
                                       CalendarLink.GetCalendarLink("Bytesfönster i Interntipset - öppnar",
                                       t.StartDate, t.StartDate.AddMinutes(1), t.GUID.ToString()+"s"),
                                       t.EndDate.ToShortDateString() , t.EndDate.ToShortTimeString(), 
                                       CalendarLink.GetCalendarLink("Bytesfönster i Interntipset - deadline", 
                                       t.EndDate.AddHours(-2), t.EndDate, t.GUID.ToString()+"e")),
                                       DeadLine = t.EndDate
                                   };


                rptTransferWindows.DataSource = transferPeriods.OrderBy(t=>t.DeadLine).ToList();
                rptTransferWindows.DataBind();

            }
        }

        private void LoadRules()
        {
            using (var db = Global.GetConnection())
            {
                var rules = from r in db.Inti_TournamentRule
                            where r.TournamentGUID == SessionProps.SelectedTournament.GUID
                            select r;

                rptRules.DataSource = rules.OrderBy(r => r.SortOrder).ToList();
                rptRules.DataBind();

                lblRulesHeader.Text = "Regler för interntipset - " + SessionProps.SelectedTournament.Name;

            }
        }
    }
}
