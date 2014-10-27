using System;
using System.Linq;
using System.Web.UI.WebControls;
using inti2008.Data;
using inti2008.Web.WebUtils;

namespace inti2008.Web
{
    public partial class News : IntiPage
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
            if(!IsPostBack)
                LoadNews();
        }

        private void LoadNews()
        {
            var page = this.GetRedirectParameter("page", true);
            var pageNumber = 0;
            if (!String.IsNullOrEmpty(page))
                int.TryParse(page, out pageNumber);

            PageNumber = pageNumber;

            //var newsCount = new NewsManagement(SessionProps).GetNumberOfNews();
            var news = new NewsManagement(Global.ConnectionString, SessionProps).GetNews(25, PageNumber);

            LastPage = news.Count < 25;

            rptNews.DataSource = news.Select(n => new NewsDto(n));
            rptNews.DataBind();

            //grdNews.DataSource = news.ToList();
            //grdNews.DataBind();

        }

        public int PageNumber { get; set; }
        public bool LastPage { get; set; }

        private class NewsDto
        {
            private const int maxBodyLength = 255;

            public NewsDto(Ext_News newsItem)
            {
                GUID = newsItem.GUID;
                Header = newsItem.Header;
                var body = newsItem.Body.StripTagsRegexCompiled();
                Body = body.Length > maxBodyLength ? body.Substring(0, maxBodyLength) + "..." : body;
                ValidFrom = newsItem.ValidFrom ?? DateTime.Today;
            }

            public Guid GUID { get; set; }
            public string Header { get; set; }
            public string Body { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        protected string ParseOutput(string text)
        {
            return text.Replace("\n", "<br>").StripTagsRegexCompiled();
            //return Server.HtmlEncode(text.Replace("<br>","\n")).Replace("\n", "<br>");
        }

        protected void grdNews_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdNews.PageIndex = e.NewPageIndex;
            LoadNews();
        }

        protected string ParseDateOutput(string inDate)
        {
            DateTime dateTime;
            if (DateTime.TryParse(inDate, out dateTime))
                return dateTime.ToShortDateString();

            return "";
        }
    }
}
