<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadImage.ascx.cs" Inherits="inti2008.Web.UploadImage" %>

<div>
    <asp:Panel ID="pnlCurrentImage" runat="server" Visible="false">
        <div>
            <asp:Image ID="imgCurrentImage" runat="server" />
        </div>
        <div>
            <asp:Label ID="lblCurrentImage" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlPreviewImage" runat="server" Visible="false">
        <div>
            <asp:Image ID="imgPreviewImage" runat="server" />
        </div>
        <div>
            <asp:Label ID="lblPreviewImage" runat="server" />
        </div>
    </asp:Panel>

    <div>
        <asp:FileUpload ID="uplImage" runat="server" />
    </div>
    <div>
        <asp:Label ID="lblMessage" runat="server" />
    </div>
    <div>
        <asp:Button ID="btnUpload" runat="server" Text="Ladda upp..." 
            CssClass="actionButton" onclick="btnUpload_Click" />
    </div>
    <div>
        <asp:Button ID="btnUndo" runat="server" Text="Avbryt" 
            ToolTip="Använd inte den nya bilden" onclick="btnUndo_Click"
            CssClass="actionButton" />
    </div>
    <div>
        <asp:Button ID="btnChangeImage" runat="server" Text="Spara" 
            ToolTip="Använd den uppladdade bilden" onclick="btnChangeImage_Click"
            CssClass="actionButton" />
    </div>
</div>