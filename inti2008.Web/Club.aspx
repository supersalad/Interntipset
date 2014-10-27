<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="Club.aspx.cs" Inherits="inti2008.Web.Club" %>

<%@ Register Src="~/TeamBreakDown.ascx" TagPrefix="uc1" TagName="TeamBreakDown" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="twitter:card" content="summary">
    <meta name="twitter:site" content="@interntipset">
    <meta name="twitter:title" content="<%=lblHeader.Text %>">
    
    <title><%=lblHeader.Text %></title>

    <style>
        .row > .player-MGR {
            clear: both;
            float: left;
        }

            .row > .player-MGR ~ .player-MGR {
                clear: none;
                float: left;
            }

        .row > .player-GK {
            clear: both;
            float: left;
        }

            .row > .player-GK ~ .player-GK {
                clear: none;
                float: left;
            }

        .row > .player-D {
            clear: both;
            float: left;
        }

            .row > .player-D ~ .player-D {
                clear: none;
                float: left;
            }

        .row > .player-M {
            clear: both;
            float: left;
        }

            .row > .player-M ~ .player-M {
                clear: none;
                float: left;
            }

        .row > .player-S {
            clear: both;
            float: left;
        }

            .row > .player-S ~ .player-S {
                clear: none;
                float: left;
            }

        .row > .player-SUB {
            clear: both;
            float: left;
        }

            .row > .player-SUB ~ .player-SUB {
                clear: none;
                float: left;
            }

        .player-wrapper {
            position: relative;
        }

        .player-background {
            background-color: #008000;
            font-family: helvetica;
            font-weight: bold;
            font-style: italic;
            font-size: 18pt;
            position: absolute;
            top: 0;
            left: 0;
            bottom: 0;
            right: 0;
            z-index: -1;
            overflow: hidden;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h3><asp:Label runat="server" ID="lblHeader"></asp:Label></h3>
    <div class="row">
        <div class="col-md-4">
            <h4>Spelare</h4>
            <asp:Repeater runat="server" ID="rptPlayers">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <%# (bool)Eval("FirstInGroup") ? (@"<div class=""list-group-item list-group-item-success"">" + Eval("Position") + @"</div>") : string.Empty %>

                    <a href="/Player/<%#Eval("GUID") %>" class="list-group-item <%#Eval("CustomClass") %>">
                        <span class="badge"><%# Eval("Points") %></span>
                        <p class="list-group-item-heading"><%# Eval("Name") %></p>
                    </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>    
        <div class="col-md-4">
            <h4>Matcher</h4>
            <asp:Repeater runat="server" ID="rptMatches">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <a href="/Match/<%#Eval("GUID") %>" class="list-group-item">
                        <p class="list-group-item-heading"><%# Eval("MatchLabel") %></p>
                    </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="col-md-4">
            <uc1:TeamBreakDown runat="server" ID="TeamBreakDown" />
        </div>
    </div>
</asp:Content>
