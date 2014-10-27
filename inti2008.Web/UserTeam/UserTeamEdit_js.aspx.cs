using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace inti2008.Web.UserTeam
{
    public partial class UserTeamEdit_js : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScriptBlock("loadTeam", String.Format("<script type=\"text/javascript\">LoadUserTeam('{0}');</script>", TeamId.ToString()));
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
            set
            {
                ViewState["teamGUID"] = value;
            }
        }
    }
}