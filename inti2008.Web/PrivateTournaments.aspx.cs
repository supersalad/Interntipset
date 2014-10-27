using System;

namespace inti2008.Web
{
    public partial class PrivateTournaments1 : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                privateTournaments.LoadPrivateTournaments();
            }
        }
    }
}
