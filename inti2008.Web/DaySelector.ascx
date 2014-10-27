<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DaySelector.ascx.cs" Inherits="inti2008.Web.DaySelector" %>

    <h5>Visa per omgång</h5>
    <asp:DropDownList ID="drpTournamentDay" runat="server" CssClass="input" 
        AutoPostBack="True" 
        onselectedindexchanged="drpTournamentDay_SelectedIndexChanged"></asp:DropDownList>
    <p><asp:Label ID="lblMatchUpdates" runat="server"></asp:Label></p>
