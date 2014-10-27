<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="inti2008.Web.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Felsida</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Oj, det uppstod ett fel.</h4>
<p>
Felbeskrivningen nedan har mailats med automatik till systemadministratören.
</p>
<asp:Label ID="lblErrorDescription" runat="server"></asp:Label><br />
<p>
Om du inte kommer vidare med laganmälan eller liknande, maila om det via länken nedan.
</p>
<asp:Label ID="lblMailLink" runat="server"></asp:Label><br />
</asp:Content>
