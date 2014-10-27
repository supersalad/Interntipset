using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class UserTeamUploadImage : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var team = new UserTeamManagement(Global.ConnectionString, SessionProps).GetTeam(TeamId);

                //check that it's the users team
                //verify team owner
                if (team.UserGUID != SessionProps.UserGuid && !SessionProps.HasPermission("ADMIN"))
                {
                    //log the attempted breach
                    MailAndLog.SendMessage("Försök att sabba lag",
                        String.Format("Användaren: {0} med guid: {1} försökte ändra bild på laget: {2} med guid: {3}", SessionProps.UserName, SessionProps.UserGuid.ToString(), team.Name, team.GUID),
                        Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
                    throw new AccessViolationException("Attempt to tamper with other users team");
                }

                if (!String.IsNullOrEmpty(team.Picture))
                {
                    uploadImage.UploadUserImage(team.Picture);
                }
                else
                {
                    uploadImage.UploadUserImage();
                }
            }
        }

        private Guid TeamId
        {
            get
            {
                if (ViewState["teamGUID"] != null)
                    return new Guid(ViewState["teamGUID"].ToString());

                var value = this.GetRedirectParameter("teamGUID", false);
                if (value == null)
                    return Guid.Empty;
                else
                    return new Guid(value.ToString());
            }
        }

        protected void uploadImage_ImageUploaded()
        {
            //store the new image name
            var trans = new UserTeamManagement(Global.ConnectionString, SessionProps);

            trans.UpdateTeamImage(TeamId, uploadImage.ImageNameToStore);

            uploadImage.Visible = false;

            lblMessage.Text = "Bilden är uppladdad...";
        }

        protected void uploadImage_Cancel()
        {
            uploadImage.Visible = false;

            lblMessage.Text = "Du behåller din gamla bild...";
        }
    }
}
