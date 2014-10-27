using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminEditUser : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_USERS");

            if (!IsPostBack)
                LoadUsers();
        }

        private Guid Id
        {
            get
            {
                if (ViewState["Guid"] is Guid)
                    return (Guid)ViewState["Guid"];

                return Guid.Empty;
            }
            set
            {
                ViewState["Guid"] = value;
            }
        }

        private void LoadUsers()
        {
            divMessage.Visible = false;

            grdUsers.DataKeyNames = new string[]{"GUID"};
            grdUsers.DataSource = new UserManagement(Global.ConnectionString, SessionProps).GetAllUsers();
            grdUsers.DataBind();
        }

        private void LoadUser(Guid userGuid)
        {
            divMessage.Visible = false;

            var userManagement = new UserManagement(Global.ConnectionString, SessionProps);
            var user = userManagement.GetUserByGuid(userGuid);

            UserName.Text = user.UserName;
            FirstName.Text = user.FirstName;
            LastName.Text = user.LastName;

            //load permissions
            grdPermissions.DataKeyNames = new string[]{"GUID"};
            grdPermissions.DataSource = userManagement.GetPermissionsToEdit(userGuid);
            grdPermissions.DataBind();

            Id = userGuid;

            userChangeLog.LoadChangeLog(userGuid);

            pnlEditUser.Visible = true;
        }

        protected void grdUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            // Get the selected index and the command name

            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;

            switch (_commandName)
            {
                case ("Click"):
                    _gridView.SelectedIndex = _selectedIndex;
                    var userGuid = new Guid(_gridView.SelectedValue.ToString());
                    LoadUser(userGuid);
                    break;
            }
        }

        

        protected void grdUsers_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected override void Render(HtmlTextWriter writer)
        {
            //register rowcommands for event validation
            foreach (GridViewRow r in grdUsers.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    Page.ClientScript.RegisterForEventValidation
                            (r.UniqueID + "$ctl00");
                }
            }

            //todo: register page browsing for event validation


            base.Render(writer);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (Id == Guid.Empty) return;

            divMessage.Visible = false;

            var permissionGuids = new List<Guid>();
            foreach (GridViewRow row in grdPermissions.Rows)
            {
                //checked?
                var cb = (CheckBox)row.FindControl("chkPermission");
                if (cb.Checked)
                {
                    grdPermissions.SelectedIndex = row.RowIndex;

                    permissionGuids.Add(new Guid(grdPermissions.SelectedValue.ToString()));
                }
            }
            new UserManagement(Global.ConnectionString, SessionProps).SaveUser(Id, UserName.Text, FirstName.Text, LastName.Text, permissionGuids);
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (Id == Guid.Empty) return;

            var newPassword = new UserManagement(Global.ConnectionString, SessionProps).ForgotPassword(UserName.Text);

            divMessage.Visible = true;

            divMessage.InnerText = "Nytt lösenord: " + newPassword;


        }
    }
}
