using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inti2008.Data
{
    public static class Searching
    {
        public static IList<Inti_Athlete>SearchForAthletes(IntiDataContext db, string searchString)
        {
            var splitString = searchString.Replace(",", "").Split(new char[] { ' ' });

            IQueryable<Inti_Athlete> athletes = null;

            if (splitString.Length == 1)
                athletes = from a in db.Inti_Athlete
                           where a.FirstName.StartsWith(splitString[0]) || a.LastName.StartsWith(splitString[0])
                           select a;

            if (splitString.Length >= 2)
                athletes = from a in db.Inti_Athlete
                           where (a.FirstName.StartsWith(splitString[0]) && a.LastName.StartsWith(splitString[1]))
                           || (a.FirstName.StartsWith(splitString[1]) && a.LastName.StartsWith(splitString[0]))
                           select a;

            if (athletes != null)
                return athletes.ToList();

            return null;
        }
    }
}
