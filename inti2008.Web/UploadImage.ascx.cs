using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;
using Image=System.Web.UI.WebControls.Image;

namespace inti2008.Web
{
    public partial class UploadImage : System.Web.UI.UserControl
    {
        private const int MaximumHeight = 250;
        private const int MaximumWidth = 250;

        public delegate void OnImageUploaded();
        public delegate void OnCancel();

        public event OnImageUploaded ImageUploaded;
        public event OnCancel Cancel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnUndo.Visible = false;
                btnChangeImage.Visible = false;
            }
        }


        #region public methods

        public void UploadUserImage(string currentImage)
        {
            CurrentImageName = currentImage;
            LoadCurrentImage(currentImage);

            UploadUserImage();
        }

        public void UploadUserImage()
        {

        }

        #endregion

        #region properties

        private string NewImageName
        {
            get
            {
                if (ViewState["NewImageName"] == null)
                    ViewState["NewImageName"] = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");

                return ViewState["NewImageName"].ToString();
            }
        }

        public string ImageNameToStore
        {
            get
            {
                return Path.GetFileName(imgPreviewImage.ImageUrl);
            }
        }


        private string CurrentImageName
        {
            get
            {
                if (ViewState["CurrentImageName"] != null)
                    return ViewState["CurrentImageName"].ToString();

                return "";
            }
            set
            {
                ViewState["CurrentImageName"] = value;
            }
        }

        #endregion


        private void LoadPreviewImage(string image)
        {
            LoadImage(image, pnlPreviewImage, "Uppladdad bild");
        }

        private void LoadCurrentImage(string image)
        {
            LoadImage(image, pnlCurrentImage, "Nuvarande bild");
        }

        private void LoadImage(string image, Panel imagePanel, string label)
        {
            foreach (var control in imagePanel.Controls)
            {
                if (control is Image)
                {
                    (control as Image).ImageUrl = Page.ResolveUrl("~/img/user/" + image);
                }

                if (control is Label)
                {
                    (control as Label).Text = label;
                }
            }

            imagePanel.Visible = true;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            var stream = uplImage.FileContent;
            lblMessage.Text = "";
            lblMessage.ToolTip = "";

            //check file size
            if (stream.Length > (1024 * 512))
            {
                lblMessage.Text = "Max storlek för bilder att ladda upp är 512 MB";
                return;
            }

            try
            {
                var serverSideImage = new System.Drawing.Bitmap(stream);

                var rescaledImage = RescaleImage(serverSideImage, MaximumWidth, MaximumHeight);

                var fileName = NewImageName + uplImage.FileName.Substring(uplImage.FileName.LastIndexOf("."));

                rescaledImage.Save(Server.MapPath("~/img/user/") + fileName);

                LoadPreviewImage(fileName);

                uplImage.Visible = false;
                btnUpload.Visible = false;

                btnUndo.Visible = true;
                btnChangeImage.Visible = true;
            }
            catch (Exception exception)
            {
                lblMessage.Text = "Kan inte läsa bilden, testa med ett annat format.";
                lblMessage.ToolTip = exception.Message;
                WebControlManager.SendAndLogErrorMessage(exception, Parameters.Instance.MailSender, Parameters.Instance.SupportMail);
            }


        }

        private Bitmap RescaleImage(Bitmap image, int maxWidth, int maxHeight)
        {
            if (image.Size.Width > maxWidth || image.Size.Height > maxHeight)
            {
                int newWidth;
                int newHeight;

                if (((float)image.Size.Width / (float)image.Size.Height) < ((float)maxWidth / (float)maxHeight))
                {
                    newHeight = maxHeight;
                    newWidth = (int)(maxWidth * ((float)image.Size.Width / (float)image.Size.Height));
                }
                else
                {
                    newWidth = maxWidth;
                    newHeight = (int)(maxHeight * ((float)image.Size.Height / (float)image.Size.Width));
                }
                return new Bitmap(image, new Size(newWidth, newHeight));
            }

            return image;
        }

        protected void btnUndo_Click(object sender, EventArgs e)
        {
            //ta bort den nya bilden
            RemoveImage(NewImageName);
            //clear the viewstate
            ViewState["NewImageName"] = null;

            //raise cance event
            if (Cancel != null)
                Cancel();
        }

        private void RemoveImage(string imageName)
        {
            var directory = new DirectoryInfo(Server.MapPath("~/img/user/"));
            var deleted = true;

            while (deleted)
            {
                deleted = false;
                var files = directory.GetFiles(Path.GetFileNameWithoutExtension(imageName) + ".*");
                foreach (var file in files)
                {
                    file.Delete();
                    deleted = true;
                    break;
                }
            }
        }

        protected void btnChangeImage_Click(object sender, EventArgs e)
        {
            if(CurrentImageName != "")
                RemoveImage(CurrentImageName);

            if (ImageUploaded != null)
                ImageUploaded();
        }
    }
}