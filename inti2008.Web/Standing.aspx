<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="Standing.aspx.cs" Inherits="inti2008.Web.Standing" %>

<%@ Register Src="DaySelector.ascx" TagName="DaySelector" TagPrefix="inti" %>
<%@ Register Src="~/TournamentStanding.ascx" TagPrefix="inti" TagName="TournamentStanding" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Ställning</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">

    <div class="row">
        <div class="col-md-6">
                <inti:TournamentStanding runat="server" ID="TournamentStanding" />
        </div>
        <div class="col-md-6">
            <inti:DaySelector ID="daySelector" runat="server"
                OnSelectedDayChanged="daySelector_SelectedDayChanged" />
        </div>

    </div>

</asp:Content>
