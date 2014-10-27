<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TournamentSelector.ascx.cs" Inherits="inti2008.Web.TournamentSelector" %>

<asp:DropDownList ID="drpTournament" runat="server" CssClass="input" 
        AutoPostBack="True" 
        onselectedindexchanged="drpTournament_SelectedIndexChanged"></asp:DropDownList>
