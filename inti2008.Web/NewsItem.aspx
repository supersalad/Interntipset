<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="NewsItem.aspx.cs" Inherits="inti2008.Web.NewsItem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title><%=Header.Text %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div class="row">
        <div class="col-md-10 col-md-offset-1">
            <h4><asp:Label runat="server" ID="Header"></asp:Label></h4>
            <p><asp:Label runat="server" ID="Body"></asp:Label></p>
        </div>
    </div>
</asp:Content>
