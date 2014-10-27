using System;
using System.Linq;

namespace inti2008.Data
{
    public class ClubManagement : IntiManagementBase
    {
        public ClubManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public Inti_Club GetClubByCode(Guid selectedTournament, string clubCode)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var club = db.Inti_Club.Single(c => c.ShortName == clubCode &&
                                                     c.TournamentGUID == selectedTournament);

                return club;
            }
        }
    }
}