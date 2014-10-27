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
    public partial class AdminTournamentList : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_TOURMASTER");

            LoadTournaments();
        }

        private void LoadTournaments()
        {
            grdTournaments.DataKeyNames = new string[] { "GUID" };
            grdTournaments.DataSource = new CommonDataSources(Global.ConnectionString, SessionProps).AllTournaments();
            grdTournaments.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdTournaments.Rows)
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

        protected void grdTournaments_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void grdTournaments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    WebControlManager.RedirectWithQueryString("AdminTournamentEdit.aspx", new string[]{"GUID"}, new string[]{_gridView.SelectedValue.ToString()});
                    break;
            }
        }

        protected void btnAddNewTournament_Click(object sender, EventArgs e)
        {
            WebControlManager.ClearRedirectCache();
            Response.Redirect("AdminTournamentEdit.aspx");
        }
    }
}
