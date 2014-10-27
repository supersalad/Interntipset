using System;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class _Default : IntiPage
    {
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
            {
                GetTopThreeNews();

                LoadTopTen();
                LoadFormTable();
                LoadLatestForumEntries();

                LoadPrivateTournaments();


            }
        }

        private void LoadPrivateTournaments()
        {
            privateTournaments.LoadPrivateTournaments(10);
        }

        private void LoadRssFeed()
        {
            //rssSportal.Url = "http://sportal.se/sport/fotboll/premierleague/feed.rss";
        }

        private void LoadLatestForumEntries()
        {
            forumEntries.LoadLatestEntries(5);
        }

        private void GetTopThreeNews()
        {
            var newsList = new NewsManagement(Global.ConnectionString, SessionProps).GetTopThreeNews();

            lblNews.Text = "";

            foreach(var news in newsList)
            {
                lblNews.Text += "<h4>" + news.Header + "</h4>";
                lblNews.Text += "<p>" + news.Body.Replace("\n", "<br>") + "</p>";
                lblNews.Text += "<br><br>";
            }
        }

        private void LoadTopTen()
        {
            topTenStanding.LoadTopTen();
        }

        private void LoadFormTable()
        {
            formTable.LoadTopLastMonth();
        }

    }
}
