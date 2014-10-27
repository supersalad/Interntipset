<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserTeamUploadImage.aspx.cs" Inherits="inti2008.Web.UserTeamUploadImage" %>
<%@ Register src="UploadImage.ascx" tagname="UploadImage" tagprefix="inti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Ladda upp bild</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <inti:UploadImage ID="uploadImage" runat="server"
    OnImageUploaded="uploadImage_ImageUploaded"
    OnCancel="uploadImage_Cancel" />
    
    <asp:label ID="lblMessage" runat="server" />
</asp:Content>
