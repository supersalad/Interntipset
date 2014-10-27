using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class Standing : IntiPage
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
            if (!IsPostBack)
            {
                TournamentStanding.LoadFullTournament();
                daySelector.LoadTournamentDays();
            }
        }

        
        protected void daySelector_SelectedDayChanged()
        {
            //LoadStanding();
            TournamentStanding.LoadFullTournament(daySelector.SelectedDay);
        }

    }
}
