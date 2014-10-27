<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivateTournaments.ascx.cs" Inherits="inti2008.Web.PrivateTournaments" %>
<h4><asp:Label ID="lblHeader" runat="server" /></h4>
            <asp:Repeater runat="server" ID="rptPrivateTournaments">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <a href="MyTournamentStanding.aspx?pvtTournamentGUID=<%#Eval("GUID") %>" class="list-group-item">
                        <p class="list-group-item-heading"><%# Eval("Name") %></p>
                        <p class="list-group-item-text">Avgörs: <%# DataBinder.Eval(Container.DataItem,"OrderDate","{0:d}") %>, <%# Eval("NumberOfContestants") %> deltagare.</p>
                      </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
            <p>
                <asp:label ID="lblShowAllTournaments" runat="server">
                    <a href="PrivateTournaments.aspx">visa alla interna uppgörelser</a>
                </asp:label>
            </p>