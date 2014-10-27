using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminPlayerImport : IntiPage
    {

        private const string _addAthlete = "Lägg till spelare";
        private const string _addAthleteInTournament = "Lägg till spelare i tävlingen";
        private const string _updateAthlete = "Klubbbyte";

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN", "USER_ATHLETEUPDATE");

            LoadTournaments();
        }

        protected IList<ImportAthleteAction> ImportAthleteActions { get; set; }

        private void LoadTournaments()
        {
            drpTournament.DataValueField = "GUID";
            drpTournament.DataTextField = "Name";
            drpTournament.DataSource = new CommonDataSources(Global.ConnectionString, SessionProps).AllTournaments();
            drpTournament.DataBind();
        }

        protected void btnImportPlayers_Click(object sender, EventArgs e)
        {
            ProcessPlayerImportList(txtPlayersToImport.Text);
        }

        //private const int _firstName = 0;
        //private const int _lastName = 1;
        //private const int _club = 2;
        //private const int _position = 3;
        //private const int _price = 4;

        private void ProcessPlayerImportList(string players)
        {
            var playersArray = players.Split(new string[] {"\r\n", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
            

            var counter = 0;
            var totalNumberOfPlayersToImport = playersArray.GetUpperBound(0);

            ImportAthleteActions = new List<ImportAthleteAction>();

            foreach (var player in playersArray)
            {
                var playerDetails = player.Split(new string[] {","}, StringSplitOptions.None);

                ImportAthleteAction athleteImportAction;
                var thisPlayerDetails = GetPlayerDetails(playerDetails);

                if (thisPlayerDetails != null)
                {
                    athleteImportAction = GetOrCreateAthleteImportAction(SelectedTournament, thisPlayerDetails);

                }
                else
                {
                    athleteImportAction = new ImportAthleteAction()
                                              {
                                                  Exception =
                                                      new Exception(String.Format("Input not valid for {0}", player))
                                              };
                }

                athleteImportAction.InString = player;
                athleteImportAction.Done = (athleteImportAction.AthleteIsStored &&
                                            athleteImportAction.AthleteClubIsStored &&
                                            athleteImportAction.AthleteClubIsUpToDate);
                ImportAthleteActions.Add(athleteImportAction);

                
                counter++;

                if (UpdateThePanel(counter))
                    SetDataSource(false);
            }

            SetDataSource(true);
        }

        private string GetImportAction(ImportAthleteAction athleteImportAction)
        {
            string importActions = athleteImportAction.ImportAction ?? "";
            
            if(!athleteImportAction.AthleteIsStored)
            {
                importActions += (String.IsNullOrEmpty(importActions) ? "" : "\r\n") + _addAthlete;
            }

            if(!athleteImportAction.AthleteClubIsStored)
            {
                importActions += (String.IsNullOrEmpty(importActions) ? "" : "\r\n") + _addAthleteInTournament;
            }

            if(!athleteImportAction.AthleteClubIsUpToDate)
            {
                importActions += (String.IsNullOrEmpty(importActions) ? "" : "\r\n") + _updateAthlete;
            }

            return importActions;
        }

        private void SetDataSource(bool final)
        {
            var projection = from importActions in ImportAthleteActions
                             select new
                                        {
                                            GUID = importActions.AthleteClub != null ? importActions.AthleteClub.AthleteGUID : Guid.Empty,
                                            InText = importActions.InString,
                                            MainText =
                                                 importActions.Exception != null ? importActions.Exception.Message : 
                                                 importActions.AthleteClub != null
                                                     ? importActions.AthleteClub.Inti_Athlete.FirstName + " " +
                                                       importActions.AthleteClub.Inti_Athlete.LastName + ", " +
                                                       importActions.AthleteClub.Inti_Club.ShortName + ", " +
                                                       importActions.AthleteClub.Inti_Position.ShortName + ", " +
                                                       importActions.AthleteClub.Price : "????",
                                            importActions.ImportAction,
                                            importActions.Done
                                        };


            grdAthletesToImport.DataSource = projection.ToList();
            grdAthletesToImport.DataBind();

            if (final)
                btnProcessImport.Visible = (projection.ToList().Count()>0 &&
                    (projection.Where(p => !p.Done && !String.IsNullOrEmpty(p.ImportAction)).ToList().Count > 0));

            updPlayerImport.Update();    

        }

        protected Guid SelectedTournament
        {
            get
            {
                return new Guid(drpTournament.SelectedValue);
            }
        }

        private PlayerDetails GetPlayerDetails(string[] playerDetail)
        {
            if ((playerDetail.GetUpperBound(0) < 2 || playerDetail.GetUpperBound(0) > 4))
                return null;

            bool posCodeIncluded = ((playerDetail.GetUpperBound(0) > 2) &&
                                    (IsPosCode(playerDetail[2]) || IsPosCode(playerDetail[3])));

            bool hasFirstName = ((playerDetail.GetUpperBound(0) == 3 && !posCodeIncluded) ||
                                 (playerDetail.GetUpperBound(0) == 4 && posCodeIncluded));

            var outPlayerDetail = new PlayerDetails();
            outPlayerDetail.FirstName = (hasFirstName ? playerDetail[0] : "").Trim();
            outPlayerDetail.LastName = (hasFirstName ? playerDetail[1] : playerDetail[0]).Trim();
            outPlayerDetail.ClubCode = (hasFirstName ? playerDetail[2] : playerDetail[1]).Trim().ToUpper();
            if (posCodeIncluded)
                outPlayerDetail.PosCode = (hasFirstName ? playerDetail[3] : playerDetail[2]).Trim().ToUpper();
            int price;
            var priceString = playerDetail[playerDetail.GetUpperBound(0)].Replace(".","");
            if (int.TryParse(priceString, out price))
                outPlayerDetail.Price = price;
            else
                return null;

            //price imported from fantasyleague?
            if (outPlayerDetail.Price < 1000)
                outPlayerDetail.Price *= 100000;

            return outPlayerDetail;
        }

        private IList<string> _posCodes = new List<string>(){"MGR", "GK", "D", "M", "S"};

        private bool IsPosCode(string posCode)
        {
            var testString = posCode.Trim().ToUpper();
            return _posCodes.Contains(testString);
        }
        private class PlayerDetails
        {
            public int Price { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ClubCode { get; set; }
            public string PosCode { get; set; }
        }

        private ImportAthleteAction GetOrCreateAthleteImportAction(Guid selectedTournament, PlayerDetails playerDetails)
        {

            var importAthleteAction = new ImportAthleteAction();

            try
            {
                //verify and get club
                var club = new ClubManagement(Global.ConnectionString, SessionProps).GetClubByCode(selectedTournament, playerDetails.ClubCode);

                //verify and get position
                Inti_Position position = null;
                if (!String.IsNullOrEmpty(playerDetails.PosCode))
                    position = new CommonDataFetches(Global.ConnectionString, SessionProps).GetPosition(playerDetails.PosCode);

                //try to get athlete
                var athlete = new AthleteManagement(Global.ConnectionString, SessionProps).GetAthleteByName(playerDetails.FirstName, playerDetails.LastName);

                importAthleteAction.AthleteIsStored = (athlete != null);

                if (athlete == null)
                {
                    //athlete is added with full name?
                    var searchString = String.IsNullOrEmpty(playerDetails.FirstName)
                                           ? playerDetails.LastName
                                           : String.Format("{0}, {1}", playerDetails.FirstName, playerDetails.LastName);
                    IList<Inti_Athlete> matchingAthletes;
                    using (var db = Global.GetConnection())
                    {
                        matchingAthletes = Searching.SearchForAthletes(db, searchString);
                    }


                    if (matchingAthletes.Count == 0)
                    {
                        //do we have position?
                        if (position != null)
                        {
                            //create new athlete
                            athlete = new Inti_Athlete();
                            athlete.FirstName = playerDetails.FirstName;
                            athlete.LastName = playerDetails.LastName;        
                        }
                        else
                        {
                            importAthleteAction.Exception = new Exception("Ny spelare men position saknas. Ange med position");
                        }
                        
                    }
                    else
                    {
                        if (matchingAthletes.Count ==1)
                        {
                            athlete = matchingAthletes[0];
                            importAthleteAction.ImportAction += "OBS, kontrollera namnet. ";
                        }
                        else
                        {
                            var message = "Flera spelare matchar namnet:";
                            foreach (var matchingAthlete in matchingAthletes)
                            {
                                message += (matchingAthlete.FirstName + " " + matchingAthlete.LastName).Trim() + ", ";
                            }
                            importAthleteAction.Exception = new Exception(message);    
                        }
                        
                    }
                    
                }

                if (athlete != null)
                {
                    //is the athlete already in tournament?
                    var athleteClub = new AthleteManagement(Global.ConnectionString, SessionProps).GetAthleteClubByTournament(athlete.GUID,
                                                                                                     selectedTournament);

                    importAthleteAction.AthleteClubIsStored = (athleteClub != null);
                    importAthleteAction.AthleteClubIsUpToDate = true;
                    if (athleteClub == null)
                    {
                        if(position != null)
                        {
                            athleteClub = new Inti_AthleteClub();

                            athleteClub.Inti_Athlete = athlete;
                            athleteClub.Inti_Club = club;
                            athleteClub.Inti_Position = position;
                            athleteClub.Price = playerDetails.Price;    
                        }
                        else
                        {
                            importAthleteAction.Exception = new Exception("Befintlig spelare men position saknas. Ange med position");
                        }
                    }
                    else
                    {
                        importAthleteAction.AthleteClubIsUpToDate = (athleteClub.Inti_Club.GUID == club.GUID);

                        //new club
                        if (athleteClub.Inti_Club.GUID != club.GUID)
                        {
                            athleteClub.Inti_Club = club;
                        }

                        if (position != null && athleteClub.Inti_Position.GUID != position.GUID)
                        {
                            importAthleteAction.Exception = new Exception(String.Format("Changed position from {0} to {1}, is it the correct player?", athleteClub.Inti_Position.ShortName, position.ShortName));
                        }

                        if (athleteClub.Price != playerDetails.Price)
                        {
                            importAthleteAction.Exception =
                                new Exception(String.Format("Changed price from {0} to {1}, is it the correct player?",
                                    athleteClub.Price, playerDetails.Price));
                        }
                    }


                    //set import action
                    importAthleteAction.ImportAction += GetImportAction(importAthleteAction);

                    importAthleteAction.AthleteClub = athleteClub;
                }

            }
            catch (Exception exception)
            {
                importAthleteAction.Exception = exception;
            }

            return importAthleteAction;

        }

        protected void grdAthletesToImport_DataBound(object sender, EventArgs e)
        {
            //Each time the data is bound to the grid we need to build up the CheckBoxIDs array
            if(grdAthletesToImport.Rows.Count > 1)
            {
                //Get the header CheckBox
                var headerCheckBox = (CheckBox)grdAthletesToImport.HeaderRow.FindControl("chkCheckAll");

                //Run the ChangeCheckBoxState client-side function whenever the
                //header checkbox is checked/unchecked
                headerCheckBox.Attributes.Add("onclick", "ChangeAllCheckBoxStates(this.checked);");

                foreach (GridViewRow row in grdAthletesToImport.Rows)
                {
                    //Get a programmatic reference to the CheckBox control
                    var cb = (CheckBox)row.FindControl("chkProceed");

                    //Add the CheckBox's ID to the client-side CheckBoxIDs array
                    ScriptManager.RegisterArrayDeclaration(this, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
                    
                }    
            }
        }

        protected void btnPocessImport_Click(object sender, EventArgs e)
        {
            ImportAthleteActions = new List<ImportAthleteAction>();
            var counter = 0;

            foreach (GridViewRow row in grdAthletesToImport.Rows)
            {
                
                //is proceed checked?
                var proceedCheckBox = (CheckBox) row.FindControl("chkProceed");
                if (proceedCheckBox.Checked && !String.IsNullOrEmpty(row.Cells[3].Text))
                {
                    var playerDetails = row.Cells[1].Text.Split(new string[] {","},
                                                                StringSplitOptions.None);
                    ImportAthleteAction importAthleteAction;
                    var thisPlayerDetails = GetPlayerDetails(playerDetails);

                    if (thisPlayerDetails != null)
                    {
                        importAthleteAction = GetOrCreateAthleteImportAction(SelectedTournament,thisPlayerDetails);

                    }
                    else
                    {
                        importAthleteAction = new ImportAthleteAction()
                        {
                            Exception =
                                new Exception(String.Format("Input not valid for {0}", row.Cells[1].Text))
                        };
                    }

                    
                    //perpetuate in string
                    importAthleteAction.InString = row.Cells[1].Text;

                    ImportAthleteActions.Add(importAthleteAction);
                }
                counter++;
            }


            foreach (var importAthleteAction in ImportAthleteActions)
            {
                importAthleteAction.Done = DoUpdates(importAthleteAction);
                counter++;

                if (UpdateThePanel(counter))
                    SetDataSource(false);

            }

            //refresh datasource
            SetDataSource(true);
        }

        private bool UpdateThePanel(int counter)
        {
            int remainder;
            Math.DivRem(counter, 20, out remainder);

            return (remainder == 0);

        }

        private bool DoUpdates(ImportAthleteAction importAthleteAction)
        {
            if(importAthleteAction.Exception != null)
                return false;
            
            var athleteMgmt = new AthleteManagement(Global.ConnectionString, SessionProps);

            if (!importAthleteAction.AthleteIsStored || 
                !importAthleteAction.AthleteClubIsStored ||
                !importAthleteAction.AthleteClubIsUpToDate)
            {
                Action<string, Guid> TweetAction = (tweet, athleteClubGuid) =>
                                                   {
                                                       //no tweeting when mass importing
                                                   };
                athleteMgmt.SaveAthlete(importAthleteAction.AthleteClub.Inti_Athlete.FirstName,
                                        importAthleteAction.AthleteClub.Inti_Athlete.LastName,
                                        importAthleteAction.AthleteClub.Inti_Athlete.GUID,
                                        SelectedTournament,
                                        importAthleteAction.AthleteClub.ClubGUID,
                                        importAthleteAction.AthleteClub.PositionGUID,
                                        importAthleteAction.AthleteClub.Price ?? 0,
                                        true, importAthleteAction.AthleteClub.Inti_Club.ShortName, TweetAction);
            }

            return true;
        }
    }


    public class ImportAthleteAction
    {
        public string InString { get; set; }

        public Inti_AthleteClub AthleteClub { get; set; }

        public bool AthleteIsStored { get; set; }

        public bool AthleteClubIsStored { get; set; }

        public bool AthleteClubIsUpToDate { get; set; }

        public string ImportAction { get; set; }

        public Exception Exception { get; set; }

        public bool Done { get; set; }

        
    }

    public class AthleteToImport
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid AthleteGuid { get; set; }
    }

}
