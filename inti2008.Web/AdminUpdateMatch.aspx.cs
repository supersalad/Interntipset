using System;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminUpdateMatch : IntiPage
    {


        private const string pe_Win = "AE8C1257-FF01-4152-9EBE-4DDA6AD41F45";
        private const string pe_Draw = "E6C406DB-7995-4488-824A-C4B010843A75";
        private const string pe_Goal = "EFE2BABF-E498-4E89-AD4A-8CB247377F86";
        private const string pe_CleanSheet = "16786223-BE78-4FCF-A780-2FA5D925E697";
        private const string pe_Penalty = "54DB656E-5BD1-4902-A5FA-7273B1F30BD7";
        private const string pe_OwnGoal = "53B9A242-8329-42A8-8771-93C3A90B5455";
        private const string pe_RedCard = "6F24C998-94F7-42C5-830B-258703E1E0E7";
        private const string pe_PenaltyMiss = "4E304B11-8937-405A-98DC-72D896F7A89E";

        protected void Page_Init(object sender, EventArgs e)
        {
            txtAwayTeamGoals.Attributes.Add("onchange", "CheckIfZero(" + txtAwayTeamGoals.ClientID + ".value," + chkAddCleanSheetScoreHome.ClientID + ")");
            txtHomeTeamGoals.Attributes.Add("onchange", "CheckIfZero(" + txtHomeTeamGoals.ClientID + ".value," + chkAddCleanSheetScoreAway.ClientID + ")");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN", "USER_MATCHUPDATE");

            if(!IsPostBack)
            {
                SwitchStep(0);
            }
            lblUpdateProgress.Text = "";
        }

        private Guid MatchId
        {
            get
            {
                if (drpMatches.SelectedValue.Length > 28)
                {
                    return new Guid(drpMatches.SelectedValue);
                }

                return Guid.Empty;
            }
        }

        
        private void LoadDays()
        {
            int mostRecentDay = 1;

            using (var db = Global.GetConnection())
            {
                var matches = from m in db.Inti_Match
                              where (m.IsUpdated ?? false)
                                    && m.TournamentGUID == SessionProps.SelectedTournament.GUID
                              select m;

                var matchList = matches.OrderByDescending(m => m.TourDay).ToList();
                if (matchList.Count > 0)
                    mostRecentDay = matchList[0].TourDay;
            }

            for(var i =1;i<=SessionProps.SelectedTournament.NmbrOfDays;i++)
            {
                drpTourDay.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            drpTourDay.ClearSelection();
            drpTourDay.SelectedValue = mostRecentDay.ToString();
        }

        private void LoadMatchesInSelectedDay()
        {
            //load the matches in this day
            using (var db = Global.GetConnection())
            {
                var tourDay = int.Parse(drpTourDay.SelectedValue);
                var matches = from m in db.Inti_Match
                              where m.TournamentGUID == SessionProps.SelectedTournament.GUID
                                    && m.TourDay == tourDay
                              select new
                              {
                                  m.GUID,
                                  Text = (m.Updater != null || (m.IsUpdated ?? false))
                                        ? "* " + m.HomeClubInti_Club.Name + " - " + m.Inti_Club.Name
                                        : m.HomeClubInti_Club.Name + " - " + m.Inti_Club.Name
                              };

                drpMatches.DataValueField = "GUID";
                drpMatches.DataTextField = "Text";
                drpMatches.DataSource = matches.OrderBy(m => m.Text).ToList();
                drpMatches.DataBind();
            }
        }


        protected void drpTourDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchesInSelectedDay();
        }

        protected void btnSelectMatch_Click(object sender, EventArgs e)
        {
            //update updater and startupdate date


            SwitchStep(1);
        }

        /// <summary>
        /// Byter steg i wizarden
        /// </summary>
        /// <param name="step">0=välj omgång och match,
        /// 1=ange matchresultat,
        /// 2=ange hålla nollan-poäng,
        /// 3=ange målpoäng,
        /// 4=dela ut poängen</param>
        private void SwitchStep(int step)
        {
            pnlSelectDayAndMatch.Visible = (step == 0);
            pnlMatchHeader.Visible = (step == 1);
            pnlDefenderPoints.Visible = (step == 2);
            pnlPlayerPoints.Visible = (step == 3);
            btnClearPoints.Visible = (MatchId != Guid.Empty && step==1);
            btnClearTeamPoints.Visible = (MatchId != Guid.Empty && step == 1);
            btnDistributePoints.Visible = (step == 4);

            switch(step)
            {
                case 0 :
                    matchChangeLog.LoadChangeLog(typeof(Inti_Match));
                    LoadDays();
                    LoadMatchesInSelectedDay();
                    break;
                case 1 :
                    new MatchManagement(Global.ConnectionString, SessionProps).SetUpdater(MatchId);
                    matchChangeLog.LoadChangeLog(MatchId);
                    LoadMatchHeader();
                    break;
                case 2 :
                    LoadDefenders();
                    break;
                case 3 :
                    LoadAthletes();
                    break;
                case 4 :
                    
                    break;
                    
            }
            if (step > 0) ShowPointsToDistribute(step < 4);

        }

        private void ShowPointsToDistribute(bool final)
        {
            if (MatchId != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    var mpes = from mpe in db.Inti_MatchPointEvent
                               where mpe.Inti_Match.GUID == MatchId
                               orderby mpe.Inti_AthleteClub.Inti_Club.Name
                               orderby mpe.Inti_AthleteClub.Inti_Position.Name
                               orderby mpe.Inti_PointEvent.Name
                               select mpe;

                    lblUpdateProgress.Text = "Poäng:<br>";
                    if (final) lblUpdateProgress.Text = "Följande poäng kommer delas ut för matchen:<br>";

                    lblUpdateProgress.Text += "<table><tr><td>spelare</td><td>Insats</td><td>poäng</td></tr>";
                    foreach(var mpe in mpes)
                    {
                        lblUpdateProgress.Text += String.Format("<tr><td>{0} {1}</td><td>{2}</td><td>{3}</td></tr> ",
                                                                mpe.Inti_AthleteClub.Inti_Athlete.FirstName,
                                                                mpe.Inti_AthleteClub.Inti_Athlete.LastName,
                                                                mpe.Inti_PointEvent.Name,
                                                                mpe.Points.ToString()).Trim();
                    }
                    lblUpdateProgress.Text += "</table>";

                }
            }
        }

        private void LoadAthletes()
        {
            if (MatchId != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    lblHomeGoalPoints.Text = match.HomeClubInti_Club.Name;

                    var homeAthletes = from def in db.Inti_AthleteClub
                                       where def.ClubGUID == match.HomeClub
                                             && def.Inti_Position.ShortName != "MGR"
                                             && (def.IsActive ?? false)
                                       orderby def.Inti_Athlete.LastName
                                       orderby def.Inti_Athlete.FirstName
                                       orderby def.Inti_Position.SortOrder descending 
                                    select new
                                    {
                                        def.GUID,
                                        AthleteName =
                             def.Inti_Athlete.FirstName + " " + def.Inti_Athlete.LastName,
                                        Position = def.Inti_Position.Name
                                    };

                    grdHomeGoalPoints.DataKeyNames = new string[] { "GUID" };
                    grdHomeGoalPoints.DataSource = homeAthletes.ToList();
                    grdHomeGoalPoints.DataBind();

                    lblAwayGoalPoints.Text = match.Inti_Club.Name;

                    var awayAthletes = from def in db.Inti_AthleteClub
                                    where def.ClubGUID == match.AwayClub
                                    && def.Inti_Position.ShortName != "MGR"
                                             && (def.IsActive ?? false)
                                       orderby def.Inti_Athlete.LastName
                                       orderby def.Inti_Athlete.FirstName
                                       orderby def.Inti_Position.SortOrder descending 
                                    select new
                                    {
                                        def.GUID,
                                        AthleteName =
                             def.Inti_Athlete.FirstName + " " + def.Inti_Athlete.LastName,
                                        Position = def.Inti_Position.Name
                                    };

                    grdAwayGoalPoints.DataKeyNames = new string[] { "GUID" };
                    grdAwayGoalPoints.DataSource = awayAthletes.ToList();
                    grdAwayGoalPoints.DataBind();
                }
            }
        }

        private void LoadDefenders()
        {
            if(MatchId != Guid.Empty)
            {
                //clear labels
                lblHomeClubDefPoints.Text = String.Empty;
                lblAwayClubDefPoints.Text = String.Empty;

                using (var db = Global.GetConnection())
                {
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    if((match.AwayScore ?? -1) == 0 || chkAddCleanSheetScoreHome.Checked)
                    {
                        //hemmalaget höll nollan
                        lblHomeClubDefPoints.Text = match.HomeClubInti_Club.Name;

                        grdHomeDefenderPoints.Visible = true;

                        var defenders = from def in db.Inti_AthleteClub
                                        where def.ClubGUID == match.HomeClub
                                              && (def.Inti_Position.ShortName == "GK"
                                                  || def.Inti_Position.ShortName == "D")
                                                  && (def.IsActive ?? false)
                                                    orderby def.Inti_Athlete.LastName
                                                    orderby def.Inti_Athlete.FirstName
                                                    orderby def.Inti_Position.SortOrder descending 
                                        select new
                                                   {
                                                       def.GUID,
                                                       AthleteName =
                                            def.Inti_Athlete.FirstName + " " + def.Inti_Athlete.LastName,
                                                       Position = def.Inti_Position.Name
                                                   };

                        grdHomeDefenderPoints.DataKeyNames = new string[] { "GUID" } ;
                        grdHomeDefenderPoints.DataSource = defenders.ToList();
                        grdHomeDefenderPoints.DataBind();
                    }
                    else
                    {
                        grdHomeDefenderPoints.DataSource = null;
                        grdHomeDefenderPoints.DataBind();
                        grdHomeDefenderPoints.Visible = false;
                    }

                    if ((match.HomeScore ?? -1) == 0 || chkAddCleanSheetScoreAway.Checked)
                    {
                        //bortalaget höll nollan
                        lblAwayClubDefPoints.Text = match.Inti_Club.Name;

                        grdAwayDefenderPoints.Visible = true;

                        var defenders = from def in db.Inti_AthleteClub
                                        where def.ClubGUID == match.AwayClub
                                              && (def.Inti_Position.ShortName == "GK"
                                                  || def.Inti_Position.ShortName == "D")
                                                  && (def.IsActive ?? false)
                                        orderby def.Inti_Athlete.LastName
                                        orderby def.Inti_Athlete.FirstName
                                        orderby def.Inti_Position.SortOrder descending 
                                        select new
                                        {
                                            def.GUID,
                                            AthleteName =
                                 def.Inti_Athlete.FirstName + " " + def.Inti_Athlete.LastName,
                                            Position = def.Inti_Position.Name
                                        };

                        grdAwayDefenderPoints.DataKeyNames = new string[] { "GUID" };
                        grdAwayDefenderPoints.DataSource = defenders.ToList();
                        grdAwayDefenderPoints.DataBind();
                    }
                    else
                    {
                        grdAwayDefenderPoints.DataSource = null;
                        grdAwayDefenderPoints.DataBind();
                        grdAwayDefenderPoints.Visible = false;
                    }
                }
            }
        }

        
        private void LoadMatchHeader()
        {
            if (MatchId != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    lblHomeTeam.Text = match.HomeClubInti_Club.Name;
                    lblAwayTeam.Text = match.Inti_Club.Name;
                    txtMatchMatchDate.Text = (match.MatchDate ?? DateTime.Now).ToShortDateString();
                    txtHomeTeamGoals.Text = (match.HomeScore ?? 0).ToString();
                    txtAwayTeamGoals.Text = (match.AwayScore ?? 0).ToString();

                    for (var i = 1; i <= SessionProps.SelectedTournament.NmbrOfDays; i++)
                    {
                        drpMatchTourDay.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    drpMatchTourDay.ClearSelection();
                    drpMatchTourDay.SelectedValue = match.TourDay.ToString();
                }
            }
        }

        private void ClearPointEvents(bool onlyTeamPointEvents)
        {
            if (MatchId != Guid.Empty)
            {

                var matchManagement = new MatchManagement(Global.ConnectionString, SessionProps);

                if (onlyTeamPointEvents)
                    matchManagement.ClearTeamPoints(MatchId); 
                else
                    matchManagement.ClearPointEvents(MatchId);
            }
        }

        protected void btnClearPoints_Click(object sender, EventArgs e)
        {
            ClearPointEvents(false);
        }

        protected void btnUpdateMatchHeader_Click(object sender, EventArgs e)
        {
            if (MatchId != Guid.Empty)
            {

                var step = 2;
                using (var db = Global.GetConnection())
                {
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    match.HomeScore = int.Parse(txtHomeTeamGoals.Text);
                    match.AwayScore = int.Parse(txtAwayTeamGoals.Text);
                    match.TourDay = int.Parse(drpTourDay.SelectedValue);
                    match.MatchDate = DateTime.Parse(txtMatchMatchDate.Text);

                    db.SubmitChanges();

                    //ta bort gamla poäng
                    ClearPointEvents(false);

                    //tilldela manager poäng
                    if (match.HomeScore == match.AwayScore)
                    {
                        var peMgr = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_Draw));
                        var mgrs = from ac in db.Inti_AthleteClub
                                   where (ac.ClubGUID == match.HomeClub
                                          || ac.ClubGUID == match.AwayClub)
                                         && (ac.IsActive ?? false)
                                         && ac.Inti_Position.ShortName == "MGR"
                                   select ac;
                        foreach (var mgr in mgrs)
                        {
                            AddMatchPointEvent(db, match, mgr,peMgr,1);    
                        }
                        
                    }
                    else
                    {
                        var peMgr = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_Win));
                        var clubGuid = (match.HomeScore > match.AwayScore ? match.HomeClub : match.AwayClub);

                        var mgrs = from ac in db.Inti_AthleteClub
                                   where (ac.ClubGUID == clubGuid)
                                         && (ac.IsActive ?? false)
                                         && ac.Inti_Position.ShortName == "MGR"
                                   select ac;
                        foreach (var mgr in mgrs)
                        {
                            AddMatchPointEvent(db, match, mgr,peMgr,1);    
                        }
                    }
                    
                    db.SubmitChanges();

                    if(match.HomeScore > 0 && match.AwayScore > 0 && !chkAddCleanSheetScoreHome.Checked && !chkAddCleanSheetScoreAway.Checked)
                    {
                        step = 3;
                    }

                }

                SwitchStep(step);
            }

        }

        protected void btnUpdateDefenderPoints_Click(object sender, EventArgs e)
        {
            //loopa de båda gridsen, om de visas
            if (MatchId != Guid.Empty)
            {

                using (var db = Global.GetConnection())
                {
                    
                    //hämta hålla nollan poäng
                    var peCleanSheet = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_CleanSheet));

                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    for (var i = 0; i < grdHomeDefenderPoints.Rows.Count; i++)
                    {
                        var row = grdHomeDefenderPoints.Rows[i];
                        grdHomeDefenderPoints.SelectedIndex = i;
                        var guid = new Guid(grdHomeDefenderPoints.SelectedValue.ToString());

                        var athlete = db.Inti_AthleteClub.Single(ac => ac.GUID == guid);

                        if((row.Cells[3].FindControl("chkCleanSheet") as CheckBox).Checked)
                        {
                            var mpe = new Inti_MatchPointEvent();
                            mpe.MatchGUID = MatchId;
                            mpe.AthleteClubGUID = guid;
                            mpe.PointEventGUID = peCleanSheet.GUID;

                            mpe.Points =
                                peCleanSheet.Inti_PointEventPosition.Where(
                                    pep => pep.PositionGUID == athlete.PositionGUID).ToList()[0].Points;

                            if(match.TourDay == 19 || match.TourDay == 38)
                            {
                                //dubbla poäng
                                mpe.Points = mpe.Points*2;
                            }

                            db.Inti_MatchPointEvent.InsertOnSubmit(mpe);
                        }
                    }

                    for (var i = 0; i < grdAwayDefenderPoints.Rows.Count; i++)
                    {
                        var row = grdAwayDefenderPoints.Rows[i];
                        grdAwayDefenderPoints.SelectedIndex = i;
                        var guid = new Guid(grdAwayDefenderPoints.SelectedValue.ToString());

                        var athlete = db.Inti_AthleteClub.Single(ac => ac.GUID == guid);

                        if ((row.Cells[3].FindControl("chkCleanSheet") as CheckBox).Checked)
                        {
                            var mpe = new Inti_MatchPointEvent();
                            mpe.MatchGUID = MatchId;
                            mpe.AthleteClubGUID = guid;
                            mpe.PointEventGUID = peCleanSheet.GUID;

                            mpe.Points =
                                peCleanSheet.Inti_PointEventPosition.Where(
                                    pep => pep.PositionGUID == athlete.PositionGUID).ToList()[0].Points;

                            if (match.TourDay == 19 || match.TourDay == 38)
                            {
                                //dubbla poäng
                                mpe.Points = mpe.Points * 2;
                            }

                            db.Inti_MatchPointEvent.InsertOnSubmit(mpe);
                        }
                    }

                    db.SubmitChanges();
                }
             
                SwitchStep(3);
            }
        }

        protected void btnUpdateGoalPoints_Click(object sender, EventArgs e)
        {
            //loopa de båda gridsen, om de visas
            if (MatchId != Guid.Empty)
            {

                using (var db = Global.GetConnection())
                {

                    //hämta mål/straff poäng
                    var peGoal = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_Goal));
                    var peRedCard = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_RedCard));
                    var pePenalty = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_Penalty));
                    var peOwnGoal = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_OwnGoal));
                    var pePenaltyMiss = db.Inti_PointEvent.Single(pe => pe.GUID == new Guid(pe_PenaltyMiss));
                    
                    var match = db.Inti_Match.Single(m => m.GUID == MatchId);

                    for (var i = 0; i < grdHomeGoalPoints.Rows.Count; i++)
                    {
                        var row = grdHomeGoalPoints.Rows[i];
                        grdHomeGoalPoints.SelectedIndex = i;
                        var guid = new Guid(grdHomeGoalPoints.SelectedValue.ToString());

                        var athlete = db.Inti_AthleteClub.Single(ac => ac.GUID == guid);

                        //mål?
                        var goals = int.Parse((row.Cells[3].FindControl("drpGoals") as DropDownList).SelectedValue);
                        if(goals > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, peGoal, goals);
                        }
                        //straffmål?
                        var penalties = int.Parse((row.Cells[4].FindControl("drpPenGoals") as DropDownList).SelectedValue);
                        if (penalties > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, pePenalty, penalties);
                        }
                        //självmål?
                        var ownGoals = int.Parse((row.Cells[5].FindControl("drpOwnGoals") as DropDownList).SelectedValue);
                        if (ownGoals > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, peOwnGoal, ownGoals);
                        }
                        //röttkort?
                        var redCard = ((row.Cells[6].FindControl("chkRedCard") as CheckBox).Checked);
                        if (redCard)
                        {
                            AddMatchPointEvent(db, match, athlete, peRedCard, 1);
                        }

                        //straffmiss?
                        var penMiss = int.Parse((row.Cells[7].FindControl("drpPenMissed") as DropDownList).SelectedValue);
                        if (penMiss > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, pePenaltyMiss, penMiss);
                        }
                    }

                    for (var i = 0; i < grdAwayGoalPoints.Rows.Count; i++)
                    {
                        var row = grdAwayGoalPoints.Rows[i];
                        grdAwayGoalPoints.SelectedIndex = i;
                        var guid = new Guid(grdAwayGoalPoints.SelectedValue.ToString());

                        var athlete = db.Inti_AthleteClub.Single(ac => ac.GUID == guid);

                        //mål?
                        var goals = int.Parse((row.Cells[3].FindControl("drpGoals") as DropDownList).SelectedValue);
                        if (goals > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, peGoal, goals);
                        }
                        //straffmål?
                        var penalties = int.Parse((row.Cells[4].FindControl("drpPenGoals") as DropDownList).SelectedValue);
                        if (penalties > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, pePenalty, penalties);
                        }
                        //självmål?
                        var ownGoals = int.Parse((row.Cells[5].FindControl("drpOwnGoals") as DropDownList).SelectedValue);
                        if (ownGoals > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, peOwnGoal, ownGoals);
                        }
                        //röttkort?
                        var redCard = ((row.Cells[6].FindControl("chkRedCard") as CheckBox).Checked);
                        if (redCard)
                        {
                            AddMatchPointEvent(db, match, athlete, peRedCard, 1);
                        }
                        //straffmiss?
                        var penMiss = int.Parse((row.Cells[7].FindControl("drpPenMissed") as DropDownList).SelectedValue);
                        if (penMiss > 0)
                        {
                            AddMatchPointEvent(db, match, athlete, pePenaltyMiss, penMiss);
                        }
                    }


                    db.SubmitChanges();
                }

                SwitchStep(4);
            }
        }

        private void AddMatchPointEvent(IntiDataContext db, Inti_Match match, Inti_AthleteClub athleteClub, Inti_PointEvent pointEvent, int nmbrOfEvents)
        {
            for(var i=1;i<= nmbrOfEvents;i++)
            {
                var mpe = new Inti_MatchPointEvent();
                mpe.MatchGUID = match.GUID;
                mpe.AthleteClubGUID = athleteClub.GUID;
                mpe.PointEventGUID = pointEvent.GUID;

                mpe.Points =
                    pointEvent.Inti_PointEventPosition.Where(
                        pep => pep.PositionGUID == athleteClub.PositionGUID).ToList()[0].Points;

                if (match.TourDay == 19 || match.TourDay == 38)
                {
                    //dubbla poäng
                    mpe.Points = mpe.Points * 2;
                }

                db.Inti_MatchPointEvent.InsertOnSubmit(mpe);
            }
            
            
        }

        protected void btnReDistributePointsForFullDay_Click(object sender, EventArgs e)
        {
            var matchManagement = new MatchManagement(Global.ConnectionString, SessionProps);

            using (var db = Global.GetConnection())
            {

                var tourDay = int.Parse(drpTourDay.SelectedValue);
                var matches = from m in db.Inti_Match
                              where m.TournamentGUID == SessionProps.SelectedTournament.GUID
                                    && m.TourDay == tourDay
                                    && m.IsUpdated == true
                              select m;

                lblUpdateProgress.Text = "";

                foreach (var match in matches)
                {
                    //clear the points
                    matchManagement.ClearTeamPoints(match.GUID);

                    matchManagement.DistributePoints(match.GUID);

                    lblUpdateProgress.Text += String.Format("{0} - {1}<br>", match.HomeClubInti_Club.Name,
                                                            match.Inti_Club.Name);
                }

                SwitchStep(0);
            }
        }

        protected void btnDistributePoints_Click(object sender, EventArgs e)
        {
            var matchManagement = new MatchManagement(Global.ConnectionString, SessionProps);

            ClearPointEvents(true);

            if (MatchId != Guid.Empty)
            {
                matchManagement.DistributePoints(MatchId);

                lblUpdateProgress.Text = "Matchen uppdaterad!";

                //try to send tweet
                SendMatchTweet(MatchId, matchManagement);

                SwitchStep(0);


            }
        }

        
        private void SendMatchTweet(Guid matchId, MatchManagement matchManagement)
        {
            try
            {

                //compile match text
                var matchText = matchManagement.GetMatchText(matchId);
                lblUpdateProgress.Text += "<BR>" + matchText;

                var url = "http://interntipset.com/Match/" + matchId.ToString();
                Global.SendTweet(matchText, url, SessionProps);
                
                
            }
            catch (Exception ex)
            {
                lblUpdateProgress.Text += "<BR>Kunde inte twittra:<BR>";
                lblUpdateProgress.Text += ex.GetType().Name;
                lblUpdateProgress.Text += ex.Message;
            }
        }
        

        protected void btnClearTeamPoints_Click(object sender, EventArgs e)
        {
            if(MatchId != Guid.Empty)
            {
                ClearPointEvents(true);

                SwitchStep(4);
            }
        }        
    }
}
