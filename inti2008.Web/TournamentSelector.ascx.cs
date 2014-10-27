using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class TournamentSelector : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                LoadTournaments();
        }

        private void LoadTournaments()
        {
            if(drpTournament.Items.Count ==0)
            {
                drpTournament.DataValueField = "GUID";
                drpTournament.DataTextField = "Name";
                drpTournament.DataSource = new CommonDataSources(Global.ConnectionString, Global.SessionProperties).AllTournaments();
                drpTournament.DataBind();    
            }

            drpTournament.SelectedValue = Global.SessionProperties.SelectedTournament.GUID.ToString();
        }

        protected void drpTournament_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tours = new CommonDataSources(Global.ConnectionString, Global.SessionProperties).AllTournaments();
            foreach (var tournament in tours)
            {
                if (tournament.GUID == new Guid(drpTournament.SelectedValue))
                {
                    Global.SessionProperties.SelectedTournament = tournament;
                    break;
                }
                    
            }

            WebControlManager.RefreshPage();
            
        }
    }
}