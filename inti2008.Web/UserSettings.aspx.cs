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
    public partial class UserSettings : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Cancel.Attributes.Add("onclick", "window.history.go(-1);");
            if (!IsPostBack)
            {
                using (var db = Global.GetConnection())
                {
                    var userQ = from u in db.Sys_User
                                where u.UserName == SessionProps.UserName
                                select u;

                    var user = userQ.ToList()[0];

                    FirstName.Text = user.FirstName;
                    LastName.Text = user.LastName;
                }
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            //validate
            if(FirstName.Text.Trim().Length ==0 || LastName.Text.Trim().Length == 0)
            {
                lblMessage.Text = "Du måste ange både förnamn och efternamn";
            }
            else
            {
                using (var db = Global.GetConnection())
                {
                    var userQ = from u in db.Sys_User
                                where u.UserName == SessionProps.UserName
                                select u;

                    var user = userQ.ToList()[0];

                    user.FirstName = FirstName.Text;
                    user.LastName = LastName.Text;

                    db.SubmitChanges();

                    lblMessage.Text = "Nu är ändringarna sparade";
                }   
            }

            
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("/UserTeamView.aspx", new[] { "GUID" }, new[] { SessionProps.UserGuid.ToString() });
        }
    }
}
