using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using inti2008.Data;

namespace inti2008.Web
{
    /// <summary>
    /// Summary description for Completion
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Completion : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] CompletePlayers(string prefixText, int count)
        {

            string[] result = new string[count];

            IList<Inti_Athlete> players;
            using (var db = Global.GetConnection())
            {
                players = Searching.SearchForAthletes(db, prefixText);
            }

            prefixText = prefixText.ToLower();

            count = (players.Count() > count) ? count : players.Count();
            result = new string[count];

            var i = 0;
            foreach (var athlete in players)
            {
                if (athlete.FirstName.ToLower().StartsWith(prefixText))
                    result[i] = String.Format("{0} {1}", athlete.FirstName, athlete.LastName);
                else
                {
                    if (athlete.LastName.ToLower().StartsWith(prefixText))
                        result[i] = String.Format("{0}, {1}", athlete.LastName, athlete.FirstName);
                }

                i++;

                if (i >= count)
                    break;
            }

            return result;
            
        }
    }
}
