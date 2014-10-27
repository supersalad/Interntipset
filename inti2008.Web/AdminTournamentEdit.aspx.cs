using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminTournamentEdit : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_TOURMASTER");

            pnlEditTransferWindow.Visible = false;

            if (!IsPostBack)
                LoadTournament();

            pnlClubs.Visible = (TourId != Guid.Empty);
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

        private void LoadTournament()
        {
            if (TourId != Guid.Empty)
            {
                //get this tournament
                using (var db = Global.GetConnection())
                {
                    var tour = db.Inti_Tournament.Single(t => t.GUID == TourId);

                    TournamentName.Text = tour.Name;
                    TournamentDescription.Text = tour.Description;
                    PublicateDate.Text = tour.PublicateDate.ToString();
                    StartRegistration.Text = tour.StartRegistration.ToString();
                    EndRegistration.Text = tour.EndRegistration.ToString();
                    NmbrOfClubs.Text = tour.NmbrOfClubs.ToString();
                    NmbrOfDays.Text = tour.NmbrOfDays.ToString();
                    NmbrOfTransfers.Text = tour.NmbrOfTransfers.ToString();
                    Budget.Text = tour.Budget.ToString();
                    IncludeManager.Checked = tour.IncludeManager;

                    //get clubs if any

                    var clubs = from c in db.Inti_Club
                                where c.TournamentGUID == TourId
                                select c;

                    grdClubs.DataKeyNames = new string[]{"GUID"};
                    grdClubs.DataSource = clubs.OrderBy(c => c.Name).ToList();
                    grdClubs.DataBind();

                    LoadTransferWindows();
                }
            }
        }

        private void LoadTransferWindows()
        {
            using (var db = Global.GetConnection())
            {
                var transferWindows = db.Inti_TransferPeriod.Where(tf => tf.TournamentGUID == TourId);

                grdTransferPeriods.DataKeyNames = new string[]{"GUID"};
                grdTransferPeriods.DataSource = transferWindows.OrderBy(tf => tf.StartDate).ToList();
                grdTransferPeriods.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (TourId != Guid.Empty)
            {
                //update this tournament
                using (var db = Global.GetConnection())
                {
                    var tour = db.Inti_Tournament.Single(t => t.GUID == TourId);

                    tour.Name =TournamentName.Text;
                    tour.Description = TournamentDescription.Text;
                    tour.PublicateDate = DateTime.Parse(PublicateDate.Text);
                    tour.StartRegistration = DateTime.Parse(StartRegistration.Text);
                    tour.EndRegistration = DateTime.Parse(EndRegistration.Text);
                    tour.NmbrOfClubs = int.Parse(NmbrOfClubs.Text);
                    tour.NmbrOfDays = int.Parse(NmbrOfDays.Text);
                    tour.NmbrOfTransfers = int.Parse(NmbrOfTransfers.Text);
                    tour.Budget = int.Parse(Budget.Text);
                    tour.IncludeManager = IncludeManager.Checked;

                    db.SubmitChanges();
                }

            }
            else
            {
                using (var db = Global.GetConnection())
                {
                    var tour = new Inti_Tournament();

                    tour.Name = TournamentName.Text;
                    tour.Description = TournamentDescription.Text;
                    tour.PublicateDate = DateTime.Parse(PublicateDate.Text);
                    tour.StartRegistration = DateTime.Parse(StartRegistration.Text);
                    tour.EndRegistration = DateTime.Parse(EndRegistration.Text);
                    tour.NmbrOfClubs = int.Parse(NmbrOfClubs.Text);
                    tour.NmbrOfDays = int.Parse(NmbrOfDays.Text);
                    tour.NmbrOfTransfers = int.Parse(NmbrOfTransfers.Text);
                    tour.Budget = int.Parse(Budget.Text);
                    tour.IncludeManager = IncludeManager.Checked;

                    db.Inti_Tournament.InsertOnSubmit(tour);

                    //insert rules
                    var headers = new List<string> {"Insats", "Betalning", "Spelarlista", "Budget", "System", "Byten", "Pottfördelning", "Vid samma poäng",
                    "Vid samma poäng II", "Poäng för manager", "Poäng för mål", "Hålla nollan", "Minuspoäng", "Bonuspoäng", "Deadline"};
                    var bodies = new List<string>(){"100 kr exkl. payson-avgifter", "Detta sker uteslutande via payson.se När man lagt till ett lag får man upp en länk som man ska följa. För denna service tar payson en symbolisk avgift som läggs på hundringen.", 
                        "Hämtas från <a target=\"_blank\" href=\"http://www.fantasyleague.com\">www.fantasyleague.com</a>. Vi försöker hålla vår lista uppdaterad så gott det går. Saknar du en viss spelare så flagga upp på forumet om det. Spelaren måste dock vara upplagd på fantasy league för att vi ska agera.", 
                        Budget.Text, "Godkända system är: 3-5-2,3-4-3,4-4-2,4-5-1,4-3-3,5-4-1 eller 5-3-2.", 
                        tour.NmbrOfTransfers + " byten får göras under vissa transferfönster. Dessa annonseras på förstasidan. Även manager får bytas. Det nya laget gäller för alla matcher som spelas efter att transferfönstret stängts.", 
                        "Flest poäng vinner 70% av potten, tvåan får 20%, trean får 5%.", 
                        "Om 2 eller fler spelare slutar på samma poäng är det deltagaren som har spenderat minst pengar på spelarköp som vinner. Vi räknar då med det genomsnittliga priset för alla spelare som ingått i laget under tävlingen, så det spelar ingen roll hur många eller få spelare man byter.",
                    "Om två eller fler lag inte går att skilja med poäng och budget, så delar de lagen på de placeringar som de upptar. Motsvarande pott-pengar delas lika mellan dessa lagen.", 
                    "3p för vinst<br>1p för oavgjort<br>(Då förlängning och straffavgörande förekommer räknas slutresultatet)", 
                    "Målvakt: 7p<br>Back: 6p<br>Mittfältare: 5p<br>Anfallare: 4p<br>Straffmål: 2p (gäller ej straffavgörande)", 
                    "Målvakt: 4p<br>Back: 2p<br>(Man måste ha spelat minst 45 minuter för att få poäng för att hålla nollan)<br>(Då förlängning förekommer räknas bara resultat vid fulltid)", 
                    "Rött kort: -5p<br>Självmål: -3p<br>Missad straff i straffavgörande: -2p", 
                    "För att göra hemsidan lite roligare får du 2 bonuspoäng om du laddar upp en bild på dig själv. Notera att du numera laddar upp en bild per lag.", 
                    "Senast " + tour.EndRegistration.ToShortDateString() + " kl " + tour.EndRegistration.ToString("HH:mm") + " ska ditt lag vara betalt och skickat."};

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var rule = new Inti_TournamentRule();
                        rule.Inti_Tournament = tour;
                        rule.SortOrder = i;
                        rule.Header = headers[i];
                        rule.Body = bodies[i];
                        db.Inti_TournamentRule.InsertOnSubmit(rule);
                    }
                    
                    //add forum category
                    var forumCategory = new Ext_ForumCategory();
                    forumCategory.Inti_Tournament = tour;
                    forumCategory.Name = tour.Name + ", diverse";
                    
                    db.Ext_ForumCategory.InsertOnSubmit(forumCategory);

                    //set penalty miss point event if euro/world cup
                    if (tour.Name.StartsWith("EM") || tour.Name.StartsWith("VM"))
                    {
                        var pointEvent =
                            db.Inti_PointEvent.Single(pe => pe.GUID == new Guid("4E304B11-8937-405A-98DC-72D896F7A89E"));

                        pointEvent.Inti_Tournament = tour;
                    }
                    
                    db.SubmitChanges();    
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminTournamentList.aspx");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdClubs.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            foreach (GridViewRow r in grdTransferPeriods.Rows)
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

        protected void grdClubs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    WebControlManager.RedirectWithQueryString("AdminEditClub.aspx", new string[]{"clubGUID", "tourGUID"}, new string[]{_gridView.SelectedValue.ToString(), TourId.ToString()} );
                    break;
            }
        }

        protected void grdClubs_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void btnAddNewClub_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("AdminEditClub.aspx", new string[]{"tourGUID"}, new string[] {TourId.ToString()});
        }

        protected void btnGenerateMatches_Click(object sender, EventArgs e)
        {
            using (var db = Global.GetConnection())
            {
                //delete any matches fropm before, if any have been updated. Skip this action
                var checkUpdatedMatches = from m in db.Inti_Match
                                          where m.TournamentGUID == TourId
                                          select m;

                if (checkUpdatedMatches.ToList().Count > 0)
                {
                    lblMatchEditMessage.Text =
                        "Det finns redan matcher i denna tävlingen. Du får editera matcherna manuellt.";

                    return;
                }
                

                //delete all matches in this tournament
                //var matchesToDelete = db.Inti_Match.Where(m => m.TournamentGUID == TourId);

                //foreach(var match in matchesToDelete.ToList())
                //{
                //    db.Inti_Match.DeleteOnSubmit(match);
                //}

                //db.SubmitChanges();

                //loop the clubs of the tournament
                var clubs = db.Inti_Club.Where(c => c.TournamentGUID == TourId);

                var clubs1 = clubs.ToList();
                var clubs2 = clubs.ToList();

                foreach(var homeClub in clubs1)
                {
                    foreach(var awayClub in clubs2)
                    {
                        if (homeClub.GUID != awayClub.GUID)
                        {
                            var match = new Inti_Match();

                            match.TournamentGUID = TourId;
                            match.HomeClub = homeClub.GUID;
                            match.AwayClub = awayClub.GUID;
                            match.MatchDate = DateTime.Parse(EndRegistration.Text);
                            match.TourDay = 1;
                            match.IsUpdated = false;

                            db.Inti_Match.InsertOnSubmit(match);    
                        }
                    }
                }

                db.SubmitChanges();
            }
        }

        protected void btnEditMatches_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("AdminEditMatch.aspx", new string[] { "GUID" }, new string[] { TourId.ToString() });
        }

        protected void btnAddTransferWindow_Click(object sender, EventArgs e)
        {
            //kontrollera att det inte finns fler transferfönster änvad som angetts i turneringen
            using (var db = Global.GetConnection())
            {
                var transferWindows = db.Inti_TransferPeriod.Where(tf => tf.TournamentGUID == TourId);

                var tour = db.Inti_Tournament.Single(t => t.GUID == TourId);

                var transferWindow = new Inti_TransferPeriod();
                transferWindow.TournamentGUID = TourId;
                transferWindow.StartDate = tour.StartRegistration;
                transferWindow.EndDate = tour.StartRegistration;
                transferWindow.Name = "Nytt transferfönster";
                transferWindow.Description = "";

                db.Inti_TransferPeriod.InsertOnSubmit(transferWindow);

                db.SubmitChanges();

                LoadTransferWindows();

                LoadTransferWindow(transferWindow.GUID);                

            }
        }

        protected void grdTransferPeriods_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void grdTransferPeriods_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    LoadTransferWindow(new Guid(_gridView.SelectedValue.ToString()));
                    break;
            }
        }

        private void LoadTransferWindow(Guid guid)
        {
            using (var db = Global.GetConnection())
            {
                var transferWindow = db.Inti_TransferPeriod.Single(tf => tf.GUID == guid);

                txtTransferWindowName.Text = transferWindow.Name;
                txtTransferWindowDescription.Text = transferWindow.Description;
                txtTransferWindowStartDate.Text = transferWindow.StartDate.ToString();
                txtTransferWindowEndDate.Text = transferWindow.EndDate.ToString();

                ViewState["transferPeriodGUID"] = guid.ToString();

                pnlEditTransferWindow.Visible = true;
            }
        }

        protected void btnSaveTransferWindow_Click(object sender, EventArgs e)
        {
            var guid = new Guid(ViewState["transferPeriodGUID"].ToString());
            using (var db = Global.GetConnection())
            {
                var transferWindow = db.Inti_TransferPeriod.Single(tf => tf.GUID == guid);

                transferWindow.Name = txtTransferWindowName.Text;
                transferWindow.Description = txtTransferWindowDescription.Text;
                transferWindow.StartDate = DateTime.Parse(txtTransferWindowStartDate.Text);
                transferWindow.EndDate = DateTime.Parse(txtTransferWindowEndDate.Text);

                db.SubmitChanges();

                LoadTransferWindows();
            }
        }

        protected void btnResetValidFromOnTeams_Click(object sender, EventArgs e)
        {
            using (var db = Global.GetConnection())
            {
                var teams = from team in db.Inti_Team
                            where team.TournamentGUID == TourId
                            select team;
                foreach (var intiTeamVersion in teams.ToList().SelectMany(team => team.Inti_TeamVersion))
                {
                    intiTeamVersion.ValidFrom = DateTime.Parse(EndRegistration.Text);
                }

                db.SubmitChanges();
            }
        }

        protected void btnBatchEditMatches_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("AdminBatchEditMatches.aspx", new string[] { "GUID" }, new string[] { TourId.ToString() });
        }
    }
}
