<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="PlayerView.aspx.cs" Inherits="inti2008.Web.PlayerView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title><%=lblPlayerName.Text %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>
        <asp:Label ID="lblPlayerName" runat="server"></asp:Label></h4>
    <div class="row">
        <div class="col-md-6">
            <p><asp:Label ID="lblPos" runat="server"></asp:Label> i <asp:Label ID="lblClub" runat="server"></asp:Label>, kostar <asp:Label ID="lblPrice" runat="server"></asp:Label></p>
            
            <asp:Label ID="lblPoints" runat="server"></asp:Label>
        </div>

        <div class="col-md-6">
            <asp:Repeater runat="server" ID="rptIntiTeams">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <a href="/Team/<%#Eval("TeamGUID") %>" class="list-group-item <%#Eval("CustomClass") %>">
                        <p class="list-group-item-heading"><%# Eval("TeamName") %></p>
                        <p class="list-group-item-text"><%# Eval("TeamManager") %></p>
                        </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>


</asp:Content>
