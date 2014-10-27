using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class NewsItem : IntiPage
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
            var header = "Hittade inte nyheten";
            var body = "";
            var newsId = this.GetRedirectParameter("id",true);
            if (!String.IsNullOrEmpty(newsId))
            {
                try
                {
                    var newsGuid = new Guid(newsId);
                    var newsItem = new NewsManagement(Global.ConnectionString, SessionProps).GetNewsItem(newsGuid);
                    if (newsItem != null)
                    {
                        header = newsItem.Header;
                        body = ParseOutput(newsItem.Body);
                    }
                }
                catch (Exception)
                {
                    
                    //do nothing - bad input data - can't find news item
                }
                
            }

            Header.Text = header;
            Body.Text = body;

        }

        protected string ParseOutput(string text)
        {
            return text.Replace("\n", "<br>");
            //return Server.HtmlEncode(text.Replace("<br>","\n")).Replace("\n", "<br>");
        }
    }
}