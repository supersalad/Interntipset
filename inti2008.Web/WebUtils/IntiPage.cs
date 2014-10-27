using System;
using System.Web;
using System.Web.UI;
using inti2008.Data;
using StackExchange.Profiling;

namespace inti2008.Web
{
    public class IntiPage : Page
    {
        protected const string _cookiestoremove = "CookiesToRemove";
        protected const string _cookiestoadd = "CookiesToAdd";

        protected override void OnPreLoad(EventArgs e)
        {
            if (AuthenticationIsRequired && String.IsNullOrEmpty(SessionProps.UserName))
                WebControlManager.RedirectWithQueryString("SignIn.aspx", new string[]{"SignInredirecturl"}, new string[]{Request.Url.ToString()} );
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            if (Session != null &&
                Session[_cookiestoadd] != null &&
                (Session[_cookiestoadd] is HttpCookie[]))
            {
                foreach (var httpCookie in (Session[_cookiestoadd] as HttpCookie[]))
                {
                    Response.Cookies.Add(httpCookie);    
                }
                

                Session.Remove(_cookiestoadd);
            }

            if (Session != null &&
                Session[_cookiestoremove] != null &&
                Session[_cookiestoremove] is string[])
            {
                foreach (var cookieToRemove in (Session[_cookiestoremove] as string[]))
                {
                    if (Request.Cookies.Get(cookieToRemove) != null)
                        Response.Cookies.Remove(cookieToRemove);    
                }


                Session.Remove(_cookiestoremove);
            }
        }

        private string lastError = "";

        public virtual bool AuthenticationIsRequired
        {
            get { return true; }
        }

        public SessionProperties SessionProps
        {
            get { return Global.SessionProperties; }
        }

        public string LastError()
        {
            return lastError;
        }

        public void VerifyAccess(params string[] permissionPrefix)
        {
            //verify permission
            bool hasPermission = SessionProps.HasPermission("ADMIN_SYSTEM");
            
            if(!hasPermission)
            {
                foreach (var permission in permissionPrefix)
                {
                    //one of the required permissions is enough
                    if (SessionProps.HasPermission(permission))
                    {
                        hasPermission = true;
                    }
                }    
            }
            if (!hasPermission)
            {
                //log the attempted breach
                MailAndLog.SendMessage("Försök att öppna säkrad sida",
                    String.Format("Användaren: {0} med guid: {1} försökte öppna sidan: {2}.", SessionProps.UserName, SessionProps.UserGuid.ToString(), GetType().BaseType.FullName),
                    Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                throw new AccessViolationException("Attempt to open restricted page");
            }
        }

        public bool SignIn(string username, string password)
        {
            //try to fetch the user with the supplied username and password
            var um = new UserManagement(Global.ConnectionString, SessionProps);


            try
            {
                return um.ValidateUser(username, password);
                    
            }
            catch(Exception exception)
            {
                lastError = exception.Message;
            }

            return false;
        }

        protected string GetStatusImage(bool status, string trueText, string falseText)
        {
            if (status)
                return String.Format("<img alt=\"{0}\" title=\"{0}\" src=\"{1}\">", trueText, "img/flag_green.gif");


            return String.Format("<img alt=\"{0}\" title=\"{0}\" src=\"{1}\">", falseText, "img/flag_orange.gif"); ;
        }

        protected string GetAthleteActiveImage(bool isActive, string trueText, string falseText)
        {
            if (!isActive)
                return String.Format("<img alt=\"{0}\" title=\"{0}\" src=\"{1}\">", falseText, "img/flag_red.gif");

            return "";
        }
    }
}
