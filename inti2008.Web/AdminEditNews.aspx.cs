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
    public partial class AdminEditNews : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN", "USER_NEWS");

            if (!IsPostBack)
            {
                LoadNews();
                pnlNewsEditor.Visible = false;
            }
                

        }

        private void LoadNews()
        {
            grdAllNews.DataKeyNames = new string[] { "GUID" };
            grdAllNews.DataSource = new NewsManagement(Global.ConnectionString, SessionProps).GetAllNews(grdAllNews.PageSize, 0);
            grdAllNews.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdAllNews.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            //todo: register page browsing for event validation


            base.Render(writer);
        }

        private void EditNewsItems(Guid newsGuid)
        {
            pnlNewsEditor.Visible = true;

            var newsItem = new NewsManagement(Global.ConnectionString, SessionProps).GetNewsItem(newsGuid);

            Id = newsGuid;

            Header.Text = newsItem.Header;
            Body.Text = newsItem.Body;
            ValidFrom.Text = newsItem.ValidFrom.ToString();
            ValidTo.Text = newsItem.ValidTo.ToString() ?? "";

            btnTweet.Visible = true;

            newsChangeLog.LoadChangeLog(Id);

        }

        protected void grdAllNews_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdAllNews.DataKeyNames = new string[] { "GUID" };
            grdAllNews.DataSource = new NewsManagement(Global.ConnectionString, SessionProps).GetNews(grdAllNews.PageSize, e.NewPageIndex);
            grdAllNews.DataBind();
        }

        private Guid Id
        {
            get
            {
                if (ViewState["Guid"] is Guid)
                    return (Guid)ViewState["Guid"];
                
                return Guid.Empty;
            }
            set
            {
                ViewState["Guid"] = value;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var validFrom = DateTime.Parse(ValidFrom.Text);
            var testDate = DateTime.MinValue;
            DateTime? validTo = null;
            DateTime.TryParse(ValidTo.Text, out testDate);
            if (testDate != DateTime.MinValue)
                validTo = testDate;

            if (Id == Guid.Empty)
            {
                Id = new NewsManagement(Global.ConnectionString, SessionProps).StoreNewNews(Header.Text, Body.Text, "", validFrom, validTo);
            }
            else
            {
                new NewsManagement(Global.ConnectionString, SessionProps).UpdateNews(Id, Header.Text, Body.Text, "", validFrom, validTo); 
            }

            EditNewsItems(Id);

            //refresh list
            LoadNews();
        }

        protected void grdAllNews_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                var _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                // Add this javascript to the onclick Attribute of the row

                e.Row.Attributes["onclick"] = _jsSingle;
            }

        }

        protected void grdAllNews_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    EditNewsItems(new Guid(_gridView.SelectedValue.ToString()));
                    break;
            }

        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Id = Guid.Empty;
            pnlNewsEditor.Visible = true;
            Header.Text = "";
            Body.Text = "";
            ValidFrom.Text = DateTime.Now.ToShortDateString();
            ValidTo.Text = "";

            btnTweet.Visible = false;

            newsChangeLog.ClearLog();

        }

        protected void btnTweet_Click(object sender, EventArgs e)
        {
            var url = "http://interntipset.com/News/" + Id.ToString();
            Global.SendTweet(Header.Text, url, SessionProps); 
        }
    }
}
