using System;
using System.Collections.Generic;
using System.Web.UI;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class menu : System.Web.UI.UserControl
    {
        public SessionProperties SessionProps
        {
            get
            {
                return Global.SessionProperties;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //i grundläget är alla knappar synliga

            if (String.IsNullOrEmpty(SessionProps.UserName))
            {
                //ej inloggad
                BtnSignOut.Visible = false;
                lnkAdmin.Visible = false;
                BtnMyPage.Visible = false;
                BtnNewMessage.Visible = false;
                
            }
            else
            {
                //inloggad
                lnkSignIn.Visible = false;
                lnkSignUp.Visible = false;

                //new message?
                BtnNewMessage.Visible = new UserManagement(Global.ConnectionString, SessionProps).HasNewMessages(SessionProps.UserGuid);

                lnkAdmin.Visible = SessionProps.HasPermission("ADMIN") ||
                    SessionProps.HasPermission("USER_MATCHUPDATE") ||
                    SessionProps.HasPermission("USER_NEWS") ||
                    SessionProps.HasPermission("USER_ATHLETEUPDATE");
            }
        }

        
        protected void BtnMyPage_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("/UserTeamView.aspx", new[]{"GUID"},new[]{SessionProps.UserGuid.ToString()});
        }

       

       

      

        protected void BtnSignOut_Click(object sender, EventArgs e)
        {
            SessionProps.SignOut();

            var cookiesToRemove = new List<string>();

            //remove auto-sign in cookie?
            if (Request.Cookies.Get("SignMeIn") != null)
                cookiesToRemove.Add("SignMeIn");

            if (Request.Cookies.Get("Profiling") != null)
                cookiesToRemove.Add("Profiling");

            if (cookiesToRemove.Count > 0)
                Session["CookiesToRemove"] = cookiesToRemove.ToArray();

            Response.Redirect("/Default.aspx");
        }



        protected void BtnNewMessage_Click(object sender, ImageClickEventArgs e)
        {
            WebControlManager.RedirectWithQueryString("/UserMessages.aspx", new[] { "GUID" }, new[] { SessionProps.UserGuid.ToString() });
        }
    }
}