using System;
using System.Linq;
using System.Web;
using inti2008.Data;
using TweetSharp;

namespace inti2008.Web
{
    public partial class AdminMain : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN", "USER_NEWS", "USER_MATCHUPDATE", "USER_ATHLETEUPDATE");

            //initialt är alla knappar dolda
            if (SessionProps.HasPermission("ADMIN") || SessionProps.HasPermission("USER_NEWS"))
            {
                BtnNews.Visible = true;
                lnkNewsInstructions.Visible = true;
            }


            if (SessionProps.HasPermission("ADMIN") || SessionProps.HasPermission("USER_ATHLETEUPDATE"))
            {
                btnPlayers.Visible = true;
            }
            if (SessionProps.HasPermission("ADMIN") || SessionProps.HasPermission("USER_MATCHUPDATE"))
            {
                btnUpdateMatches.Visible = true;
                lnkUpdateMatchesInstructions.Visible = true;
            }

            if (SessionProps.HasPermission("ADMIN_TOURMASTER") || SessionProps.HasPermission("ADMIN_SYSTEM"))
                btnTournaments.Visible = true;

            if (SessionProps.HasPermission("ADMIN_TOURMASTER") || SessionProps.HasPermission("ADMIN_SYSTEM"))
                btnRules.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM"))
                btnApproveTeams.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM") || SessionProps.HasPermission("ADMIN_USERS"))
                btnUsers.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM"))
                btnProfiling.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM"))
                btnTestMail.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM") && String.IsNullOrEmpty(Parameters.Instance.TwitterAccessTokenSecret))
                btnSignInToTwitter.Visible = true;

            if (SessionProps.HasPermission("ADMIN_SYSTEM") && !String.IsNullOrEmpty(Parameters.Instance.TwitterAccessTokenSecret))
                btnSignOutFromTwitter.Visible = true;

            LoadTopUpdaters();

        }

        private void LoadTopUpdaters()
        {
            var updaters = new StatsManagement(Global.ConnectionString, SessionProps).GetTopUpdaters(
                SessionProps.SelectedTournament.GUID).OrderByDescending(kv => kv.Value);

            var output = updaters.Aggregate(String.Empty, (current, keyValuePair) => current + String.Format("{0} {1} ({2})<br>", keyValuePair.Key.FirstName, keyValuePair.Key.LastName, keyValuePair.Value));

            TopUpdatersPanel.Text = output;

            if (SessionProps.UserName == "folkesson@gmail.com")
            {
                TopUpdatersPanel.Text += "</br>";
                TopUpdatersPanel.Text += "<ul>";
                TopUpdatersPanel.Text += "<li>MailServer: " + Parameters.Instance.MailServer + "</li>";
                TopUpdatersPanel.Text += "<li>MailServerUserName: " + Parameters.Instance.MailServerUserName + "</li>";
                TopUpdatersPanel.Text += "</ul>";

            }
        }

        protected void BtnNews_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminEditNews.aspx");
        }

        protected void btnTournaments_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminTournamentList.aspx");
        }

        protected void btnPlayers_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPlayerList.aspx");
        }

        protected void btnApproveTeams_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminApproveUserTeams.aspx");
        }

        protected void btnUpdateMatches_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminUpdateMatch.aspx");
        }

        protected void lnkUpdateMatchesInstructions_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("MediaPlayer.aspx", new string[] { "mediaToShow" }, new string[] { "MatchUpdate" });
        }

        protected void lnkNewsInstructions_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("MediaPlayer.aspx", new string[] { "mediaToShow" }, new string[] { "NewsUpdate" });
        }

        protected void btnRules_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminEditRules.aspx");
        }

        protected void btnUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminEditUser.aspx");
        }

        protected void btnProfiling_Click(object sender, EventArgs e)
        {
            var profilerCookie = new HttpCookie("Profiling", "1");
            profilerCookie.Expires = DateTime.Now.AddHours(1);
            Session[_cookiestoadd] = new HttpCookie[] {profilerCookie};
        }

        protected void btnSignInToTwitter_Click(object sender, EventArgs e)
        {
            // Step 1 - Retrieve an OAuth Request Token
            TwitterService service = new TwitterService(Global.TwitterConsumerKey, Global.TwitterConsumerSecret);

            // This is the registered callback URL
            OAuthRequestToken requestToken = service.GetRequestToken("http://" + Request.Url.Host + ":" + Request.Url.Port + "/api/TwAuthCallback.ashx");

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);
            Response.Redirect(uri.ToString());
        }

        protected void btnSignOutFromTwitter_Click(object sender, EventArgs e)
        {
            Global.TwitterService = null;
            Parameters.Instance.TwitterAccessTokenSecret = String.Empty;
            Parameters.Instance.TwitterAccessToken = String.Empty;
        }

        protected void btnTestMail_OnClick(object sender, EventArgs e)
        {
            try
            {
                MailAndLog.MailMessageNoCatch("Test mail", "Some body", Parameters.Instance.MailSender,
                    Parameters.Instance.SupportMail);
            }
            catch (Exception exception)
            {
                TopUpdatersPanel.Text = exception.ToString();
            }
        }
    }
}
