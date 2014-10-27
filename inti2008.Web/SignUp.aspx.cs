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
    public partial class SignUp : IntiPage
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

        }

        protected void SignMeUp_Click(object sender, EventArgs e)
        {

            if (Password.Text == PasswordConfirm.Text)
            {
                try
                {

                    var um = new UserManagement(Global.ConnectionString, SessionProps);
                    um.RegisterUser(FirstName.Text, LastName.Text, Email.Text, Password.Text);

                    //if we are here, registration was successful
                    pnlForm.Visible = false;
                    lblMessage.Text = "Nu är du registrerad, gå till <a href=\"SignIn.aspx\">inloggningssidan</a> och logga in.";
                }
                catch (IntiGeneralException intiGeneralException)
                {
                    lblMessage.Text = intiGeneralException.Message;
                }    
            }
            else
            {
                lblMessage.Text = "Bekräftelsen av lösenordet matchar inte lösenordet";
            }

            
            
            
        }
    }
}
