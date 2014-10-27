using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
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
    public partial class Error : IntiPage
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
                var exception = Request.Params["errorMsg"].ToString();
                if (exception != null)
                {
                    lblErrorDescription.Text = exception;
                    
                }
            }

            lblMailLink.Text = "<a href=\"mailto:" + Parameters.Instance.SupportMail + "\">" +
                               Parameters.Instance.SupportMail + "</a>";
            
        }
    }
}
