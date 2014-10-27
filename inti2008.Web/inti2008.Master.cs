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
    public partial class inti2008 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadFooterText();
        }

        public SessionProperties SessionProps
        {
            get
            {
                return Global.SessionProperties;
            }
        }

        private void LoadFooterText()
        {
            lblFooterText.Text = String.Empty;
            
            if (SessionProps == null) return;
            if (SessionProps.SelectedTournament == null) return;

            lblFooterText.Text = SessionProps.SelectedTournament.Name + " | " + SessionProps.FooterText + " | " + DateTime.Now.ToString();
        }
    }
}
