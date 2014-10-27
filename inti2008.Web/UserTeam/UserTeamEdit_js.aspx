<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserTeamEdit_js.aspx.cs" Inherits="inti2008.Web.UserTeam.UserTeamEdit_js" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript" ></script>
    <script src="../Scripts/knockout-2.2.1.js" type="text/javascript" ></script>
    <script src="../Scripts/UserTeam.js" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div id="userTeam">
        <h1 data-bind="text: Name"></h1>
        <span data-bind="text: Description"></span>

        <span>Todo: skapa lag, ladda upp bild, administrera byten - SPA-style</span>

    </div>
</asp:Content>
