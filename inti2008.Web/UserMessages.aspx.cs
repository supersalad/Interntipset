using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace inti2008.Web
{
    public partial class UserMessages : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadHeader();

            LoadMessages();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdNewMessages.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            base.Render(writer);
        }

        private Guid Id
        {
            get
            {
                var value = this.GetRedirectParameter("GUID", false);
                if (value != null)
                    return new Guid(value.ToString());


                return SessionProps.UserGuid;
            }
        }

        private void LoadHeader()
        {
            using (var db = Global.GetConnection())
            {
                var users = from u in db.Sys_User
                            where u.GUID == SessionProps.UserGuid
                            select u;

                var user = users.ToList()[0];

                Header.Text = String.Format("{0} {1}", user.FirstName, user.LastName);
            }
        }

        private void LoadMessages()
        {
            if (Id == SessionProps.UserGuid)
            {
                using (var db = Global.GetConnection())
                {
                    var messages = from mrec in db.Ext_MessageRecipient
                                   where mrec.Sys_User.GUID == SessionProps.UserGuid
                                   select new
                                   {
                                       mrec.Ext_Message.GUID,
                                       mrec.Ext_Message.Header,
                                       FromName = String.Format("{0} {1}", mrec.Ext_Message.Sys_User.FirstName, mrec.Ext_Message.Sys_User.LastName).Trim(),
                                       mrec.Ext_Message.SentDate,
                                       mrec.ReadOn
                                   };


                    grdNewMessages.DataKeyNames = new string[] { "GUID" };
                    grdNewMessages.DataSource = messages.OrderByDescending(m => m.SentDate).ToList();
                    grdNewMessages.DataBind();
                }
            }
        }

        protected void grdNewMessages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                var _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                // Add this javascript to the onclick Attribute of the row

                e.Row.Attributes["onclick"] = _jsSingle;
            }
        }

        protected void grdNewMessages_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;

                    WebControlManager.RedirectWithQueryString("MessageView.aspx", new string[] { "messageGUID" }, new string[] { _gridView.SelectedValue.ToString() });

                    break;
            }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            WebControlManager.RedirectWithQueryString("MessageCreate.aspx", new string[] { "msgSubject" }, new string[] { "" });
        }


        protected string GetMailReadStatusIcon(DateTime? readOn)
        {
            if (readOn.HasValue)
                return "<img src=\"img/email_open.png\">";

            return "<img src=\"img/email.png\">";
        }
    }
}