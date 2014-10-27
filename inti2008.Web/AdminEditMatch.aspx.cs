using System;
using System.Collections.Generic;
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
    public partial class AdminEditMatch : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN");

            if (!IsPostBack)
            {
                LoadMatches();
                InitNewMatchForm();
            }
            
        }

        private void LoadMatches()
        {
            using(var db = Global.GetConnection())
            {
                var matches = from m in db.Inti_Match
                              where m.TournamentGUID == TourId
                              orderby m.Inti_Club.Name
                              orderby m.HomeClubInti_Club.Name
                              orderby m.TourDay
                              select m;

                grdMatches.DataKeyNames = new string[] { "GUID" };
                grdMatches.DataSource = matches.ToList();
                grdMatches.DataBind();
            }
        }

        private Guid TourId
        {
            get
            {
                var value = this.GetRedirectParameter("GUID", false);
                if (value == null)
                    return Guid.Empty;

                return new Guid(value.ToString());
            }
        }

        private IList<Inti_Club> clubsDataSource = null;

        private IList<Inti_Club> ClubDataSource
        {
            get
            {
                if (clubsDataSource == null)
                {
                    using (var db = Global.GetConnection())
                    {
                        var clubs = db.Inti_Club.Where(c => c.TournamentGUID == TourId);

                        clubsDataSource = clubs.OrderBy(c => c.Name).ToList();
                    }    
                }

                return clubsDataSource;
            }
        }

        
        protected void grdMatches_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                ////// load the club dropdowns
                ////var drp = (DropDownList) e.Row.Cells[0].Controls[0];
                ////drp.DataTextField = "Name";
                ////drp.DataValueField = "GUID";
                ////drp.DataSource = ClubDataSource;
                ////drp.DataBind();

                ////drp = (DropDownList)e.Row.Cells[1].Controls[0];
                ////drp.DataTextField = "Name";
                ////drp.DataValueField = "GUID";
                ////drp.DataSource = ClubDataSource;
                ////drp.DataBind();

                //////load day dropdown
                ////drp = (DropDownList) e.Row.Cells[2].Controls[0];
                ////for (var i = 1; i <= 38; i++)
                ////{
                ////    drp.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ////}

            }
        }

        private void InitNewMatchForm()
        {
            //Init hometeam drop down
            drpAddMatchHomeTeam.DataTextField = "Name";
            drpAddMatchHomeTeam.DataValueField = "GUID";
            drpAddMatchHomeTeam.DataSource = ClubDataSource;
            drpAddMatchHomeTeam.DataBind();

            //Init awayteam drop down
            drpAddMatchAwayTeam.DataTextField = "Name";
            drpAddMatchAwayTeam.DataValueField = "GUID";
            drpAddMatchAwayTeam.DataSource = ClubDataSource;
            drpAddMatchAwayTeam.DataBind();

            //init day drop down
            for (var i = 1; i <= 38; i++)
            {
                drpAddMatchTourDay.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

        }

        protected void btnAddMatch_Click(object sender, EventArgs e)
        {
            using(var db = Global.GetConnection())
            {
                var match = new Inti_Match();

                match.HomeClub = new Guid(drpAddMatchHomeTeam.SelectedValue);
                match.AwayClub = new Guid(drpAddMatchAwayTeam.SelectedValue);
                match.TournamentGUID = TourId;
                match.MatchDate = DateTime.Parse(txtAddMatchMatchDate.Text);
                match.TourDay = int.Parse(drpAddMatchTourDay.SelectedValue);
                match.IsUpdated = false;

                db.Inti_Match.InsertOnSubmit(match);

                db.SubmitChanges();
            }

            LoadMatches();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using(var db = Global.GetConnection())
            {
                for (var i = 0; i < grdMatches.Rows.Count; i++)
                {
                    var row = grdMatches.Rows[i];
                    grdMatches.SelectedIndex = i;
                    //is it updated?
                    var newTourDay = int.Parse((row.Cells[2].FindControl("drpDay") as DropDownList).SelectedValue);
                    var orgTourDay = int.Parse((row.Cells[4].FindControl("txtOrgTourDay") as TextBox).Text);
                    var newMatchDate = DateTime.Parse((row.Cells[3].FindControl("txtMatchDate") as TextBox).Text);
                    var orgMatchDate = DateTime.Parse((row.Cells[5].FindControl("txtOrgMatchDate") as TextBox).Text);

                    if (newTourDay != orgTourDay || newMatchDate != orgMatchDate)
                    {
                        var guid = new Guid(grdMatches.SelectedValue.ToString());

                        var match = db.Inti_Match.Single(m => m.GUID == guid);

                        match.HomeClub = new Guid((row.Cells[0].FindControl("drpHomeTeam") as DropDownList).SelectedValue);
                        match.AwayClub = new Guid((row.Cells[1].FindControl("drpAwayTeam") as DropDownList).SelectedValue);
                        match.TourDay = newTourDay;
                        match.MatchDate = newMatchDate;    
                    }
                }
    
                db.SubmitChanges();
            }
            LoadMatches();
            

        }

        
    }
}
