using System.Collections.Generic;
using System.Linq;

namespace inti2008.Data
{
    public class CommonDataSources : IntiManagementBase
    {
        public CommonDataSources(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public IList<Inti_Tournament> AllTournaments()
        {
            IList<Inti_Tournament> listToReturn = null;

            using (var db = new IntiDataContext(_connectionString))
            {
                var tours = from t in db.Inti_Tournament
                            select t;

                listToReturn = tours.OrderByDescending(t => t.EndRegistration).ToList();
            }

            return listToReturn;
        }
    }
}