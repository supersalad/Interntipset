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
    public partial class PlayerSelector : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadPlayers();
        }

        private Guid TourId
        {
            get
            {
                var value = this.GetRedirectParameter("tourGUID", false);
                if (value == null)
                    throw new ArgumentNullException("tourGUID","Tournament id must be supplied");
                
                return new Guid(value.ToString());
            }
        }

        private void LoadPlayers()
        {
            var position = this.GetRedirectParameter("position", false);

            using (var db = Global.GetConnection())
            {
                var athletes = from a in db.Inti_AthleteClub
                               where (position == null ? true : a.Inti_Position.ShortName == position.ToString())
                                     && a.Inti_Club.TournamentGUID == TourId
                                     && (a.IsActive ?? false)
                               select new
                                          {
                                              GUID = a.GUID,
                                              PlayerName =
                                   String.Format("{0} {1}", a.Inti_Athlete.FirstName, a.Inti_Athlete.LastName).Trim(),
                                              ClubShortName = a.Inti_Club.ShortName,
                                              Position = a.Inti_Position.ShortName,
                                              Price = a.Price
                                          };

                grdPlayers.DataKeyNames = new string[]{"GUID"};
                grdPlayers.DataSource = athletes.OrderBy(club => club.ClubShortName).ToList();
                grdPlayers.DataBind();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdPlayers.Rows)
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

        protected void grdPlayers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    var teamGUID = this.GetRedirectParameter("teamGUID", false);
                    if (teamGUID != null)
                    {
                        WebControlManager.RedirectWithQueryString(this.GetRedirectParameter("redirecturl", false).ToString(), new string[] { "athleteGUID", "teamGUID" }, new string[] { _gridView.SelectedValue.ToString(), teamGUID.ToString() });    
                    }                    
                    break;
            }
        }

        protected void grdPlayers_RowDataBound(object sender, GridViewRowEventArgs e)
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



    }
}
