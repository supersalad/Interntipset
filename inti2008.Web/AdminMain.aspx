<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminMain.aspx.cs" Inherits="inti2008.Web.AdminMain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Administrera</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>So you think you can admin?</h4>
    <p>
        Härifrån når du de admin-funktioner som du har rätt att pilla med.
    </p>
    <div class="row">
        <div class="col-md-6">
            <div class="adminButtons">
                <asp:Button ID="btnUpdateMatches" runat="server" Visible="false" CssClass="btn"
                    Text="Uppdatera matcher" ToolTip="Uppdatera matcher"
                    OnClick="btnUpdateMatches_Click" />
                <br />
                <asp:LinkButton ID="lnkUpdateMatchesInstructions" runat="server" Visible="false"
                    Text="Instruktioner" OnClick="lnkUpdateMatchesInstructions_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="BtnNews" runat="server" Visible="false" CssClass="btn"
                    Text="Nyheter" ToolTip="Lägg till, editera och ta bort nyheter"
                    OnClick="BtnNews_Click" />
                <br />
                <asp:LinkButton ID="lnkNewsInstructions" runat="server" Visible="false"
                    Text="Instruktioner" OnClick="lnkNewsInstructions_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnTournaments" runat="server" Visible="false" CssClass="btn"
                    Text="Tävlingar"
                    ToolTip="Lägg till, editera och ta bort tävlingar (ny säsong eller EM/VM)"
                    OnClick="btnTournaments_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnRules" runat="server" Visible="false" CssClass="btn"
                    Text="Regler"
                    ToolTip="Editera regler"
                    OnClick="btnRules_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnUsers" runat="server" Visible="false" CssClass="btn"
                    Text="Användare"
                    ToolTip="Editera användare"
                    OnClick="btnUsers_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnPlayers" runat="server" Visible="false" CssClass="btn"
                    Text="Spelare"
                    ToolTip="Lägg till och editera spelare" OnClick="btnPlayers_Click" />
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnApproveTeams" runat="server" Visible="false" CssClass="btn"
                    Text="Godkänn lag" ToolTip="Aktivera och markera lag som betalda"
                    OnClick="btnApproveTeams_Click" />
            </div>
            <div>
                <asp:Button runat="server" ID="btnSignInToTwitter" Visible="False" CssClass="btn" Text="Logga in på twitter" OnClick="btnSignInToTwitter_Click"/>
            </div>
            <div>
                <asp:Button runat="server" ID="btnSignOutFromTwitter" Visible="False" CssClass="btn" Text="Logga ut från twitter" OnClick="btnSignOutFromTwitter_Click"/>
            </div>
            <div class="adminButtons">
                <asp:Button ID="btnProfiling" runat="server" Visible="false" CssClass="btn"
                    Text="Slå på MiniProfiler" ToolTip="Ger dig timing för sidorna i en timma"
                    OnClick="btnProfiling_Click" />
            </div>
        </div>
        <div class="col-md-6">
            <h5>Mesta uppdaterarna</h5>
            <asp:Label ID="TopUpdatersPanel" runat="server"></asp:Label>
        </div>
    </div>
</asp:Content>
