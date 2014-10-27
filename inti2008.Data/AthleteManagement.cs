using System;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Security.Cryptography;

namespace inti2008.Data
{
    public class AthleteManagement : IntiManagementBase
    {
        public AthleteManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public Inti_Athlete GetAthleteByName(string firstName, string lastName)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var athletes = from a in db.Inti_Athlete
                               where a.FirstName.ToLower().Trim() == firstName.ToLower().Trim()
                                     && a.LastName.ToLower().Trim() == lastName.ToLower().Trim()
                               select a;

                if (athletes.ToList().Count == 1)
                {
                    return athletes.ToList()[0];
                }

                if (athletes.ToList().Count > 1)
                {
                    throw new ArgumentException("More than one player is named {0}. Correct this in player data.", String.Format("{0} {1}", firstName, lastName).Trim());
                }
            }

            return null;
        }

        public Inti_AthleteClub GetAthleteClubByTournament(Guid athleteId, Guid tournamentId)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var dlo = new DataLoadOptions();
                dlo.LoadWith<Inti_AthleteClub>(ac => ac.Inti_Club);
                dlo.LoadWith<Inti_AthleteClub>(ac => ac.Inti_Position);
                dlo.LoadWith<Inti_AthleteClub>(ac => ac.Inti_Athlete);
                db.LoadOptions = dlo;

                var athleteClubs = from ac in db.Inti_AthleteClub
                                   where ac.AthleteGUID == athleteId &&
                                         ac.Inti_Club.TournamentGUID == tournamentId
                                   select ac;
                
                if(athleteClubs.ToList().Count == 1)
                {
                    var athleteClub = athleteClubs.ToList()[0];

                    return athleteClub;
                }

                if (athleteClubs.ToList().Count > 1)
                {
                    throw new ArgumentException("More than one club for the athlete with id {0} in the same tournament.", athleteId.ToString());
                }
            }

            return null;
        }

        private bool AthleteHasNameDuplicate(IntiDataContext db, Guid athleteGUID, string firstName, string lastName)
        {
            var athleteTest = from a in db.Inti_Athlete
                              where a.FirstName.ToLower().Trim() == firstName.ToLower().Trim()
                                    && a.LastName.ToLower().Trim() == lastName.ToLower().Trim()
                                    && (a.GUID != athleteGUID || athleteGUID == Guid.Empty)
                              select a;

            if(athleteTest.ToList().Count > 0)
            {
                return true;
            }

            return false;
        }

        public Guid SaveAthlete(string firstName, string lastName, Guid playerGuid, Guid tournamentGuid, Guid clubGuid, Guid positionGuid, int price, bool isActive, string clubCode, Action<string, Guid> notifyCommunityAction)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                
                Inti_Athlete athlete;
                                

                if (playerGuid != Guid.Empty)
                {
                    try
                    {
                        athlete = db.Inti_Athlete.Single(a => a.GUID == new Guid(playerGuid.ToString()));
                    }
                    catch (Exception)
                    {
                        //not saved athlete
                        athlete = null;
                    }    
                }
                else
                {
                    athlete = null;
                }

                var newAthlete = athlete == null;
                var newInTournament = true;

                //check if athlete with same name already exists
                if (AthleteHasNameDuplicate(db, playerGuid, firstName, lastName))
                    throw new DuplicateNameException(String.Format("There is already a player with the name {0} {1}", firstName, lastName));
                
                if(athlete == null)
                {
                    athlete = new Inti_Athlete();
                    db.Inti_Athlete.InsertOnSubmit(athlete);
                }

                athlete.FirstName = firstName;
                athlete.LastName = lastName;

                var fired = false;
                var isManager = false;
                var changedClub = false;
                var athleteClubGuid = Guid.Empty;
                Inti_AthleteClub athleteClub;

                //add clubs/tournaments
                if (clubGuid != Guid.Empty && positionGuid != Guid.Empty)
                {
                    bool newAthleteClub = false;

                    //save club/tournament setting
                    try
                    {
                        athleteClub = db.Inti_AthleteClub.Single(ac => ac.AthleteGUID == athlete.GUID && ac.Inti_Club.TournamentGUID == tournamentGuid);
                    }
                    catch (Exception)
                    {
                        //no athleteClub for this athlete/tournament
                        //new settings
                        newAthleteClub = true;
                        athleteClub = new Inti_AthleteClub();
                        athleteClub.AthleteGUID = athlete.GUID;
                    }

                    fired = !isActive && (athleteClub.IsActive ?? true);
                    
                    changedClub = clubGuid != athleteClub.ClubGUID;
                    athleteClub.ClubGUID = clubGuid;
                    athleteClub.PositionGUID = positionGuid;

                    

                    athleteClub.Price = price;
                    athleteClub.IsActive = isActive;

                    if (newAthleteClub)
                        db.Inti_AthleteClub.InsertOnSubmit(athleteClub);

                    newInTournament = newAthleteClub;

                    athleteClubGuid = athleteClub.GUID;
                }

                db.SubmitChanges();

                //refresh object
                athleteClub = db.Inti_AthleteClub.Single(ac => ac.AthleteGUID == athlete.GUID && ac.Inti_Club.TournamentGUID == tournamentGuid);
                isManager = athleteClub.Inti_Position.ShortName == "MGR";

                //notify community
                if (newAthlete || newInTournament)
                {
                    notifyCommunityAction(String.Format("{0} {1} ({2}) har lagts till", firstName, lastName, clubCode), athleteClubGuid);
                }
                else
                {
                    //fired?
                    if (fired)
                    {
                        if (isManager)
                        {
                            notifyCommunityAction(String.Format("{0} {1} har fått sparken.", firstName, lastName),
                                athleteClubGuid);
                        }
                        else
                        {
                            //notifyCommunityAction(String.Format("{0} {1} har fått sparken.", firstName, lastName),
                            //    athleteClubGuid);
                        }
                        
                    }
                    else
                    {
                        if (changedClub)
                        {
                            notifyCommunityAction(String.Format("{0} {1} har gått över till {2}.", firstName, lastName, clubCode),
                                athleteClubGuid);    
                        }
                    }
                }

                playerGuid = athlete.GUID;
            }

            return playerGuid;
        }


        public void MergeAthlete(Guid destinationAthleteGUID, Guid sourceAthleteGUID)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                
                var destinationAthlete = db.Inti_Athlete.Single(a => a.GUID == destinationAthleteGUID);
                var sourceAthlete = db.Inti_Athlete.Single(a => a.GUID == sourceAthleteGUID);

                //check for conflicting athleteclub-references - both instances of the athlete can't be present in the same tournament
                foreach (var destAthleteClub in destinationAthlete.Inti_AthleteClub)
                {
                    if (
                        sourceAthlete.Inti_AthleteClub.Any(
                            ac => ac.Inti_Club.TournamentGUID == destAthleteClub.Inti_Club.TournamentGUID))
                    {
                        var tournament =
                            db.Inti_Tournament.Single(t => t.GUID == destAthleteClub.Inti_Club.TournamentGUID);
                        throw new Exception("Conflict in tournament: " + tournament.Name);
                    }
                }


                

                //move athletes club-references to the dest athlete


                //move athletes team-references to the dest athlete

                //move all referenced point events to the dest athlete

                //delete all of the old
            }
            
        }
    }
}