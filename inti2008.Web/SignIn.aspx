<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="inti2008.Web.SignIn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Logga in</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:Login ID="login" runat="server" onauthenticate="login_Authenticate"
    LoginButtonStyle-CssClass="btn btn-primary" ></asp:Login>
</asp:Content>
