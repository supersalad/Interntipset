using System;
using System.Collections;
using System.Collections.Generic;
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
    public partial class SignIn : IntiPage
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

            //Set default focus on login UserName button.
            Page.Form.DefaultFocus = login.FindControl("UserName").UniqueID;
            //Set default button to the loginbutton.
            Page.Form.DefaultButton = login.FindControl("LoginButton").UniqueID;

            //set labels
            login.LoginButtonText = "Logga in";
            login.TitleText = "Ange epost-adress och lösenord";
            login.UserNameLabelText = "Epost:";
            login.PasswordLabelText = "Lösenord:";
            login.PasswordRecoveryText = "Glömt lösenord?";
            login.PasswordRecoveryUrl = "ForgotPassword.aspx";
            login.PasswordRequiredErrorMessage = "Du måste ange lösenord";
            login.UserNameRequiredErrorMessage = "Du måste ange epost";
            login.RememberMeText = "Kom ihåg mig på den här datorn";
            login.DestinationPageUrl = String.IsNullOrEmpty(Request.Params["redirecturl"]) ? "default.aspx" : Request.Params["default.aspx"];

            login.PasswordRecoveryText = "Glöm lösenordet?";
            login.PasswordRecoveryUrl = "ForgotPassword.aspx";

            login.CreateUserText = "Skapa ny användare?";
            login.CreateUserUrl = "SignUp.aspx";

        }

        protected void login_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                e.Authenticated = SignIn(login.UserName, login.Password);

                if (e.Authenticated)
                {
                    //set login name
                    SessionProps.UserName = login.UserName;

                    //set user guid
                    var user = new UserManagement(Global.ConnectionString, SessionProps).GetUserByName(SessionProps.UserName);

                    SessionProps.UserGuid = user.GUID;

                    //set footer text
                    SessionProps.FooterText = String.Format("Inloggad som {0} {1}", user.FirstName, user.LastName);

                    //set permissions
                    SessionProps.Permissions = new UserManagement(Global.ConnectionString, SessionProps).GetUserPermissions(SessionProps.UserName);

                    //automatic signin?
                    var cookiesToAdd = new List<HttpCookie>();
                    if(login.RememberMeSet)
                    {
                        var cookie = new HttpCookie("SignMeIn", user.GUID.ToString());
                        cookie.Expires = DateTime.Now.AddYears(1);
                        cookiesToAdd.Add(cookie);
                    }

                    ////profiler on?
                    //if (SessionProps.UserGuid == new Guid("8116E67B-DBD7-40BC-932D-18DFF21B04B2"))
                    //{
                    //    var profilerCookie = new HttpCookie("Profiling", "1");
                    //    profilerCookie.Expires = DateTime.Now.AddYears(1);
                    //    cookiesToAdd.Add(profilerCookie);
                    //}

                    //add cookies?
                    if (cookiesToAdd.Count > 0)
                        Session[_cookiestoadd] = cookiesToAdd.ToArray();

                    var redirectUrl = this.GetRedirectParameter("SignInredirecturl", true);

                    if (redirectUrl == null)
                        Response.Redirect("Default.aspx");
                    else
                        Response.Redirect(redirectUrl.ToString());

                }
                else
                {
                    login.FailureText = LastError();
                }
            }
            catch (IntiGeneralException exception)
            {
                login.FailureText = exception.Message;
            }
        }
    }
}
