using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using BitlyDotNET.Implementations;
using BitlyDotNET.Interfaces;
using inti2008.Data;
using StackExchange.Profiling;
using TweetSharp;


namespace inti2008.Web
{
    public class Global : System.Web.HttpApplication
    {
        internal static string ConnectionString;

        public static SessionProperties SessionProperties
        {
            get
            {
                if (HttpContext.Current.Session == null) return SessionProperties.NullSessionProperties();

                return ((SessionProperties)HttpContext.Current.Session["sessionProps"]) ?? SessionProperties.NullSessionProperties();
            }
        }

        public static string TwitterConsumerKey { get { return "JhQBCfxPNyxeHXVKzu3Xow"; } }

        public static string TwitterConsumerSecret { get { return "ca2tCaLZNI5Cow5SSc1Mes3J9xeUGwrK2a4DUnlJas"; } }

        
        public static TwitterService TwitterService { get; set; }
        //public static string TwitterAccessToken { get; set; }
        //public static string TwitterAccessTokenSecret { get; set; }

        private static void InitiateTwitterAuthentication(SessionProperties sessionProperties)
        {
            if (String.IsNullOrEmpty(Parameters.Instance.TwitterAccessToken) ||
                String.IsNullOrEmpty(Parameters.Instance.TwitterAccessTokenSecret))
                return;

            try
            {
                // Step 3 - Exchange the Request Token for an Access Token
                Global.TwitterService = new TwitterService(Global.TwitterConsumerKey, Global.TwitterConsumerSecret);


                // Step 4 - User authenticates using the Access Token
                Global.TwitterService.AuthenticateWith(Parameters.Instance.TwitterAccessToken, Parameters.Instance.TwitterAccessTokenSecret);
                TwitterUser user = Global.TwitterService.VerifyCredentials(new VerifyCredentialsOptions());

                if (user == null) TwitterService = null;
            }
            catch (Exception exception)
            {
                WebControlManager.SendAndLogErrorMessage(exception, Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                Global.TwitterService = null;
            }
        }

        public static void SendTweet(string tweet, string appendUrl, SessionProperties sessionProperties)
        {
            if (TwitterService == null)
            {
                InitiateTwitterAuthentication(sessionProperties);
            }


            if (TwitterService == null)
            {
                WebControlManager.SendAndLogErrorMessage(new Exception("TwitterService not initialized"), Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                //clear authentication, guess we need to authenticate again?
                Parameters.Instance.TwitterAccessToken = null;
                Parameters.Instance.TwitterAccessTokenSecret = null;
                return;
            }

            try
            {

                //format the string, replace 
                if (tweet.Contains("&"))
                {
                    tweet = HttpUtility.HtmlDecode(tweet);
                }

                string shortUrl = String.Empty;
                //parse url to shorturl
                if (!String.IsNullOrEmpty(appendUrl))
                {
                    IBitlyService s = new BitlyService("o_3cpfcndji4", "R_8e203358cb5ca0f68d809419b056b192");

                    string shortened = s.Shorten(appendUrl);
                    if (shortened != null)
                    {
                        shortUrl = " " + shortened;
                    }
                }
                var maxLength = 140 - shortUrl.Length;
                if (tweet.Length > maxLength)
                    tweet = tweet.Substring(0, maxLength);

                tweet += shortUrl;

                TwitterService.SendTweet(new SendTweetOptions(){Status = tweet});

                // Likely this is an error; we don't have to go fishing for it
                TwitterError error = TwitterService.Response.Error;
                TwitterResponse response = TwitterService.Response;
                if (error != null || response.StatusCode != HttpStatusCode.OK)
                {
                    // You now know you have a real error from Twitter, and can handle it
                    string message;
                    if (error != null)
                    {
                        message = String.Format("Twitter error: {0} ({1})", error.Message, error.Code);
                    }
                    else
                    {
                        message = String.Format("Twitter response status not ok: {0} ({1})\n{2}",
                            response.StatusDescription, response.StatusCode, response.Response);
                    }
                    WebControlManager.SendAndLogErrorMessage(new Exception(message), Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                }
                
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static IntiDataContext GetConnection()
        {
            var connection = new StackExchange.Profiling.Data.ProfiledDbConnection(new SqlConnection(ConnectionString),
                                                                                   MiniProfiler.Current);
            return new IntiDataContext(connection);
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            
            //fallback
            ConnectionString =
                @"Data source=database.onricoh.se;Initial catalog=interntipset;User Id=inti;Password=brollan1";

            var webConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            #if DEBUG
                ConnectionString = webConfig.ConnectionStrings.ConnectionStrings["Debug"].ConnectionString;
            #else
                ConnectionString = webConfig.ConnectionStrings.ConnectionStrings["Release"].ConnectionString;
            #endif

            //initiate parameters
            Parameters.Init(ConnectionString);

            //ensure all parameters
            Parameters.Instance.UpdateParameters();

            RegisterRoutes(RouteTable.Routes);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var clientInfo = WebControlManager.GetClientInfo();
            var sessionInfo = new SessionProperties(true, clientInfo);

            //get the default tournament
            using (var db = Global.GetConnection())
            {
                var tours = from t in db.Inti_Tournament
                            select t;

                foreach (var tour in tours.OrderByDescending(tStart => tStart.StartRegistration).ToList())
                {
                    sessionInfo.SelectedTournament = tour;
                    sessionInfo.DefaultTournament = tour;
                    break;
                }
                
            }

            if (Request.Cookies != null)
            {
                if (Request.Cookies.Get("SignMeIn") != null)
                {
                    //automatic sign in
                    sessionInfo.UserGuid = new Guid(Request.Cookies["SignMeIn"].Value);

                    //set user guid
                    var user = new UserManagement(Global.ConnectionString, sessionInfo).GetUserByGuid(sessionInfo.UserGuid);

                    sessionInfo.UserName = user.UserName;

                    //set footer text
                    sessionInfo.FooterText = String.Format("Inloggad som {0} {1}", user.FirstName, user.LastName);

                    //set permissions
                    sessionInfo.Permissions = new UserManagement(Global.ConnectionString, sessionInfo).GetUserPermissions(sessionInfo.UserName);

                    
                }
            }
            
            

            //set as not signed in
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Add("sessionProps", sessionInfo);

            
            
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //if (SessionProperties.UserGuid == new Guid("8116E67B-DBD7-40BC-932D-18DFF21B04B2"))
            //    MiniProfiler.Start();

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.Cookies["Profiling"] != null)
                MiniProfiler.Start();

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            MiniProfiler.Stop();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError().InnerException ?? Server.GetLastError();

            //mail error message
#if !DEBUG
            WebControlManager.SendAndLogErrorMessage(exception, Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
#endif

            var exceptionMsg = Server.HtmlEncode(exception.Message);
            //exceptionMsg += "<p>" + Server.HtmlEncode(exception.StackTrace).Replace("\n", "<br>").Replace(" at ", "<br> at ") + "</p>";        

            Response.Redirect("/Error.aspx?errorMsg="+Server.UrlEncode(exceptionMsg));
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("TeamRoute", "Team/{teamGUID}", "~/TeamView.aspx");
            routes.MapPageRoute("TeamRouteDay", "Team/{teamGUID}/{tourDay}", "~/TeamView.aspx");

            routes.MapPageRoute("NewsList", "NewsList/{page}", "~/News.aspx");
            routes.MapPageRoute("News", "News/{id}", "~/NewsItem.aspx");

            routes.MapPageRoute("Match", "Match/{id}", "~/MatchView.aspx");

            routes.MapPageRoute("Club", "Club/{id}", "~/Club.aspx");


            routes.MapPageRoute("Player", "Player/{acGUID}", "~/PlayerView.aspx");

        }
    }
}