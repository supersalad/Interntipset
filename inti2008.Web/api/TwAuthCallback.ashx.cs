using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using inti2008.Data;
using TweetSharp;


namespace inti2008.Web.api
{
    /// <summary>
    /// Summary description for TwAuthCallback
    /// </summary>
    public class TwAuthCallback : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //get parameters
            string oauth_token = context.Request.Params["oauth_token"];
            string oauth_verifier = context.Request.Params["oauth_verifier"];

            var requestToken = new OAuthRequestToken { Token = oauth_token };

            // Step 3 - Exchange the Request Token for an Access Token
            Global.TwitterService = new TwitterService(Global.TwitterConsumerKey, Global.TwitterConsumerSecret);
            OAuthAccessToken accessToken = Global.TwitterService.GetAccessToken(requestToken, oauth_verifier);

            // Step 4 - User authenticates using the Access Token
            Global.TwitterService.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            TwitterUser user = Global.TwitterService.VerifyCredentials(new VerifyCredentialsOptions());

            //var sessionProps = (SessionProperties) context.Session["sessionProps"];
            //sessionProps.TwitterAccessToken = accessToken.Token;
            //sessionProps.TwitterAccessTokenSecret = accessToken.TokenSecret;
            
            Parameters.Instance.TwitterAccessToken = accessToken.Token;
            Parameters.Instance.TwitterAccessTokenSecret = accessToken.TokenSecret;

            //service.SendTweet(new SendTweetOptions(){Status = "Testing twitter function. Carrry on"});
            context.Response.Redirect("../AdminMain.aspx");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}