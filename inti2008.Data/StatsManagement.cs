using System;
using System.Collections.Generic;
using System.Linq;

namespace inti2008.Data
{
    public class StatsManagement : IntiManagementBase
    {
        public StatsManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public Dictionary<Sys_User, int> GetTopUpdaters(Guid tournamentId)
        {
            var updatersFullUserObjects = new Dictionary<Sys_User, int>(10);
            using (var db = new IntiDataContext(_connectionString))
            {
                var matches = db.Inti_Match.Where(m => m.TournamentGUID == tournamentId && (m.IsUpdated ?? false));

                var updaters = new Dictionary<Guid, int>(10);

                //lock in the object log of each updated match - get the earliest DistributePoints - that's the one that counts.
                foreach (var intiMatch in matches)
                {
                    var log =
                        db.Ext_ChangeLog.Where(l => l.ObjectGUID == intiMatch.GUID && l.Message == "DistributePoints").
                            OrderBy(l => l.LogDate).FirstOrDefault();

                    if (log == null) continue;

                    if (!updaters.ContainsKey(log.UserGUID))
                        updaters.Add(log.UserGUID, 0);

                    updaters[log.UserGUID]++;

                    
                }

                //populate users list
                foreach (var updater in updaters)
                {
                    var user = db.Sys_User.SingleOrDefault(u => u.GUID == updater.Key);
                    if (user == null) continue;

                    updatersFullUserObjects.Add(user, updater.Value);
                }
            }

            return updatersFullUserObjects;
        }
    }
}