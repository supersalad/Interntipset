<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TournamentStanding.ascx.cs" Inherits="inti2008.Web.TournamentStanding" %>
<h4><asp:Label ID="lblHeader" runat="server" /></h4>
<div><asp:Label runat="server" ID="lblDescription"></asp:Label></div>
    <asp:Repeater runat="server" ID="rptStanding">
        <HeaderTemplate>
            <div class="list-group">
        </HeaderTemplate>
        <ItemTemplate>
            <a href="/Team/<%#Eval("GUID") %>/<%# GetTourDayParam() %>" class="list-group-item <%#Eval("CustomClass") %>">
                <span class="badge"><%# Eval("Points") %></span>
                <p class="list-group-item-heading"><%# GetPosition() %>&nbsp;<%# Eval("TeamName") %></p>
                <p class="list-group-item-text"><%# Eval("Manager") %></p>
                </a>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

