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
    public partial class MessageCreate : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRecipients();

                LoadHeader();    
            }

            btnCancel.Attributes.Add("onclick", "window.history.go(-1);");
            
        }

        private void LoadHeader()
        {
            var header = this.GetRedirectParameter("msgSubject", false);
            if (header != null)
                Header.Text = header.ToString();
        }

        private void LoadRecipients()
        {
            using (var db = Global.GetConnection())
            {

                var messageGUID = this.GetRedirectParameter("messageGUID", false);

                

                var users = from u in db.Sys_User
                            select new
                            {
                                u.GUID,
                                Name = u.FirstName + " " + u.LastName
                            };

                drpRecipient.DataValueField = "GUID";
                drpRecipient.DataTextField = "Name";
                drpRecipient.DataSource = users.OrderBy(u => u.Name).ToList();
                drpRecipient.DataBind();

                if (messageGUID != null)
                {
                    var message = db.Ext_Message.Single(m => m.GUID == new Guid(messageGUID.ToString()));

                    drpRecipient.ClearSelection();
                    drpRecipient.Items.FindByValue(message.FromUserGUID.ToString().Replace("{", "").Replace("}","")).Selected = true;
                    
                    drpRecipient.Enabled = false;

                }
                
                
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if(drpRecipient.SelectedValue.Length >0 && Header.Text.Length > 0 && Body.Text.Length > 0 && SessionProps.UserGuid != Guid.Empty)
            {
                using (var db = Global.GetConnection())
                {
                    var message = new Ext_Message();
                    message.FromUserGUID = SessionProps.UserGuid;
                    message.Header = Header.Text;
                    message.Body = Body.Text;
                    message.SentDate = DateTime.Now;

                    var recipient = new Ext_MessageRecipient();
                    recipient.MessageGUID = message.GUID;
                    recipient.RecipientUserGUID = new Guid(drpRecipient.SelectedValue);

                    db.Ext_Message.InsertOnSubmit(message);
                    db.Ext_MessageRecipient.InsertOnSubmit(recipient);

                    db.SubmitChanges();

                    pnlEditControls.Visible = false;
                    pnlSuccessControls.Visible = true;
                    lblSuccessMessage.Text = "Meddelandet är skickat";
                }    
            }
            else
            {
                lblMessage.Text = "Kunde inte skicka meddelandet, ämne, text och mottagare måste fyllas i.";
            }
            
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
