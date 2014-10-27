<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MessageView.aspx.cs" Inherits="inti2008.Web.MessageView"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Meddelande</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h1><asp:Label ID="lblHeader" runat="server"></asp:Label></h1>
<div class="formLoosePane">
<p><b><asp:Label ID="lblInformation" runat="server"></asp:Label></b></p>
<p><asp:Label ID="lblBody" runat="server"></asp:Label></p>
</div>
<div class="formButtons">
<asp:Button ID="btnDelete" runat="server" Text="Ta bort" 
        ToolTip="Ta bort detta meddelandet" CssClass="actionButton" 
        onclick="btnDelete_Click" />
<asp:Button ID="btnReply" runat="server"  Text="Svara" 
        ToolTip="Skicka ett svar på detta meddelandet" CssClass="actionButton" 
        onclick="btnReply_Click"/>
</div>
</asp:Content>
