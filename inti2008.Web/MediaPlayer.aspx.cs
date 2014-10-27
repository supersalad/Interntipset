

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace inti2008.Web
{
    public partial class MediaPlayer : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblHeader.Text = "Mediaspelare";
            lblDescription.Text = "Ingen film vald";
            _mediaPlayer.Visible = false;

            if (!IsPostBack)
            {
                LoadMedia();
            }
        }

        private void LoadMedia()
        {
            var mediaToShow = this.GetRedirectParameter("mediaToShow", true);

            if (mediaToShow != null)
            {
                switch (mediaToShow.ToString())
                {
                    case "MatchUpdate":
                        lblHeader.Text = "Uppdatera matcher";
                        lblDescription.Text = "Följ instruktionerna i filmen för att uppdatera matcher.";
                        _mediaPlayer.MediaSource = "media/Matchuppdatering.wmv";
                        _mediaPlayer.Visible = true;
                        break;
                    case "NewsUpdate":
                        lblHeader.Text = "Skriva nyheter";
                        lblDescription.Text = "Följ instruktionerna i filmen för att skriva nyheter.";
                        _mediaPlayer.MediaSource = "media/Nyheter.wmv";
                        _mediaPlayer.Visible = true;
                        break;
                }
            }
        }
    }
}
