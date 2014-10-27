using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class PlayerList : IntiPage
    {
        public override bool AuthenticationIsRequired
        {
            get
            {
                return false;
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadClubs();

                LoadPositions();
            }
        }

        private void LoadClubs()
        {
            using (var db = Global.GetConnection())
            {
                var clubs = from c in db.Inti_Club
                            where c.TournamentGUID == SessionProps.SelectedTournament.GUID
                            select c;

                drpClubs.DataValueField = "GUID";
                drpClubs.DataTextField = "Name";
                drpClubs.DataSource = clubs.OrderBy(c => c.Name).ToList();
                drpClubs.DataBind();

                drpClubs.Items.Insert(0, new ListItem("-alla-", ""));
            }
        }

        private void LoadPositions()
        {
            using (var db = Global.GetConnection())
            {
                var pos = from p in db.Inti_Position
                            where (SessionProps.SelectedTournament.IncludeManager || p.ShortName != "MGR")
                            select p;

                drpPosition.DataValueField = "GUID";
                drpPosition.DataTextField = "Name";
                drpPosition.DataSource = pos.OrderBy(p => p.ShortName).ToList();
                drpPosition.DataBind();

                drpPosition.Items.Insert(0, new ListItem("-alla-", ""));
            }
        }


        protected void btnShowPlayers_Click(object sender, EventArgs e)
        {
            using(var db = Global.GetConnection())
            {
                var clubGUID = drpClubs.SelectedValue == "" ? Guid.Empty : new Guid(drpClubs.SelectedValue);
                var posGUID = drpPosition.SelectedValue == "" ? Guid.Empty : new Guid(drpPosition.SelectedValue);


                var players = from ac in db.Inti_AthleteClub
                              where (clubGUID == Guid.Empty || ac.ClubGUID == clubGUID)
                                    && (posGUID == Guid.Empty || ac.PositionGUID == posGUID)
                                    && ac.Inti_Club.TournamentGUID == SessionProps.SelectedTournament.GUID
                              select new
                                         {
                                             ac.GUID,
                                             Name = ac.Inti_Athlete.FirstName + " " + ac.Inti_Athlete.LastName,
                                             Club = ac.Inti_Club.Name,
                                             Position = ac.Inti_Position.Name,
                                             Price = ac.Price,
                                             Points = (ac.Inti_MatchPointEvent.Count > 0 ? ac.Inti_MatchPointEvent.Sum(mpe=>mpe.Points) : 0),
                                             CustomClass = !ac.IsActive ?? false ? "list-group-item-danger" : ""
                                         };
                
                rptPlayers.DataSource = players.OrderByDescending(p => p.Points).ToList();
                rptPlayers.DataBind();
            }
        }

    }
}
