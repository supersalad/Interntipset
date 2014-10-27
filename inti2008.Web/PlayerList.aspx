<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="PlayerList.aspx.cs" Inherits="inti2008.Web.PlayerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Spelare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>Spelare</h4>
    <div class="row">
        <div class="col-md-4">
            <div class="row">
                <label for="<%=drpClubs.ClientID %>" class="col-md-4">Lag</label>
                <div class="col-md-8">
                    <asp:DropDownList ID="drpClubs" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="row top-buffer">
                <label for="<%=drpPosition.ClientID %>" class="col-md-4">Position</label>
                <div class="col-md-8">
                    <asp:DropDownList ID="drpPosition" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="row top-buffer">
                <div class="col-md-offset-4 col-md-8">
                    <asp:Button ID="btnShowPlayers" runat="server" Text="Visa"
                        ToolTip="Visa spelare i vald klubb och position" CssClass="btn btn-primary"
                        OnClick="btnShowPlayers_Click" />
                </div>
            </div>
        </div>
        <div class="col-md-8">
             <asp:Repeater runat="server" ID="rptPlayers">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <a href="/Player/<%#Eval("GUID") %>" class="list-group-item <%#Eval("CustomClass") %>">
                        <span class="badge"><%# Eval("Points") %></span>
                        <p class="list-group-item-heading"><%# Eval("Name") %></p>
                        <p class="list-group-item-text"><%# Eval("Club") %>, <%# Eval("Position") %>, <%# Eval("Price") %></p>
                        </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
