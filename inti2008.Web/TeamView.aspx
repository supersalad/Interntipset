<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="TeamView.aspx.cs" Inherits="inti2008.Web.TeamView" %>

<%@ Import Namespace="System.Windows.Forms" %>

<%@ Register Src="DaySelector.ascx" TagName="DaySelector" TagPrefix="inti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <div class="row">
        <div class="col-sm-6">
            <h4>
                <asp:Label ID="lblHeader" runat="server"></asp:Label></h4>

            <asp:Repeater runat="server" ID="rptPlayers">
                <HeaderTemplate>
                    <div class="list-group">
                </HeaderTemplate>
                <ItemTemplate>
                    <%# (bool)Eval("FirstInGroup") ? (@"<div class=""list-group-item list-group-item-success"">" + Eval("Position") + @"</div>") : string.Empty %>

                    <a href="/Player/<%#Eval("GUID") %>" class="list-group-item <%#Eval("CustomClass") %>">
                        <span class="badge"><%# Eval("Points") %></span>
                        <p class="list-group-item-heading"><%# Eval("Name") %></p>
                        <p class="list-group-item-text"><%# Eval("Club") %></p>
                        <p class="list-group-item-text"><%# ((DateTime?)Eval("TradeOut")).HasValue ? "Utbytt: " + Eval("TradeOut", "{0:d}" ) : string.Empty  %></p>
                    </a>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>


            <p>
                <asp:Label ID="lblTeamTransfers" runat="server"></asp:Label></p>

            <asp:Button ID="btnToggleFavorite" runat="server" Text="Markera laget"
                CssClass="btn btn-primary" OnClick="btnToggleFavorite_Click" />

        </div>

        <div class="col-sm-6">
            <asp:Panel ID="pnlTeamImage" runat="server" Visible="false">
                <div>
                    <asp:Image ID="imgTeamImage" runat="server" />
                </div>
            </asp:Panel>
            <p>
                <asp:Label ID="lblDescription" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="lblTeamInfo" runat="server"></asp:Label>
            </p>
            
            <p>
                <a target="_blank" href="https://facebook.com/sharer.php?u=<%=Request.Url%>">Dela på Facebook</a>
            </p>
            <p>
                <a target="_blank" href="https://twitter.com/intent/tweet?url=<%=Request.Url%>&text=<%=lblHeader.Text%>&via=interntipset">Dela på Twitter</a>
            </p>

            <div>
                <inti:DaySelector ID="daySelector" runat="server"
                    OnSelectedDayChanged="daySelector_SelectedDayChanged" />
            </div>
        </div>
    </div>




</asp:Content>
