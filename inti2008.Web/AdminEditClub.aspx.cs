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
    public partial class AdminEditClub : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_TOURMASTER");

            if (!IsPostBack)
                LoadClub();
        }

        private void LoadClub()
        {
            var clubGuid = this.GetRedirectParameter("clubGUID", false);
            var tourGuid = this.GetRedirectParameter("tourGUID", false);

            if (clubGuid != null)
            {
                //read the club info
                using (var db = Global.GetConnection())
                {
                    var club = db.Inti_Club.Single(c => c.GUID == new Guid(clubGuid.ToString()));

                    ClubName.Text = club.Name;
                    ClubDescription.Text = club.Description;
                    ClubShortName.Text = club.ShortName;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("AdminTournamentEdit.aspx", new string[]{"GUID"}, new string[]{this.GetRedirectParameter("tourGUID", false).ToString()} );
        }

        protected void brnSave_Click(object sender, EventArgs e)
        {
            //save the club
            var clubGuid = this.GetRedirectParameter("clubGUID", false);
            var tourGuid = this.GetRedirectParameter("tourGUID", false);

            using (var db = Global.GetConnection())
            {
                Inti_Club club;
                if (clubGuid == null)
                {
                    club = new Inti_Club();
   
                    club.TournamentGUID = new Guid(tourGuid.ToString());
                }
                else
                    club = db.Inti_Club.Single(c => c.GUID == new Guid(clubGuid.ToString()));


                club.Name = ClubName.Text;
                club.Description = ClubDescription.Text;
                club.ShortName = ClubShortName.Text;

                if (clubGuid == null)
                    db.Inti_Club.InsertOnSubmit(club);

                db.SubmitChanges();

                if (clubGuid == null)
                {
                    WebControlManager.RedirectWithQueryString("AdminEditClub.aspx", new string[] { "clubGuid" }, new string[] { club.GUID.ToString() });
                }
            }

        }
    }
}
