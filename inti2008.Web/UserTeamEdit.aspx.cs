using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using inti2008.Data;
using inti2008.Data.Common;

namespace inti2008.Web
{
    public partial class UserTeamEdit : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowTournamentRules();

            //edit or view?
            pnlSavedTeamInfo.Visible = (Tournament.EndRegistration < CurrentDate);
            pnlTeamEdit.Visible = (Tournament.EndRegistration >= CurrentDate);
            var imageDeadline = Tournament.EndRegistration.AddDays(14);
            btnUploadImage.Visible = (imageDeadline >= CurrentDate && TeamId != Guid.Empty);
            if (btnUploadImage.Visible)
            {
                var timeLeftForImage = imageDeadline.Subtract(CurrentDate);
                lblUploadImageInfo.Text = timeLeftForImage.ToFriendlyLocalizedString(false) +
                                          " kvar för att ladda upp bild.";
            }
            else
            {
                lblUploadImageInfo.Text = "Tiden för att ladda upp bild har gått ut";
            }

            //Skickades ett teamGUID - hämta det laget
            if (TeamId == Guid.Empty )
            {
                //new team
                btnActivate.Visible = false;
                btnDeleteTeam.Visible = false;

                pnlSavedTeamInfo.Visible = false;
                pnlSavedLineUp.Visible = false;
                pnlFutureLineUp.Visible = false;
                imgTeamImage.Visible = false;
                pnlTeamTransfers.Visible = false;
            }
            else
            {
                var athleteGUID = this.GetRedirectParameter("athleteGUID", true);
                    if (athleteGUID != null)
                    {
                        //add this player
                        AddPlayer(new Guid(athleteGUID.ToString()));
                    }

                //are we in transferwindow?
                CheckTransferWindow(false);

                LoadTeam();

                //super admin?
                if (SessionProps.HasPermission("ADMIN_SYSTEM"))
                {
                    LoadForSuperAdmin(TeamId);
                }
            }
        }

        private bool? _isInTransferWindow = null;

        private bool IsInOpenTransferWindow
        {
            get
            {
                if (_isInTransferWindow.HasValue)
                    return _isInTransferWindow.Value;

                var tf = new CommonDataFetches(Global.ConnectionString, SessionProps).GetCurrentTransferPeriod(CurrentDate);

                _isInTransferWindow = (tf != null);

                return _isInTransferWindow.Value;
            }

        }


        private DateTime CurrentDate
        {
            get
            {
                DateTime checkDate;
                if (Session["FAKEDATE"] == null || string.IsNullOrEmpty(Session["FAKEDATE"].ToString()) || !DateTime.TryParse(Session["FAKEDATE"].ToString(), out checkDate))
                {
                    return DateTime.Now;
                }

                txtFakeDate.Text = checkDate.ToString("yyyy-MM-dd");
                return checkDate;
            }
        }

        private void CheckTransferWindow(bool undoChanges)
        {

            var tf = new CommonDataFetches(Global.ConnectionString, Global.SessionProperties).GetCurrentTransferPeriod(CurrentDate);
            if (tf != null)
            {
                ShowTransferWindow(tf, undoChanges);
            }

        }

        private void ShowTransferWindow(Inti_TransferPeriod tf, bool undoChanges)
        {

            //show info about the transfer period
            lblTransferPeriodInfo.Text = "Transferfönstret " + tf.Name + " är öppet från och med " +
                                         tf.StartDate.ToString() + " till " + tf.EndDate + ".";
            lblTransferPeriodInfo.Text += "<br>" + tf.Description;


            pnlFutureLineUp.Visible = false;



            //create new team version if needed
            using (var db = Global.GetConnection())
            {
                Inti_TeamVersion transferVersion = null;
                Inti_TeamVersion currentVersion = null;

                GetCurrentAndTransferVersion(db, out currentVersion, out transferVersion);

                if(transferVersion.ValidFrom == null)
                {
                    lblTransferPeriodInfo.Text += "<br><div class=\"redFrame\"><b>" + "Tänk på att du måste klicka på knappen \"Genomför byten\" ";
                    lblTransferPeriodInfo.Text +=
                        "när du är färdiga med dina byten. Annars tror vi bara att du lattjat runt lite.</b></div>";

                    if ((undoChanges || transferVersion.Inti_TeamAthlete.ToList().Count == 0) && currentVersion != null && transferVersion != null)
                    {
                        var trans = new UserTeamManagement(Global.ConnectionString, SessionProps);
                        trans.ReloadTransferVersion(db, transferVersion, currentVersion, TeamId);
                    }

                    //load transfer controls
                    LoadTransferControls(transferVersion, db);

                    pnlTeamTransfers.Visible = true;
                    btnReOpenTransferTeam.Visible = false;
                }
                else
                {
                    lblTransferPeriodInfo.Text +=
                        "<br><br><span class=\"label label-success\">Dina byten är gjorda.</span> Vill du ändra något, klicka på knappen \"Ångra genomförande av byten\"";
                    lblTransferPeriodInfo.Text +=
                        "<br>Tänk på att du då måste klicka på knappen \"Genomför byten\" igen för att bytena ska gå igenom.";



                    pnlTeamTransfers.Visible = false;
                    btnReOpenTransferTeam.Visible = true;
                }

            }

        }



        private void GetCurrentAndTransferVersion(IntiDataContext db, out Inti_TeamVersion currentVersion, out Inti_TeamVersion transferVersion)
        {
            currentVersion = null;
            transferVersion = null;
            var currentDate = CurrentDate;

            var teamVersions = db.Inti_TeamVersion.Where(tv => tv.TeamGUID == TeamId);
            foreach (var teamVersion in teamVersions.OrderByDescending(tv => tv.Version))
            {
                if ((teamVersion.ValidFrom == null || teamVersion.ValidFrom >= currentDate) && teamVersion.ValidTo == null)
                {
                    //we have our version
                    transferVersion = teamVersion;
                }
                if (teamVersion.ValidFrom < currentDate
                    && (teamVersion.ValidTo ?? currentDate).AddDays(1) >= currentDate)
                {
                    currentVersion = teamVersion;
                }
            }

            //create a new team version
            if (transferVersion == null && currentVersion != null)
            {
                transferVersion = new Inti_TeamVersion();
                transferVersion.TeamGUID = TeamId;
                transferVersion.Version = currentVersion.Version + 1;

                db.Inti_TeamVersion.InsertOnSubmit(transferVersion);

                db.SubmitChanges();
            }

            //in tournament startoff, current version and transfer is the same
            currentVersion = currentVersion ?? transferVersion;
        }

        private void LoadTransferControls(Inti_TeamVersion version, IntiDataContext db)
        {
            //insert header
            var tbl = new Table();

            TableRow row;
            TableCell cell;
            Label lbl;
            var posGUID = Guid.Empty;

            foreach(var teamAthlete in db.Inti_TeamAthlete.Where(ta=>ta.TeamVersionGUID == version.GUID).OrderBy(ta=>ta.Inti_AthleteClub.Inti_Position.SortOrder).ToList())
            {
                if(posGUID != teamAthlete.Inti_AthleteClub.PositionGUID)
                {
                    if (posGUID != Guid.Empty )
                    {
                        //add spacer
                        row = new TableRow();
                        cell = new TableCell();
                        cell.ColumnSpan = 4;
                        tbl.Rows.Add(row);
                    }

                    //add header row
                    row = new TableRow();
                    cell = new TableCell();
                    lbl = new Label();

                    lbl.ID = "lblHeader" + teamAthlete.Inti_AthleteClub.Inti_Position.ShortName;
                    lbl.Text = "<strong>" + teamAthlete.Inti_AthleteClub.Inti_Position.Name + "</strong>";

                    cell.Controls.Add(lbl);
                    cell.ColumnSpan = 4;
                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);

                    posGUID = teamAthlete.Inti_AthleteClub.PositionGUID;
                }

                //add controls for the player
                row = new TableRow();

                //player name
                cell = new TableCell();
                lbl = new Label();
                lbl.Text = String.Format("{0} {1}", teamAthlete.Inti_AthleteClub.Inti_Athlete.FirstName,
                                         teamAthlete.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                cell.Controls.Add(lbl);
                row.Cells.Add(cell);

                //player club
                cell = new TableCell();
                lbl = new Label();
                lbl.Text = teamAthlete.Inti_AthleteClub.Inti_Club.ShortName;
                cell.Controls.Add(lbl);
                row.Cells.Add(cell);

                //player price
                cell = new TableCell();
                lbl = new Label();
                lbl.Text = teamAthlete.Inti_AthleteClub.Price.ToString();
                cell.Controls.Add(lbl);
                row.Cells.Add(cell);

                //remove-button
                cell = new TableCell();
                var btnDel = new Button();
                btnDel.ID = "btnDelete" + teamAthlete.AthleteGUID.ToString();
                btnDel.CssClass = "btn btn-danger";
                btnDel.Text = "ta bort";
                btnDel.ToolTip = "ta bort " + String.Format("{0} {1}", teamAthlete.Inti_AthleteClub.Inti_Athlete.FirstName,
                                                            teamAthlete.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                btnDel.Click += DeletePlayer_Click;
                cell.Controls.Add(btnDel);
                row.Cells.Add(cell);

                //add this row
                tbl.Rows.Add(row);
            }

            var positions =
                    db.Inti_Position.Where(p => (p.ShortName != "MGR" || SessionProps.SelectedTournament.IncludeManager))
                        .OrderBy(p => p.SortOrder).ToList();

            Button btnAddNew = null;
            foreach (var pos in positions)
            {
                btnAddNew = new Button();
                btnAddNew.ID = "btnAdd" + pos.ShortName;
                btnAddNew.CssClass = "btn btn-primary";
                btnAddNew.Text = "lägg till " + pos.Name;
                btnAddNew.ToolTip = "lägg till en " + pos.Name;
                btnAddNew.Click += new EventHandler(AddPlayer_Click);

                //add button to add a new player
                row = new TableRow();
                cell = new TableCell();
                cell.ColumnSpan = 4;

                cell.Controls.Add(btnAddNew);
                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }

            plhTransferLineUp.Controls.Clear();
            plhTransferLineUp.Controls.Add(tbl);
        }

        private Guid TeamId
        {
            get
            {
                if (ViewState["teamGUID"] != null)
                    return new Guid(ViewState["teamGUID"].ToString());

                var value = this.GetRedirectParameter("teamGUID", false);
                if (value == null)
                    return Guid.Empty;
                else
                    return new Guid(value.ToString());
            }
            set
            {
                ViewState["teamGUID"] = value;
            }
        }

        private Guid TeamVersionId
        {
            get
            {
                //get most recent version
                using (var db = Global.GetConnection())
                {
                    var teamVersions = from tv in db.Inti_TeamVersion
                                       where tv.TeamGUID == TeamId
                                       select tv;

                    Inti_TeamVersion mostRecentVersion;
                    if (IsInOpenTransferWindow)
                    {
                        mostRecentVersion = teamVersions.OrderByDescending(tv => tv.Version).ToList().First();
                    }
                    else
                    {
                        if (teamVersions.Count() > 1)
                            mostRecentVersion = teamVersions.OrderByDescending(tv => tv.Version).ToList()[1];
                        else
                            mostRecentVersion = teamVersions.OrderByDescending(tv => tv.Version).ToList().First();
                    }


                    ViewState["CurrentVersion"] = mostRecentVersion.GUID;

                    return mostRecentVersion.GUID;
                }

            }
        }

        private Inti_Tournament _tournament;
        private Inti_Tournament Tournament
        {
            get
            {
                if (_tournament != null)
                {
                    return _tournament;
                }

                var value = this.GetRedirectParameter("tourGUID",false);
                if (value == null)
                {
                    value = this.GetRedirectParameter("teamGUID", false);
                    if (value == null)
                        return SessionProps.DefaultTournament;
                    else
                    {
                        //get tournament of the team
                        using (var db = Global.GetConnection())
                        {
                            var team = db.Inti_Team.Single(t => t.GUID == TeamId);
                            _tournament = team.Inti_Tournament;
                            return _tournament;
                        }
                    }

                }


                using (var db = Global.GetConnection())
                {
                    _tournament =db.Inti_Tournament.Single(t => t.GUID == new Guid(value.ToString()));
                    return _tournament;
                }
            }
        }

        private void ShowTournamentRules()
        {
            lblTournamentInfo.Text = "<h5>" + Tournament.Name + "</h5>";
            lblTournamentInfo.Text += "<strong>Budget: </strong>" + Tournament.Budget.ToString() + "<br>";
            lblTournamentInfo.Text += "<strong>Deadline för nya lag: </strong>" + Tournament.EndRegistration.ToShortDateString() + " " + Tournament.EndRegistration.ToShortTimeString();
        }

        private void LoadTeam()
        {
            if (TeamId != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);

                    //verify team owner
                    if (team.Sys_User.GUID != SessionProps.UserGuid && !SessionProps.HasPermission("ADMIN"))
                    {
                        //log the attempted breach
                        MailAndLog.SendMessage("Försök att sabba lag",
                            String.Format("Användaren: {0} med guid: {1} försökte öppna laget: {2} med guid: {3}", SessionProps.UserName, SessionProps.UserGuid.ToString(), team.Name, team.GUID),
                            Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                        throw new AccessViolationException("Attempt to open other users team");
                    }


                    lblTeamName.Text = team.Name;
                    lblTeamDescription.Text = team.Presentation;

                    lblNmbrOfTransfers.Text = team.Inti_TeamTransfer.ToList().Count.ToString() + " av " +
                                              SessionProps.SelectedTournament.NmbrOfTransfers.ToString() +
                                              " byten gjorda.";

                    //load input boxes
                    if (!IsPostBack)
                    {
                        imgTeamImage.Visible = false;

                        TeamName.Text = team.Name;
                        TeamDescription.Text = team.Presentation;

                        if (!String.IsNullOrEmpty(team.Picture))
                        {
                            imgTeamImage.ImageUrl = "~/img/user/" + team.Picture;
                            imgTeamImage.Visible = true;
                        }

                    }
                    Inti_TeamVersion currentVersion;
                    Inti_TeamVersion transferVersion;

                    GetCurrentAndTransferVersion(db, out currentVersion, out transferVersion);

                    var lineUp = from ta in db.Inti_TeamAthlete
                                 where ta.Inti_TeamVersion.GUID == currentVersion.GUID
                                 select ta;

                    //var LineUpTransfer = new Dictionary<string, List<Inti_TeamAthlete>>();
                    IQueryable<Inti_TeamAthlete> lineUpTransfer = null;
                    pnlFutureLineUp.Visible = false;
                    if (transferVersion != null)
                    {
                        //put the players in a organized list
                        lineUpTransfer = from ta in db.Inti_TeamAthlete
                                             where ta.Inti_TeamVersion.GUID == transferVersion.GUID
                                             select ta;

                        lblNextLineUp.Text = GetLineUpAsString(db, lineUpTransfer);
                        if (transferVersion.ValidFrom != null)
                            lblNextLineUpValid.Text = String.Format("<p>Börjar gälla: {0}</p>",
                                                                    transferVersion.ValidFrom.Value.ToShortDateString());

                        if (!IsInOpenTransferWindow)
                        {
                            lblNextLineUpValid.Text =
                                String.Format(
                                    "<p>Byten ej genomförda under förra transferfönstret. Kan ändras eller genomföras vid nästa fönster.</p>");
                        }

                        pnlFutureLineUp.Visible = (transferVersion.ValidFrom != null && lineUpTransfer.ToList().Count > 0);
                    }


                    lblLineUp.Text = GetLineUpAsString(db, lineUp);

                    var listLineUp = GetLineUpAsList(lineUp);

                    LoadTeamForEdit(listLineUp);

                    if(transferVersion != null && lineUpTransfer != null && IsInOpenTransferWindow)
                    {
                        ValidateTeam(GetLineUpAsList(lineUpTransfer));
                    }
                    else
                    {
                        ValidateTeam(listLineUp);
                    }

                    //is activated but not valid?
                    if (!btnActivate.Visible && (team.IsActive ?? false)  && pnlTeamEdit.Visible)
                    {
                        team.IsActive = false;
                        db.SubmitChanges();
                        btnActivate.Visible = true;
                    }


                    //if team is activated - no need to do it again
                    if (btnActivate.Visible && (team.IsActive ?? false))
                        btnActivate.Visible = false;

                    ShowPaymentInfo(team);
                }

                //don't show current and future line up unless we're in a transfer window
                pnlSavedLineUp.Visible = (Tournament.EndRegistration < CurrentDate);
                pnlFutureLineUp.Visible = (Tournament.EndRegistration < CurrentDate);

                teamChangeLog.LoadChangeLog(TeamId);
            }
        }

        private Dictionary<string, List<Inti_TeamAthlete>> GetLineUpAsList(IQueryable<Inti_TeamAthlete> lineUp)
        {
            var returnList = new Dictionary<string, List<Inti_TeamAthlete>>();
            foreach (var athlete in lineUp.OrderBy(a => a.Inti_AthleteClub.Inti_Position.ShortName))
            {
                if (!returnList.ContainsKey(athlete.Inti_AthleteClub.Inti_Position.ShortName))
                    returnList.Add(athlete.Inti_AthleteClub.Inti_Position.ShortName, new List<Inti_TeamAthlete>());

                returnList[athlete.Inti_AthleteClub.Inti_Position.ShortName].Add(athlete);
            }

            return returnList;
        }

        private string GetLineUpAsString(IntiDataContext db, IQueryable<Inti_TeamAthlete> lineUp)
        {
            //put the players in a organized list
            var LineUp = new Dictionary<string, List<Inti_TeamAthlete>>();

            if (lineUp.ToList().Count > 0)
            {
                foreach (var athlete in lineUp.OrderBy(a => a.Inti_AthleteClub.Inti_Position.ShortName))
                {
                    if (!LineUp.ContainsKey(athlete.Inti_AthleteClub.Inti_Position.ShortName))
                        LineUp.Add(athlete.Inti_AthleteClub.Inti_Position.ShortName, new List<Inti_TeamAthlete>());

                    LineUp[athlete.Inti_AthleteClub.Inti_Position.ShortName].Add(athlete);
                }
            }

            var returnString = "";

            //print manager
            if (LineUp.ContainsKey("MGR"))
            {
                returnString += GetAthletesInString(LineUp["MGR"]);
            }

            //print goalkeeper
            if (LineUp.ContainsKey("GK"))
            {
                returnString += GetAthletesInString(LineUp["GK"]);
            }

            //print defenders
            if (LineUp.ContainsKey("D"))
            {
                returnString += GetAthletesInString(LineUp["D"]);
            }

            //print midfielders
            if (LineUp.ContainsKey("M"))
            {
                returnString += GetAthletesInString(LineUp["M"]);
            }
            //print strikers
            if (LineUp.ContainsKey("S"))
            {
                returnString += GetAthletesInString(LineUp["S"]);
            }

            return returnString;
        }

        private void ShowPaymentInfo(Inti_Team team)
        {
            if(team.IsPaid ?? false)
            {
                //informera om att laget är betalt
                lblPaymentInfo.Text = String.Format("<p>Vi har registrerat din betalning och {0} är med i matchen</p>",
                                                    team.Name);
            }
            else
            {
                //generera betallänk
                var paySonLink =
                    String.Format(
                        "https://www.payson.se//SendMoney/?De={0}&Se=jacizd%40hotmail.com&Cost=100%2c00&ShippingAmount=0%2c00&Gr=1",
                        Server.UrlEncode(team.GUID.ToString()));

                var payPalLink =
                    String.Format("<form action=\"https://www.paypal.com/cgi-bin/webscr\" method=\"post\">" +
                                  "<input type=\"hidden\" name=\"cmd\" value=\"_xclick\">" +
                                  "<input type=\"hidden\" name=\"business\" value=\"folkesson@gmail.com\">" +
                                  "<input type=\"hidden\" name=\"item_name\" value=\"{0}\">" +
                                  "<input type=\"hidden\" name=\"item_number\" value=\"{1}\">" +
                                  "<input type=\"hidden\" name=\"amount\" value=\"100.00\">" +
                                  "<input type=\"hidden\" name=\"shipping\" value=\"0.00\">" +
                                  "<input type=\"hidden\" name=\"no_shipping\" value=\"1\">" +
                                  "<input type=\"hidden\" name=\"no_note\" value=\"1\">" +
                                  "<input type=\"hidden\" name=\"currency_code\" value=\"SEK\">" +
                                  "<input type=\"hidden\" name=\"tax\" value=\"0.00\">" +
                                  "<input type=\"hidden\" name=\"lc\" value=\"SE\">" +
                                  "<input type=\"hidden\" name=\"bn\" value=\"PP-BuyNowBF\">" +
                                  "<input type=\"image\" src=\"https://www.paypal.com/en_US/i/btn/btn_paynow_SM.gif\" border=\"0\" name=\"submit\" alt=\"PayPal - The safer, easier way to pay online!\">" +
                                  "<img alt=\"\" border=\"0\" src=\"https://www.paypal.com/en_US/i/scr/pixel.gif\" width=\"1\" height=\"1\">" +
                                  "</form>", team.Name, team.GUID);

                var payPalLink2 =
                    String.Format(
                        "https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=folkesson@gmail.com&item_name={0}&item_number={1}&amount=100.00&shipping=0.00&no_shipping=1&no_note=1&currency_code=SEK&tax=0.00&lc=SE&bn=PP-BuyNowBF",
                        team.Name, team.GUID);


                //payson och paypal
                //lblPaymentInfo.Text =
                //    String.Format(
                //        "<p>Betala för ditt lag</p><p>När du betalt kommer ditt lag markeras som betalt på denna sidan, och betallänken försvinner. Eftersom detta kräver manuellt ingripande från den ideellt arbetande personalen kan det ta ett par dagar.</p><p><a target=\"_blank\" href=\"{0}\">Klicka här för att betala via payson</a></p><p>Eller klicka på knappen nedan för att betala via Paypal</p><p><a target=\"_blank\" href=\"{1}\"><img border=\"0\" src=\"https://www.paypal.com/en_US/i/btn/btn_paynow_SM.gif\" /></a></p>",
                //        paySonLink, payPalLink2);

                //Payson only
                //lblPaymentInfo.Text = String.Format("<p>Betala för ditt lag</p><p>När du betalt kommer ditt lag markeras som betalt på denna sidan, och betallänken försvinner. Eftersom detta kräver manuellt ingripande från den ideellt arbetande personalen kan det ta ett par dagar.</p><p><a target=\"_blank\" rel=\"noreferrer\" href=\"{0}\">Klicka här för att betala via payson</a></p>",paySonLink);

                //lblPaymentInfo.Text = "Betallösning ej klar, kom tillbaka om någon dag...";

                //just text
                string swishInstructions = "Betala genom att swisha 100 kr till 0704898009 (Jacek Izdebski). Ange lagnamnet i meddelandet. (Tänk på att inte byta lagnamn efter att du swishat!)";
                lblPaymentInfo.Text = swishInstructions;
            }
        }

        private void ValidateTeam(Dictionary<string, List<Inti_TeamAthlete>> lineUp)
        {
            var nmbrOfManagers = 0;
            var nmbrOfGoalkeepers = 0;
            var nmbrOfDefenders = 0;
            var nmbrOfMidfielders = 0;
            var nmbrOfStrikers = 0;
            var nmbrOfPlayersPerClub = new Dictionary<string, int>();

            var teamBudget = 0;


            //get info from team
            foreach(var pos in lineUp)
            {
                //loop the players
                foreach(var teamAthlete in pos.Value)
                {
                    //increase position counters
                    switch(pos.Key)
                    {
                        case "MGR":
                            nmbrOfManagers++;
                            break;
                        case "GK" :
                            nmbrOfGoalkeepers++;
                            break;
                        case "D" :
                            nmbrOfDefenders++;
                            break;
                        case "M" :
                            nmbrOfMidfielders++;
                            break;
                        case "S" :
                            nmbrOfStrikers++;
                            break;
                    }

                    //increase budget
                    teamBudget += teamAthlete.Inti_AthleteClub.Price ?? 0;

                    //increase players by club
                    if(!nmbrOfPlayersPerClub.ContainsKey(teamAthlete.Inti_AthleteClub.Inti_Club.Name))
                        nmbrOfPlayersPerClub.Add(teamAthlete.Inti_AthleteClub.Inti_Club.Name, 0);

                    nmbrOfPlayersPerClub[teamAthlete.Inti_AthleteClub.Inti_Club.Name]++;
                }
            }



            //validate info
            //ok line up?
            var teamOkToApprove = true;
            var teamInfo = "";
            var lineUpCount = 11;
            if (Tournament.IncludeManager)
                lineUpCount = 12;

            //total number of players ok?
            if ((nmbrOfManagers + nmbrOfGoalkeepers + nmbrOfDefenders + nmbrOfMidfielders + nmbrOfStrikers) != lineUpCount)
            {
                teamInfo += String.Format("<p>Antal spelare ska vara {0}</p>", lineUpCount);
                teamOkToApprove = false;
            }
            else
            {
                //line up ok?
                if ((lineUpCount - nmbrOfManagers) < 11)
                {
                    teamInfo += String.Format("<p>Max en manager</p>");
                    teamOkToApprove = false;
                }
                if ((lineUpCount - nmbrOfManagers) > 11)
                {
                    teamInfo += String.Format("<p>Du måste välja en manager</p>");
                    teamOkToApprove = false;
                }
                if (nmbrOfGoalkeepers == 0)
                {
                    teamInfo += String.Format("<p>Du måste välja en målvakt</p>");
                    teamOkToApprove = false;
                }
                if (nmbrOfGoalkeepers > 1)
                {
                    teamInfo += String.Format("<p>Max en målvakt</p>");
                    teamOkToApprove = false;
                }
                //3-5-2,3-4-3
                if (nmbrOfDefenders == 3 )
                {
                    if (!(nmbrOfMidfielders == 5 && nmbrOfStrikers == 2 || nmbrOfMidfielders == 4 && nmbrOfStrikers == 3))
                    {
                        teamInfo += String.Format("<p>{0}-{1}-{2} är ingen godkänd uppställning</p>", nmbrOfDefenders, nmbrOfMidfielders, nmbrOfStrikers);
                        teamOkToApprove = false;
                    }
                }

                //4-4-2,4-5-1,4-3-3
                if (nmbrOfDefenders == 4)
                {
                    if (!(nmbrOfMidfielders == 4 && nmbrOfStrikers == 2 ||
                        nmbrOfMidfielders == 5 && nmbrOfStrikers == 1 ||
                        nmbrOfMidfielders == 3 && nmbrOfStrikers == 3))
                    {
                        teamInfo += String.Format("<p>{0}-{1}-{2} är ingen godkänd uppställning</p>", nmbrOfDefenders, nmbrOfMidfielders, nmbrOfStrikers);
                        teamOkToApprove = false;
                    }
                }

                //5-4-1,5-3-2
                if (nmbrOfDefenders == 5)
                {
                    if (!(nmbrOfMidfielders == 4 && nmbrOfStrikers == 1 ||
                        nmbrOfMidfielders == 3 && nmbrOfStrikers == 2))
                    {
                        teamInfo += String.Format("<p>{0}-{1}-{2} är ingen godkänd uppställning</p>", nmbrOfDefenders, nmbrOfMidfielders, nmbrOfStrikers);
                        teamOkToApprove = false;
                    }
                }
            }

            //number of players from the same club ok?
            //foreach (var club in nmbrOfPlayersPerClub)
            //{
            //    if (club.Value > 3)
            //    {
            //        teamInfo += String.Format("<p>{0} spelare från {1}, det får max vara 3 från samma lag</p>", club.Value, club.Key);
            //        teamOkToApprove = false;
            //    }
            //}

            //budget ok?
            if (teamBudget > Tournament.Budget)
            {
                teamInfo += String.Format("<p>Du har överskridit budgeten</p>");
                teamOkToApprove = false;
            }

            lblTeamSystem.Text = String.Format("Uppställning: {0}-{1}-{2}", nmbrOfDefenders, nmbrOfMidfielders,
                                          nmbrOfStrikers);
            lblTeamBudget.Text = String.Format("Budget: {0}/{1}", teamBudget, Tournament.Budget);

            lblLineUpInfo.Text = lblTeamSystem.Text + "<br>" + lblTeamBudget.Text;

            //number of transfers ok?
            if (pnlTeamTransfers.Visible)
            {
                //compare transfer version with current version - how many trades are done?
                using (var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);
                    var totalNmbrOfTransfers = team.Inti_TeamTransfer.ToList().Count;

                    Inti_TeamVersion currentVersion;
                    Inti_TeamVersion transferVersion;

                    GetCurrentAndTransferVersion(db, out currentVersion, out transferVersion);

                    var transferOuts = 0;
                    foreach(var teamAthlete in currentVersion.Inti_TeamAthlete.ToList())
                    {
                        //is this athlete removed in the new version?
                        if (transferVersion.Inti_TeamAthlete.Where(ta => ta.AthleteGUID == teamAthlete.AthleteGUID).ToList().Count == 0)
                        {
                            transferOuts += 1;
                            lblNmbrOfTransfers.Text += String.Format("<br>ut: {0} {1}", teamAthlete.Inti_AthleteClub.Inti_Athlete.FirstName, teamAthlete.Inti_AthleteClub.Inti_Athlete.LastName);
                        }

                    }

                    var transferIns = 0;
                    foreach (var teamAthlete in transferVersion.Inti_TeamAthlete.ToList())
                    {
                        //is this athlete present in the old version?
                        if (currentVersion.Inti_TeamAthlete.Where(ta => ta.AthleteGUID == teamAthlete.AthleteGUID).ToList().Count == 0)
                        {
                            transferIns += 1;
                            lblNmbrOfTransfers.Text += String.Format("<br>in: {0} {1}", teamAthlete.Inti_AthleteClub.Inti_Athlete.FirstName, teamAthlete.Inti_AthleteClub.Inti_Athlete.LastName);
                        }

                    }

                    if (transferIns != transferOuts)
                    {
                        teamInfo += "<p>Du måste ta in lika många spelare som du byter ut</p>";
                        teamOkToApprove = false;
                    }
                    else
                    {
                        totalNmbrOfTransfers += transferIns;
                        if (totalNmbrOfTransfers > SessionProps.SelectedTournament.NmbrOfTransfers)
                        {
                            teamInfo += String.Format("<p>Du får max byta {0} spelare</p>",
                                                      SessionProps.SelectedTournament.NmbrOfTransfers.ToString());
                            teamOkToApprove = false;
                        }
                    }
                    //anything in the transfers to undo?
                    btnUndoTransfers.Visible = (transferIns > 0 || transferOuts > 0);
                }

            }

            btnActivate.Visible = teamOkToApprove;
            btnCommitTransfers.Visible = teamOkToApprove;

            if (!teamOkToApprove)
                lblMessage.Text = "<div class='alert alert-danger'>" + teamInfo + "</div>";
            else
                lblMessage.Text = "";

        }

        private void LoadTeamForEdit(Dictionary<string, List<Inti_TeamAthlete>> lineUp)
        {
            if ((Tournament.EndRegistration >= CurrentDate && Tournament.StartRegistration <= CurrentDate))
            {
                //ok to edit team, load controls
                using (var db = Global.GetConnection())
                {
                    //empty placeholder
                    plhLineUpControls.Controls.Clear();
                    lblLineUpAll.Text = "";

                    var positions = db.Inti_Position.ToList();

                    //include managers?
                    if (Tournament.IncludeManager)
                    {
                        foreach(var position in positions)
                        {
                            if (position.ShortName == "MGR")
                            {
                                //GenerateControlsForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("MGR") ? lineUp["MGR"] : null), plhLineUpControls);
                                lblLineUpAll.Text += GenerateHtmlForPosition(position.ShortName, position.Name,
                                    (lineUp.ContainsKey("MGR") ? lineUp["MGR"] : null), plhLineUpControls);
                                break;
                            }
                        }
                    }
                    //goalkeeper
                    foreach (var position in positions)
                    {
                        if (position.ShortName == "GK")
                        {
                            //GenerateControlsForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("GK") ? lineUp["GK"] : null), plhLineUpControls);
                            lblLineUpAll.Text += GenerateHtmlForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("GK") ? lineUp["GK"] : null), plhLineUpControls);
                            break;
                        }
                    }

                    //defenders
                    foreach (var position in positions)
                    {
                        if (position.ShortName == "D")
                        {
                            //GenerateControlsForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("D") ? lineUp["D"] : null), plhLineUpControls);
                            lblLineUpAll.Text += GenerateHtmlForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("D") ? lineUp["D"] : null), plhLineUpControls);
                            break;
                        }
                    }

                    //midfielders
                    foreach (var position in positions)
                    {
                        if (position.ShortName == "M")
                        {
                            //GenerateControlsForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("M") ? lineUp["M"] : null), plhLineUpControls);
                            lblLineUpAll.Text += GenerateHtmlForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("M") ? lineUp["M"] : null), plhLineUpControls);
                            break;
                        }
                    }

                    //strikers
                    foreach (var position in positions)
                    {
                        if (position.ShortName == "S")
                        {
                            //GenerateControlsForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("S") ? lineUp["S"] : null), plhLineUpControls);
                            lblLineUpAll.Text += GenerateHtmlForPosition(position.ShortName, position.Name, (lineUp.ContainsKey("S") ? lineUp["S"] : null), plhLineUpControls);
                            break;
                        }
                    }

                }


            }
        }

        private void GenerateControlsForPosition(string positionShortName, string positionLongName, List<Inti_TeamAthlete> listOfAthletes, PlaceHolder container)
        {
            //insert header
            var tbl = new Table();

            var row = new TableRow();
            var cell = new TableCell();
            var lbl = new Label();
            lbl.ID = "lblHeader" + positionShortName;
            lbl.Text = "<strong>" + positionLongName + "</strong>";

            cell.Controls.Add(lbl);
            cell.ColumnSpan = 4;
            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            //add control to delete?
            if (listOfAthletes != null)
            {
                foreach(var ta in listOfAthletes)
                {
                    row = new TableRow();

                    //player name
                    cell = new TableCell();
                    lbl = new Label();
                    lbl.Text = String.Format("{0} {1}", ta.Inti_AthleteClub.Inti_Athlete.FirstName,
                                             ta.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                    cell.Controls.Add(lbl);
                    row.Cells.Add(cell);

                    //player club
                    cell = new TableCell();
                    lbl = new Label();
                    lbl.Text = ta.Inti_AthleteClub.Inti_Club.ShortName;
                    cell.Controls.Add(lbl);
                    row.Cells.Add(cell);

                    //player price
                    cell = new TableCell();
                    lbl = new Label();
                    lbl.Text = ta.Inti_AthleteClub.Price.ToString();
                    cell.Controls.Add(lbl);
                    row.Cells.Add(cell);

                    //remove-button
                    cell = new TableCell();
                    var btnDel = new Button();
                    btnDel.ID = "btnDelete" + ta.AthleteGUID.ToString();
                    btnDel.CssClass = "btn btn-warning";
                    btnDel.Text = "ta bort";
                    btnDel.ToolTip = "ta bort " + String.Format("{0} {1}", ta.Inti_AthleteClub.Inti_Athlete.FirstName,
                                                                ta.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                    btnDel.Click += DeletePlayer_Click;
                    cell.Controls.Add(btnDel);
                    row.Cells.Add(cell);

                    //add this row
                    tbl.Rows.Add(row);
                }
            }

            //add button to add a new one
            row = new TableRow();
            cell = new TableCell();
            cell.ColumnSpan = 4;

            var btn = new Button();
            btn.ID = "btnAdd" + positionShortName;
            btn.CssClass = "btn btn-primary";
            btn.Text = "lägg till";
            btn.ToolTip = "lägg till en " + positionLongName;
            btn.Click += new EventHandler(AddPlayer_Click);
            cell.Controls.Add(btn);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            container.Controls.Add(tbl);
        }

        private string GenerateHtmlForPosition(string positionShortName, string positionLongName, List<Inti_TeamAthlete> listOfAthletes, PlaceHolder container)
        {
            //insert header
            var sHtml = "";
            sHtml += "<div class='list-group-item list-group-item-success'>";


            sHtml += "<a class='btn btn-primary pull-right'";
            sHtml += " href='PlayerSelector.aspx?teamGuid=" + TeamId.ToString() + "&redirecturl=UserTeamEdit.aspx&position=" + positionShortName + "&tourGUID=" + Tournament.GUID.ToString() + "'";

            sHtml += ">lägg till</a>";
            sHtml += "</p>";

            sHtml += "<p class='list-group-item-heading'>" + positionLongName + "</p>";

            sHtml += "</div>";


            //add control to delete?
            if (listOfAthletes != null)
            {
                foreach (var ta in listOfAthletes)
                {

                    sHtml += "<div class='list-group-item'>";

                    sHtml += "<button class='btn btn-danger pull-right' ";
                    sHtml += " onclick='DeletePlayer(\"" + ta.AthleteGUID + "\", \"" + TeamVersionId + "\");'";
                    sHtml += " >ta bort</button>";

                    //player name
                    var playerName = String.Format("{0} {1}", ta.Inti_AthleteClub.Inti_Athlete.FirstName,
                                             ta.Inti_AthleteClub.Inti_Athlete.LastName).Trim();

                    //no points visible in this view
                    // <span class="badge"><%# Eval("Points") %></span>
                    sHtml += "<p class='list-group-item-heading'>" + playerName + "</p>";
                    sHtml += "<p class='list-group-item-text'>" + ta.Inti_AthleteClub.Inti_Club.Name + "</p>";
                    sHtml += "<p class='list-group-item-text'>" + ta.Inti_AthleteClub.Price.ToString() +"</p>";
                    //no trade out info in this view
                    //<p class="list-group-item-text"><%# ((DateTime?)Eval("TradeOut")).HasValue ? "Utbytt: " + Eval("TradeOut", "{0:d}" ) : string.Empty  %></p>



                    sHtml += "</div>";



                    //remove-button
                    //cell = new TableCell();
                    //var btnDel = new Button();
                    //btnDel.ID = "btnDelete" + ta.AthleteGUID.ToString();
                    //btnDel.CssClass = "btn btn-warning";
                    //btnDel.Text = "ta bort";
                    //btnDel.ToolTip = "ta bort " + String.Format("{0} {1}", ta.Inti_AthleteClub.Inti_Athlete.FirstName,
                    //                                            ta.Inti_AthleteClub.Inti_Athlete.LastName).Trim();
                    //btnDel.Click += DeletePlayer_Click;
                }
            }


            //var btn = new Button();
            //btn.ID = "btnAdd" + positionShortName;
            //btn.CssClass = "btn btn-primary";
            //btn.Text = "lägg till";
            //btn.ToolTip = "lägg till en " + positionLongName;
            //btn.Click += new EventHandler(AddPlayer_Click);

            return sHtml;
        }

        [System.Web.Services.WebMethod]
        public static string DeletePlayer(Guid athleteGuid, Guid teamVersionId)
        {
            //delete this ahtlete
            using (var db = Global.GetConnection())
            {
                //var current user

                var currentUserGuid = Global.SessionProperties.UserGuid;
                var isAdmin = Global.SessionProperties.HasPermission("ADMIN");

                var teamAthlete =
                    db.Inti_TeamAthlete.SingleOrDefault(
                        ta =>  (ta.Inti_TeamVersion.Inti_Team.Sys_User.GUID == currentUserGuid || isAdmin) && ta.Inti_TeamVersion.GUID == teamVersionId && ta.AthleteGUID == athleteGuid);

                if (teamAthlete == null) return "";
                db.Inti_TeamAthlete.DeleteOnSubmit(teamAthlete);

                db.SubmitChanges();

            }

            return "";
        }

        /// <summary>
        /// Show admin functions for the team
        /// </summary>
        /// <param name="teamId"></param>
        private void LoadForSuperAdmin(Guid teamId)
        {
            pnlSuperAdmin.Visible = true;

            using (var db = Global.GetConnection())
            {
                var teamVersions = db.Inti_TeamVersion.Where(t => t.TeamGUID == teamId).OrderBy(t=>t.Version);

                var canBeCleaned = false;
                var outputString = "<ul>";
                foreach (var teamVersion in teamVersions)
                {
                    if (!teamVersion.ValidFrom.HasValue)
                        canBeCleaned = true;

                    outputString += "<li>";
                    outputString += teamVersion.Version;
                    outputString +=  ": ";
                    outputString += teamVersion.ValidFrom.HasValue ? teamVersion.ValidFrom.Value.ToShortDateString() : string.Empty;
                    outputString += " - ";
                    outputString += teamVersion.ValidTo.HasValue ? teamVersion.ValidTo.Value.ToShortDateString() : string.Empty;
                    outputString += "<br/><small>";
                    outputString += String.Format("antal spelare: {0}", teamVersion.Inti_TeamAthlete.Count);
                    outputString += "<br/>";
                    outputString += String.Format("guid: {0}", teamVersion.GUID);
                    outputString += "</small></li>";
                }
                outputString += "</ul>";

                lblTeamVersions.Text = outputString;

                btnCleanTeam.Enabled = canBeCleaned;
            }


        }

        protected void btnCleanTeam_Click(object sender, EventArgs e)
        {
            CleanTeam(TeamId);

            LoadForSuperAdmin(TeamId);
        }

        protected void btnRemoveVersion_OnClick(object sender, EventArgs e)
        {
            var versionToRemove = txtVersionToRemove.Text;

            Guid guidToRemove;
            if (!Guid.TryParse(versionToRemove, out guidToRemove)) return;

            RemoveVersion(guidToRemove);

            LoadForSuperAdmin(TeamId);

        }


        /// <summary>
        /// Delete a teamVersion
        /// </summary>
        /// <param name="teamVersionGuid"></param>
        private void RemoveVersion(Guid teamVersionGuid)
        {
            using (var db = Global.GetConnection())
            {
                //get team versions to delete
                var teamVersionToDelete =
                    db.Inti_TeamVersion.Single(tv=>tv.GUID == teamVersionGuid);

                var athletesToDelete = db.Inti_TeamAthlete.Where(a => a.TeamVersionGUID == teamVersionGuid);

                foreach (var intiTeamAthlete in athletesToDelete)
                {
                    db.Inti_TeamAthlete.DeleteOnSubmit(intiTeamAthlete);
                }

                db.Inti_TeamVersion.DeleteOnSubmit(teamVersionToDelete);

                db.SubmitChanges();

            }
        }

        private void CleanTeam(Guid teamGuid)
        {
            //delete any team versions that are the latest
            using (var db = Global.GetConnection())
            {
                //get highest version number
                var maxTeamVersion = db.Inti_TeamVersion.Where(t => t.TeamGUID == teamGuid).Max(t => t.Version);

                //get team versions to delete
                var teamVersionsToDelete =
                    db.Inti_TeamVersion.Where(t => t.TeamGUID == teamGuid && t.Version == maxTeamVersion);

                var athletesToDelete =
                    db.Inti_TeamAthlete.Where(a => teamVersionsToDelete.Any(t => t.GUID == a.TeamVersionGUID));

                foreach (var intiTeamAthlete in athletesToDelete)
                {
                    db.Inti_TeamAthlete.DeleteOnSubmit(intiTeamAthlete);
                }

                foreach (var intiTeamVersion in teamVersionsToDelete)
                {
                    db.Inti_TeamVersion.DeleteOnSubmit(intiTeamVersion);
                }

                db.SubmitChanges();

            }

        }





        protected void DeletePlayer_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var senderButton = (sender as Button);
                if (senderButton.ID.StartsWith("btnDelete"))
                {
                    var athleteGUID = new Guid(senderButton.ID.Replace("btnDelete", ""));

                    //delete this ahtlete
                    using(var db = Global.GetConnection())
                    {
                        var teamAthlete =
                            db.Inti_TeamAthlete.Single(
                                ta => ta.Inti_TeamVersion.GUID == TeamVersionId && ta.AthleteGUID == athleteGUID);

                        db.Inti_TeamAthlete.DeleteOnSubmit(teamAthlete);

                        db.SubmitChanges();

                        LoadTeam();

                        CheckTransferWindow(false);
                    }
                }
            }
        }


        protected void AddPlayer_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var senderButton = (sender as Button);
                if (senderButton.ID.StartsWith("btnAdd"))
                {
                    var positionShortName = senderButton.ID.Replace("btnAdd", "");

                    WebControlManager.RedirectWithQueryString("PlayerSelector.aspx", new string[] { "teamGUID", "redirecturl", "position", "tourGUID" }, new string[] { TeamId.ToString(), "UserTeamEdit.aspx", positionShortName, Tournament.GUID.ToString() });

                }
            }
        }

        private void AddPlayer(Guid athleteGuid)
        {
            //test so the player isn't added already
            using (var db = Global.GetConnection())
            {
                var test = from ta in db.Inti_TeamAthlete
                           where ta.AthleteGUID == athleteGuid &&
                                    ta.TeamVersionGUID == TeamVersionId
                               select ta;
                if (!test.Any())
                {
                    var teamAthlete = new Inti_TeamAthlete();
                    teamAthlete.AthleteGUID = athleteGuid;
                    teamAthlete.TeamVersionGUID = TeamVersionId;
                    db.Inti_TeamAthlete.InsertOnSubmit(teamAthlete);
                    db.SubmitChanges();
                }
            }
        }

        private string GetAthletesInString(List<Inti_TeamAthlete> listOfAthletes)
        {
            string output = "";
            foreach(var ta in listOfAthletes)
            {
                if (output == "")
                    output = "<h5>" + ta.Inti_AthleteClub.Inti_Position.Name + "</h5>";

                output +=
                    String.Format("{0} {1}", ta.Inti_AthleteClub.Inti_Athlete.FirstName,
                                  ta.Inti_AthleteClub.Inti_Athlete.LastName).Trim()
                                  + String.Format(" ({0})", ta.Inti_AthleteClub.Inti_Club.ShortName)
                                  + "<br>";

            }

            if (output != "")
                output += "<br>";

            return output;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (TeamId == Guid.Empty)
            {
                //create new team
                var team = new Inti_Team();
                team.Name = TeamName.Text;
                team.Presentation = TeamDescription.Text;

                team.UserGUID = SessionProps.UserGuid;
                team.TournamentGUID = Tournament.GUID;

                team.IsActive = false;

                team.IsPaid = false;
                team.BonusPoints = 0;

                //team version
                var teamVersion = new Inti_TeamVersion();
                teamVersion.TeamGUID = team.GUID;
                teamVersion.Version = 1;
                teamVersion.ValidFrom = Tournament.EndRegistration;

                using (var db = Global.GetConnection())
                {
                    db.Inti_Team.InsertOnSubmit(team);
                    db.Inti_TeamVersion.InsertOnSubmit(teamVersion);
                    db.SubmitChanges();
                }

                WebControlManager.RedirectWithQueryString("UserTeamEdit.aspx", new string[] { "teamGUID" }, new string[] { team.GUID.ToString() });

            }
            else
            {
                //update this team
                using(var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);

                    team.Name = TeamName.Text;
                    team.Presentation = TeamDescription.Text;

                    db.SubmitChanges();
                }
            }
        }

        protected void btnDeleteTeam_Click(object sender, EventArgs e)
        {
            if (TeamId != Guid.Empty)
            {
                //delete this team
                using (var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);


                }
            }
        }

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            if (TeamId != Guid.Empty)
            {
                //activate this team
                using (var db = Global.GetConnection())
                {
                    var team = db.Inti_Team.Single(t => t.GUID == TeamId);

                    team.IsActive = true;

                    db.SubmitChanges();

                    LoadTeam();

                }
            }
        }

        protected void btnUndoTransfers_Click(object sender, EventArgs e)
        {
            CheckTransferWindow(true);

            LoadTeam();
        }

        protected void btnCommitTransfers_Click(object sender, EventArgs e)
        {
            using (var db = Global.GetConnection())
            {
                //get current transferwindow
                var transferPeriods = db.Inti_TransferPeriod.Where(tf => tf.TournamentGUID == SessionProps.SelectedTournament.GUID).ToList();
                Inti_TransferPeriod transferPeriod = null;
                foreach (var tf in transferPeriods)
                {
                    if (tf.StartDate <= CurrentDate && tf.EndDate >= CurrentDate)
                    {
                        transferPeriod = tf;
                        break;
                    }
                }
                if (transferPeriod != null)
                {
                    Inti_TeamVersion transferVersion = null;
                    Inti_TeamVersion currentVersion = null;

                    GetCurrentAndTransferVersion(db, out currentVersion, out transferVersion);

                    var trans = new UserTeamManagement(Global.ConnectionString, SessionProps);
                    trans.CommitTransfers(db, transferPeriod, currentVersion, transferVersion, TeamId);

                    lblTransferPeriodInfo.Text += "<br><br><span class=\"label label-success\">Nu är bytena genomförda!</span>";

                    //are we in transferwindow?
                    CheckTransferWindow(false);

                    LoadTeam();
                }

            }



        }



        protected void btnReOpenTransferTeam_Click(object sender, EventArgs e)
        {
            using (var db = Global.GetConnection())
            {
                //get current transferwindow
                var transferPeriods = db.Inti_TransferPeriod.Where(tf => tf.TournamentGUID == SessionProps.SelectedTournament.GUID).ToList();
                Inti_TransferPeriod transferPeriod = null;
                foreach (var tf in transferPeriods)
                {
                    if (tf.StartDate <= CurrentDate && tf.EndDate >= CurrentDate)
                    {
                        transferPeriod = tf;
                        break;
                    }
                }
                if (transferPeriod != null)
                {
                    Inti_TeamVersion transferVersion = null;
                    Inti_TeamVersion currentVersion = null;

                    GetCurrentAndTransferVersion(db, out currentVersion, out transferVersion);

                    var trans = new UserTeamManagement(Global.ConnectionString, SessionProps);
                    trans.ReOpenTeamForTransfers(db, transferPeriod, transferVersion, currentVersion, TeamId);

                    lblTransferPeriodInfo.Text += "<br><br><span class=\"label label-success\">Nu kan du ändra på bytena...</span>";

                    //are we in transferwindow?
                    CheckTransferWindow(false);

                    LoadTeam();
                }

            }
        }

        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("UserTeamUploadImage.aspx", new string[] { "teamGUID" }, new string[] { TeamId.ToString() });
        }


        protected void btnRemoveDuplicateTransfers_OnClick(object sender, EventArgs e)
        {
            using (var db = Global.GetConnection())
            {
                var teamManagement = new UserTeamManagement(Global.ConnectionString, SessionProps);

                teamManagement.RemoveDuplicateTransfers(db, TeamId);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            //ladda bara om...

            DateTime checkDate;
            if (string.IsNullOrEmpty(txtFakeDate.Text) || !DateTime.TryParse(txtFakeDate.Text, out checkDate))
            {
                Session["FAKEDATE"] = null;
                return;
            }

            Session["FAKEDATE"] = checkDate;

            //reload



        }

        protected void btnClearFakeDate_Click(object sender, EventArgs e)
        {
            Session["FAKEDATE"] = null;
            txtFakeDate.Text = null;
        }
    }
}
