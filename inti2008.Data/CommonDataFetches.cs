using System.Linq;
using System;

namespace inti2008.Data
{
    public class CommonDataFetches : IntiManagementBase
    {
        public CommonDataFetches(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public Inti_Position GetPosition(string positionCode)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                return db.Inti_Position.Single(p => p.ShortName == positionCode);
            }
        }

        /// <summary>
        /// Return the current transfer period if any
        /// </summary>
        /// <returns></returns>
        public Inti_TransferPeriod GetCurrentTransferPeriod(DateTime date)
        {
            
            using (var db = new IntiDataContext(_connectionString))
            {
                var transferPeriods = db.Inti_TransferPeriod.Where(tf => tf.TournamentGUID == SessionProperties.SelectedTournament.GUID).ToList();
                foreach (var tf in transferPeriods)
                {
                    if (tf.StartDate <= date && tf.EndDate >= date)
                    {
                        return tf;
                    }
                }
            }
            
            return null;
        }
    }
}