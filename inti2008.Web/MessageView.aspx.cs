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
    public partial class MessageView : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadMessage();
        }

        private void LoadMessage()
        {
            var messageGUID = this.GetRedirectParameter("messageGUID", false);
            if (messageGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    var message = db.Ext_Message.Single(m => m.GUID == new Guid(messageGUID.ToString()));

                    //safety 

                    foreach(var rec in message.Ext_MessageRecipient)
                    {
                        if (rec.RecipientUserGUID == SessionProps.UserGuid)
                        {
                            if(rec.ReadOn == null)
                            {
                                rec.ReadOn = DateTime.Now;

                                db.SubmitChanges();
                            }

                            lblHeader.Text = message.Header;
                            lblInformation.Text = String.Format("Från: {0} {1}. Skickat: {2}", message.Sys_User.FirstName,
                                                                message.Sys_User.LastName, (message.SentDate ?? DateTime.Now).ToShortDateString());
                            lblBody.Text = Server.HtmlEncode(message.Body).Replace("\n","<br>");

                            break;
                        }
  
                    }

                    
                }
            }
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var messageGUID = this.GetRedirectParameter("messageGUID", false);
            if (messageGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    var message = db.Ext_Message.Single(m => m.GUID == new Guid(messageGUID.ToString()));

                    //safety 

                    foreach (var rec in message.Ext_MessageRecipient)
                    {
                        if (rec.RecipientUserGUID == SessionProps.UserGuid)
                        {
                            db.Ext_MessageRecipient.DeleteOnSubmit(rec);

                            db.SubmitChanges();

                            break;
                        }
                    }
                }

                WebControlManager.RedirectWithQueryString("UserMessages.aspx", new[] { "GUID" }, new[] { SessionProps.UserGuid.ToString() });
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            var messageGUID = this.GetRedirectParameter("messageGUID", false);
            if (messageGUID != null)
            {
                using (var db = Global.GetConnection())
                {
                    var message = db.Ext_Message.Single(m => m.GUID == new Guid(messageGUID.ToString()));

                    //safety 

                    foreach (var rec in message.Ext_MessageRecipient)
                    {
                        if (rec.RecipientUserGUID == SessionProps.UserGuid)
                        {
                            WebControlManager.RedirectWithQueryString("MessageCreate.aspx", new string[] { "msgSubject", "messageGUID" }, new string[] { String.Format("Sv: {0}", message.Header), message.GUID.ToString() });

                            break;
                        }
                    }
                }
            }
        }
    }
}
