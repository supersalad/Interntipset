<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MatchView.aspx.cs" Inherits="inti2008.Web.MatchView"%>

<%@ Register Src="~/TournamentStanding.ascx" TagPrefix="uc1" TagName="TournamentStanding" %>
<%@ Register Src="~/TeamBreakDown.ascx" TagPrefix="uc1" TagName="TeamBreakDown" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <meta name="twitter:card" content="summary">
    <meta name="twitter:site" content="@interntipset">
    <meta name="twitter:title" content="<%=this.MatchHeader %>">
    <meta name="twitter:description" content="<%=this.MatchSummary %>">
    <title><%=this.MatchHeader %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4><asp:Label runat="server" ID="lblHeader"></asp:Label>&nbsp;(<asp:Label runat="server" ID="lblResult"></asp:Label>)</h4>
    <div class="row">
        <!-- Resultat -->
        <div class="col-sm-6">
            <p><asp:Label runat="server" ID="lblMatchDate"></asp:Label></p>
            <p><asp:Label runat="server" ID="lblUpdater"></asp:Label></p>
            <h5>Utdelade poäng</h5>
            <p><asp:Label runat="server" ID="lblPointEvents"></asp:Label></p>
            <p><asp:Label runat="server" ID="lblMatchFactsTest"></asp:Label></p>
            <p>
                <a target="_blank" href="https://facebook.com/sharer.php?u=<%=Request.Url%>">Dela på Facebook</a>
            </p>
            <p>
                <a target="_blank" href="https://twitter.com/intent/tweet?url=<%=Request.Url%>&text=<%=lblHeader.Text%>&via=interntipset">Dela på Twitter</a>
            </p>
        </div>
        <div class="col-sm-6">
            
            <uc1:TournamentStanding runat="server" ID="ucStanding" />
            
            <uc1:TeamBreakDown runat="server" id="ucTeamBreakDown" />
        </div>
    </div>
    
    
    
    
</asp:Content>
