<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="inti2008.Web._Default" %>

<%@ Register Src="TournamentStanding.ascx" TagName="TournamentStanding" TagPrefix="inti" %>
<%@ Register Src="ForumEntries.ascx" TagName="ForumEntries" TagPrefix="inti" %>
<%@ Register Src="PrivateTournaments.ascx" TagName="PrivateTournaments" TagPrefix="inti" %>
<%@ Register Src="Rss.ascx" TagName="Rss" TagPrefix="inti" %>
<asp:Content ContentPlaceHolderID="head" runat="server" >
    <title>Interntipset</title>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <div class="row">
                <div class="col-md-12">
                    <inti:TournamentStanding ID="topTenStanding" runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <inti:TournamentStanding ID="formTable" runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <inti:PrivateTournaments ID="privateTournaments" runat="server" />
                </div>
            </div>
        </div>
        <div class="col-sm-6 col-md-4">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="lblNews" runat="server"></asp:Label>
                    <br />
                    <a href="/NewsList/0">Visa alla nyheter</a>
                </div>
            </div>

        </div>
        <div class="col-sm-6 col-md-4">
            <div class="row">
                <div class="col-md-12">
                    <inti:ForumEntries ID="forumEntries" runat="server" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <div class="fb-like-box" data-href="https://www.facebook.com/interntipset" data-width="292" data-show-faces="true" data-stream="false" data-show-border="false" data-header="false"></div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <a class="twitter-timeline" href="https://twitter.com/interntipset" data-widget-id="346708019012304896">Tweets av @interntipset</a>
<script>!function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https'; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = p + "://platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); } }(document, "script", "twitter-wjs");</script>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
