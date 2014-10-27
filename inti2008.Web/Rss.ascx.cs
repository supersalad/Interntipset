using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace inti2008.Web
{
    public partial class Rss : System.Web.UI.UserControl
    {
        #region Public Properties
        public string Url
        {
            get { return RssData.DataFile; }
            set { RssData.DataFile = value; }
        }

        public bool ShowDescription
        {
            get
            {
                object showDesc = ViewState["ShowDescription"];
                if (showDesc != null)
                    return (bool)showDesc;
                else
                    return true;    // By default, show the description
            }
            set { ViewState["ShowDescription"] = value; }
        }

        public TableItemStyle ItemStyle
        {
            get { return RssOutput.ItemStyle; }
        }

        public TableItemStyle AlternatingItemStyle
        {
            get { return RssOutput.AlternatingItemStyle; }
        }
        #endregion


        protected void RssOutput_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Reference the various controls
                HyperLink lnkTitle = e.Item.FindControl("lnkTitle") as HyperLink;
                Label lblPubDate = e.Item.FindControl("lblPubDate") as Label;
                Label lblDescription = e.Item.FindControl("lblDescription") as Label;

                // Show the pub date, if it exists
                object dateVal = XPathBinder.Eval(e.Item.DataItem, "pubDate");
                if (dateVal != null && dateVal.ToString().Length > 0)
                    lblPubDate.Text = string.Concat("(", Convert.ToDateTime(dateVal).ToShortDateString(), ")");
                else
                    lblPubDate.Text = string.Empty;

                // Show or hide the description as needed
                lblDescription.Visible = ShowDescription;

            }
        }
    }
}