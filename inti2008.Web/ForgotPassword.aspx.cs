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
    public partial class ForgotPassword : IntiPage
    {
        //can be anonymous on this page
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
                Email.Text = (string)this.GetRedirectParameter("email", false);
        }

        protected void Send_Click(object sender, EventArgs e)
        {
            //try to get the user
            try
            {
                new UserManagement(Global.ConnectionString, SessionProps).ForgotPassword(Email.Text);

                lblMessage.Text = "Ditt nya lösenord har skickats till dig.";
            }
            catch (IntiGeneralException exception)
            {
                lblMessage.Text = exception.Message;

            }
            

        }
    }
}
