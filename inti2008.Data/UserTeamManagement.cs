using System;
using System.Collections.Generic;
using System.Linq;

namespace inti2008.Data
{
    public class UserTeamManagement : IntiManagementBase
    {
        public UserTeamManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public IList<Inti_Team>GetTeamsForUser(Guid user, Guid tournament)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var teams = from t in db.Inti_Team
                            where t.Sys_User.GUID == user
                            && t.Inti_Tournament.GUID == tournament
                            select t;

                return teams.ToList();
            }
        }

        public void ActivateTeam(Guid teamGUID)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                try
                {
                    var team = db.Inti_Team.Single(t => t.GUID == teamGUID);

                    team.IsActive = !team.IsActive;

                    db.SubmitChanges();

                }
                catch (System.InvalidOperationException)
                {
                    //team was not to be found
                }
            }
        }

        public void MarkTeamAsPaid(Guid teamGUID)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                try
                {
                    var team = db.Inti_Team.Single(t => t.GUID == teamGUID);

                    team.IsPaid = !team.IsPaid;

                    db.SubmitChanges();

                }
                catch (System.InvalidOperationException)
                {
                    //team was not to be found
                }
            }
        }

        public void ToggleBonus(Guid teamGUID)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                try
                {
                    var team = db.Inti_Team.Single(t => t.GUID == teamGUID);

                    team.BonusPoints = team.BonusPoints.HasValue && team.BonusPoints.Value == 2 ? 0 : 2;

                    db.SubmitChanges();

                }
                catch (System.InvalidOperationException)
                {
                    //team was not to be found
                }
            }
        }

        public void CommitTransfers(IntiDataContext db, Inti_TransferPeriod transferPeriod, Inti_TeamVersion currentVersion, Inti_TeamVersion transferVersion, Guid teamId)
        {
            //add transfers
            IList<Inti_TeamTransfer> transfers = new List<Inti_TeamTransfer>();
            foreach (var teamAthlete in currentVersion.Inti_TeamAthlete.ToList())
            {
                //is this athlete removed in the new version?
                if (transferVersion.Inti_TeamAthlete.Where(ta => ta.AthleteGUID == teamAthlete.AthleteGUID).ToList().Count == 0)
                {
                    var transfer = new Inti_TeamTransfer();
                    transfer.TeamGUID = teamId;
                    transfer.TransferDate = transferPeriod.EndDate;
                    transfer.AthleteOutGUID = teamAthlete.AthleteGUID;
                    transfer.AthleteInGUID = Guid.Empty;
                    transfers.Add(transfer);
                }

            }

            foreach (var teamAthlete in transferVersion.Inti_TeamAthlete.ToList())
            {
                //is this athlete present in the old version?
                if (currentVersion.Inti_TeamAthlete.Where(ta => ta.AthleteGUID == teamAthlete.AthleteGUID).ToList().Count == 0)
                {
                    foreach (var tr in transfers)
                    {
                        if (tr.AthleteInGUID == Guid.Empty)
                        {
                            tr.AthleteInGUID = teamAthlete.AthleteGUID;
                            db.Inti_TeamTransfer.InsertOnSubmit(tr);
                            break;
                        }
                    }
                }

            }

            //update validTo and ValidFroms
            transferVersion.ValidFrom = transferPeriod.EndDate;
            currentVersion.ValidTo = transferPeriod.EndDate.AddDays(-1);

            db.SubmitChanges();

            LogEvent(teamId, typeof(Inti_Team), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "Genomfört byten");
        }

        public void RemoveDuplicateTransfers(IntiDataContext db, Guid teamId)
        {
            
            var summary = from tf in db.Inti_TeamTransfer
                where tf.TeamGUID == teamId
                group tf by new {tf.TransferDate, tf.AthleteInGUID, tf.AthleteOutGUID}
                into dup
                select new {dup.Key.TransferDate,
                    dup.Key.AthleteInGUID,
                    dup.Key.AthleteOutGUID,
                    Count = dup.Count()
                };

            var duplicates = summary.Where(s => s.Count > 1).ToList();

            foreach (var duplicate in duplicates)
            {
                //remove one of the duplicates
                var toDelete = db.Inti_TeamTransfer.FirstOrDefault(
                    tf =>
                        tf.TeamGUID == teamId && tf.TransferDate == duplicate.TransferDate &&
                        tf.AthleteInGUID == duplicate.AthleteInGUID && tf.AthleteOutGUID == duplicate.AthleteOutGUID);

                if (toDelete == null) continue;

                db.Inti_TeamTransfer.DeleteOnSubmit(toDelete);
            }

            db.SubmitChanges();
        }

        public void ReloadTransferVersion(IntiDataContext db, Inti_TeamVersion transferVersion, Inti_TeamVersion currentVersion, Guid teamId)
        {
            //delete the ones in transferversion
            var teamAthletesToDelete = transferVersion.Inti_TeamAthlete.ToList();
            db.Inti_TeamAthlete.DeleteAllOnSubmit(teamAthletesToDelete);
            db.SubmitChanges();

            //get players from current version
            foreach (var ta in currentVersion.Inti_TeamAthlete.ToList())
            {
                var newTa = new Inti_TeamAthlete();
                newTa.AthleteGUID = ta.AthleteGUID;
                newTa.TeamVersionGUID = transferVersion.GUID;
                db.Inti_TeamAthlete.InsertOnSubmit(newTa);

                db.SubmitChanges();
            }
        }

        public void ReOpenTeamForTransfers(IntiDataContext db, Inti_TransferPeriod transferPeriod, Inti_TeamVersion transferVersion, Inti_TeamVersion currentVersion, Guid teamId)
        {
            //remove transfers
            var transfers =
                db.Inti_TeamTransfer.Where(
                    tf => tf.TeamGUID == teamId && tf.TransferDate == transferPeriod.EndDate).ToList();
            if (transfers.Count > 0)
                db.Inti_TeamTransfer.DeleteAllOnSubmit(transfers);

            //update validTo and ValidFroms
            transferVersion.ValidFrom = null;
            currentVersion.ValidTo = null;

            db.SubmitChanges();

            LogEvent(teamId, typeof(Inti_Team), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "Ångrat byten");
        }

        public void UpdateTeamImage(Guid teamGUID, string imageName)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var team = db.Inti_Team.Single(t => t.GUID == teamGUID);

                team.Picture = imageName;

                db.SubmitChanges();

                LogEvent(teamGUID, typeof(Inti_Team), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "Ändrat bild");
            }
        }

        /// <summary>
        /// Returns a disconnected team
        /// </summary>
        /// <param name="teamGUID"></param>
        /// <returns></returns>
        public Inti_Team GetTeam(Guid teamGUID)
        {
            Inti_Team team;

            using (var db = new IntiDataContext(_connectionString))
            {
                team = db.Inti_Team.Single(t => t.GUID == teamGUID);
            }

            return team;
        }

        public void ToggleUserFavoriteTeam(Guid teamGUID)
        {
            if (SessionProperties.UserGuid == Guid.Empty) return;

            using(var db = new IntiDataContext(_connectionString))
            {
                var userFavoriteTeam = from uft in db.Ext_UserFavoriteTeam
                                       where uft.UserGUID == SessionProperties.UserGuid &&
                                       uft.TeamGUID == teamGUID
                                       select uft;

                if (userFavoriteTeam.ToList().Count > 0)
                    db.Ext_UserFavoriteTeam.DeleteAllOnSubmit(userFavoriteTeam);
                else
                {
                    var newUserFavoriteTeam = new Ext_UserFavoriteTeam();
                    newUserFavoriteTeam.UserGUID = SessionProperties.UserGuid;
                    newUserFavoriteTeam.TeamGUID = teamGUID;

                    db.Ext_UserFavoriteTeam.InsertOnSubmit(newUserFavoriteTeam);
                }

                db.SubmitChanges();
            }
        }
    }
}