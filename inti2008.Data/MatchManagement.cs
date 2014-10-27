using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace inti2008.Data
{
    public enum EventType
    {
        Create,
        Change,
        Delete,
        Update,
        Read
    }

    public class MatchManagement : IntiManagementBase
    {
        private readonly Random _random = new Random(DateTime.Now.Millisecond);

        public MatchManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public void SetUpdater(Guid matchId)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var match = db.Inti_Match.Single(m => m.GUID == matchId);

                match.Updater = SessionProperties.UserGuid;
                match.StartUpdateDate = DateTime.Now;

                db.SubmitChanges();
            }
        }
        
        /// <summary>
        /// Compile a text string with the result and top user Team
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public string GetMatchText(Guid matchId)
        {
            var outString = String.Empty;

            using(var db = new IntiDataContext(_connectionString))
            {
                var match = db.Inti_Match.Single(m => m.GUID == matchId);

                if (match.IsUpdated ?? false)
                {
                    outString += String.Format("{0}-{1} ({2}-{3}) uppdaterad! ", match.HomeClubInti_Club.ShortName,
                                           match.Inti_Club.ShortName, match.HomeScore, match.AwayScore);

                    outString += GetRandomMatchFact(matchId);    
                }

                


            }

            return outString;
        }

        public string GetRandomMatchFact(Guid matchGuid)
        {
            var facts = GetMatchFacts(matchGuid);

            
            return facts[_random.Next(facts.Count)];
        }

        public List<string> GetMatchFacts(Guid matchGuid)
        {

            var facts = new List<string>();

            using (var db = new IntiDataContext(_connectionString))
            {
                //get teams that scored in the match
                var teams = from tpe in db.Inti_TeamPointEvents
                            where tpe.Inti_MatchPointEvent.Inti_Match.GUID == matchGuid
                            group tpe by new
                            {
                                tpe.Inti_Team.GUID,
                                tpe.Inti_Team.Name
                            }
                                into ut

                                select new
                                {
                                    ut.Key.GUID,
                                    ut.Key.Name,
                                    Points = ut.Sum(x => x.Inti_MatchPointEvent.Points)
                                };

                //get the pointevents - could be nice to look at the players that were given points
                var athletes = from mpe in db.Inti_MatchPointEvent
                    where mpe.Inti_Match.GUID == matchGuid
                    group mpe by new
                                 {
                                     mpe.Inti_AthleteClub.Inti_Athlete.GUID,
                                     mpe.Inti_AthleteClub.Inti_Athlete.FirstName,
                                     mpe.Inti_AthleteClub.Inti_Athlete.LastName,

                                 }
                    into athlete

                    select new
                           {
                               athlete.Key.GUID,
                               athlete.Key.FirstName,
                               athlete.Key.LastName,
                               Points = athlete.Sum(x => x.Points)
                           };


                if (athletes.ToList().Count == 0)
                {
                    facts.Add("Ingen spelare fick något poäng");
                }
                else
                {
                    //top performing athletes
                    var maxAthletePoint = athletes.Max(a => a.Points);
                    
                    var topAthletes = athletes.Where(a => a.Points == maxAthletePoint).ToList();

                    foreach (var topAthlete in topAthletes)
                    {
                        facts.Add(String.Format("{0} drog in {1} poäng till sina lag.", (topAthlete.FirstName + " " + topAthlete.LastName).Trim(), topAthlete.Points));
                        facts.Add(String.Format("{0} fixade {1} poäng. Har du honom?", (topAthlete.FirstName + " " + topAthlete.LastName).Trim(), topAthlete.Points));
                    }

                    //bottom athletes (if negative)
                    var minAthletePoint = athletes.Min(a => a.Points);
                    if (minAthletePoint < 0)
                    {
                        var minAthletes = athletes.Where(a => a.Points == minAthletePoint).ToList();
                        foreach (var minAthlete in minAthletes)
                        {
                            facts.Add(String.Format("{0} fick {1} minuspoäng.", (minAthlete.FirstName + " " + minAthlete.LastName).Trim(), Math.Abs(minAthlete.Points)));
                            facts.Add(String.Format("{0} sänkte sina lag med {1} poäng.", (minAthlete.FirstName + " " + minAthlete.LastName).Trim(), Math.Abs(minAthlete.Points)));
                        }
                    }
                }
                
                //no points?
                if (teams.ToList().Count == 0)
                {
                    facts.Add("Alla kammande noll");
                    facts.Add("Ingen fick några poäng");
                    facts.Add("Noll poäng utdelade i matchen");
                }
                else
                {
                    var maxPoint = teams.Max(t => t.Points);
                    var minPoint = teams.Min(t => t.Points);


                    var topTeams = teams.Where(t => t.Points == maxPoint).ToList();
                    var bottomTeams = teams.Where(t => t.Points == minPoint).ToList();

                    //vinnare
                    if (maxPoint > 0)
                    {
                        if (topTeams.Count == 1)
                        {
                            //ensam vinnare
                            facts.Add(String.Format("{0} var bäst och tog {1} poäng", topTeams[0].Name, maxPoint));
                            facts.Add(String.Format("{0} var bäst och drog in {1} poäng", topTeams[0].Name, maxPoint));
                            facts.Add(String.Format("{0} var herre på täppan med {1} poäng", topTeams[0].Name, maxPoint));
                            facts.Add(String.Format("{0} tjänade in {1} poäng", topTeams[0].Name, maxPoint));
                            facts.Add(String.Format("{0} toppade med {1} poäng", topTeams[0].Name, maxPoint));
                        }
                        else
                        {
                            //flera vinnare
                            facts.Add(String.Format("{0} lag var bäst med {1} poäng", topTeams.Count, maxPoint));
                            facts.Add(String.Format("{0} lag ökade med {1} poäng", topTeams.Count, maxPoint));
                        }
                    }
                    //minuspoäng?
                    if (minPoint < 0)
                    {
                        if (bottomTeams.Count == 1)
                        {
                            facts.Add(String.Format("{0} var sämst och backade {1} poäng",
                                                 bottomTeams[0].Name, minPoint * -1));
                            facts.Add(String.Format("{0} förlorade {1} poäng",
                                                 bottomTeams[0].Name, minPoint * -1));
                        }
                        else
                        {
                            //flera på minuspoäng
                            facts.Add(String.Format("{0} lag fick {1} minuspoäng", bottomTeams.Count, minPoint * -1));
                        }
                    }

                }
            }

            return facts;
        }

        public void DistributePoints(Guid matchId)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var match = db.Inti_Match.Single(m => m.GUID == matchId);

                var mpes = from mpe in db.Inti_MatchPointEvent
                           where mpe.Inti_Match.GUID == matchId
                           orderby mpe.Inti_AthleteClub.Inti_Club.Name
                           orderby mpe.Inti_AthleteClub.Inti_Position.Name
                           orderby mpe.Inti_PointEvent.Name
                           select mpe;

                foreach (var mpe in mpes)
                {
                    //loop the teams that has this athlete
                    var teamAthletes = from ta in db.Inti_TeamAthlete
                                       where ta.Inti_TeamVersion.ValidFrom < match.MatchDate
                                             && (ta.Inti_TeamVersion.ValidTo ?? match.MatchDate) >= match.MatchDate
                                             && (ta.Inti_TeamVersion.Inti_Team.IsActive ?? false)
                                             && (ta.Inti_TeamVersion.Inti_Team.IsPaid ?? false)
                                             && ta.AthleteGUID == mpe.AthleteClubGUID
                                       select new
                                       {
                                           ta.Inti_TeamVersion.TeamGUID
                                       };

                    foreach (var teamAthlete in teamAthletes)
                    {
                        var tpe = new Inti_TeamPointEvents();
                        tpe.MatchPointEventGUID = mpe.GUID;
                        tpe.TeamGUID = teamAthlete.TeamGUID;

                        db.Inti_TeamPointEvents.InsertOnSubmit(tpe);
                    }
                }

                match.IsUpdated = true;

                db.SubmitChanges();

                LogEvent(matchId, typeof(Inti_Match), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "DistributePoints" );
            }
        }

        public void ClearPointEvents(Guid matchId)
        {

            //always clear teamPoints first
            ClearTeamPoints(matchId);

            using (var db = new IntiDataContext(_connectionString))
            {
                var matchPointEvents = from mpe in db.Inti_MatchPointEvent
                                       where mpe.MatchGUID == matchId
                                       select mpe;

                db.Inti_MatchPointEvent.DeleteAllOnSubmit(matchPointEvents.ToList());

                var match = db.Inti_Match.Single(m => m.GUID == matchId);
                match.IsUpdated = false;

                LogEvent(matchId, typeof(Inti_Match), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "ClearPointEvents");

                db.SubmitChanges();
            }
        }

        public void ClearTeamPoints(Guid matchId)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var teamPointEvents = from tpe in db.Inti_TeamPointEvents
                                      where tpe.Inti_MatchPointEvent.MatchGUID == matchId
                                      select tpe;

                db.Inti_TeamPointEvents.DeleteAllOnSubmit(teamPointEvents.ToList());

                if (teamPointEvents.ToList().Count > 0)
                    LogEvent(matchId, typeof(Inti_Match), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "ClearTeamPoints");

                db.SubmitChanges();
            }
        }

    }
}
