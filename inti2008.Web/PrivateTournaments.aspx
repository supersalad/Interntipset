<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="PrivateTournaments.aspx.cs" Inherits="inti2008.Web.PrivateTournaments1" %>

<%@ Register src="PrivateTournaments.ascx" tagname="PrivateTournaments" tagprefix="inti" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Privata uppgörelser</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div class="formFloat">
        <div class="formPane">
            <inti:PrivateTournaments ID="privateTournaments" runat="server" />
        </div>
    </div>
</asp:Content>
