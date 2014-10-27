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
    public partial class ChangePassword : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Cancel.Attributes.Add("onclick", "window.history.go(-1);");
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            //kolla först att det nuvarande lösenordet är rätt
            var um = new UserManagement(Global.ConnectionString, SessionProps);

            try
            {
                if (um.ValidateUser(SessionProps.UserName, PresentPassword.Text))
                {
                    //stämmer det nya lösenordet?
                    if (NewPassword.Text == ConfirmNewPassword.Text)
                    {
                        //uppdatera usern
                        using (var db = Global.GetConnection())
                        {
                            var userQ = from u in db.Sys_User
                                        where u.UserName == SessionProps.UserName
                                        select u;

                            var user = userQ.ToList()[0];

                            user.Password = NewPassword.Text;

                            db.SubmitChanges();

                            lblMessage.Text = "Nu är det nya lösenordet sparat";
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Bekräftelsen av det nya lösenordet misslyckades";
                    }
                }
                else
                {
                    lblMessage.Text = "Det nuvarande lösenordet är felaktigt";
                }
            }
            catch (IntiGeneralException)
            {
                lblMessage.Text = "Det nuvarande lösenordet är felaktigt";
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("/UserTeamView.aspx", new[] { "GUID" }, new[] { SessionProps.UserGuid.ToString() });
        }
    }
}
