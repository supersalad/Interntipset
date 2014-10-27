<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MyTournamentStanding.aspx.cs" Inherits="inti2008.Web.MyTournamentStanding" %>

<%@ Register Src="~/TournamentStanding.ascx" TagPrefix="uc1" TagName="TournamentStanding" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title><%=GetTournamentName() %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <uc1:TournamentStanding runat="server" ID="TournamentStanding" />
</asp:Content>
