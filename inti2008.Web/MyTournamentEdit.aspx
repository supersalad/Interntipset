<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MyTournamentEdit.aspx.cs" Inherits="inti2008.Web.MyTournamentEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Privat utmaning</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Privat utmaning</h4>
<p>Fyll i uppgifterna nedan för att skapa en ny privat utmaning.<br />
När dina kompisar accepterat utmaningen kan ni luta er tillbaka och se vem som vinner...</p>
<div class="formLoosePane">
    <table>
        <tr>
            <td class="description">
                Namn:
            </td>
            <td class="input">
                <asp:TextBox ID="Name" runat="server" ToolTip="Namn på din utmaning"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description" colspan="2">
                Beskrivning:
            </td>
        </tr>
        <tr>
            <td class="wideDescription" colspan="2">
                <asp:TextBox ID="Description" runat="server" CssClass="textInput" TextMode="MultiLine" MaxLength="1000" ToolTip="Beskriv din utmaning, om det står något utöver äran på spel kan du ta upp det här."></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Visa utmaningen för alla:
            </td>
            <td class="input">
                <asp:CheckBox ID="IsVisibleForAll" runat="server" ToolTip="Visa hur det går i denna utmaningen även för dem som inte deltar." />
            </td>
        </tr>
    </table>
</div>
<asp:ScriptManager ID="scriptManager1" runat="server">
</asp:ScriptManager>
<asp:UpdatePanel ID="updTournamentLimitation" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<div class="formLoosePane">
    <table>
        <tr>
            <td class="description">
                Start/slut:
            </td>
            <td class="input">
                <asp:RadioButtonList ID="rblDateLimitation" runat="server" AutoPostBack="true"
                    onselectedindexchanged="rblDateLimitation_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Text="Datum" Value="datum"></asp:ListItem>
                    <asp:ListItem Selected="False" Text="Omgångar" Value="omg"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    
    <asp:Panel ID="pnlDateLimitation" runat="server">
        <table>
            <tr>
                <td class="description">
                    Startdatum:
                </td>
                <td class="input">
                    <asp:TextBox ID="StartDate" runat="server" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="description">
                    Slutdatum:
                </td>
                <td class="input">
                    <asp:TextBox ID="EndDate" runat="server" ></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlDayLimitation" runat="server" Visible="false">
    <table>
            <tr>
                <td class="description">
                    Startomgång:
                </td>
                <td class="input">
                    <asp:DropDownList ID="drpStartDay" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="description">
                    Slutomgång:
                </td>
                <td class="input">
                    <asp:DropDownList ID="drpEndDay" runat="server"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<asp:Panel ID="pnlAddParticipants" runat="server">
<asp:UpdatePanel ID="updAddParticipants" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<div class="formLoosePane">
    <h2>Utmana deltagarlag</h2>
    <p>(betalda lag visas i listan nedan)</p>
    <asp:DropDownList ID="drpTeamsToChallenge" runat="server"></asp:DropDownList><br />
    <asp:Button ID="btnChallenge" runat="server" Text="Utmana" 
        ToolTip="Utmana det valda laget" CssClass="actionButton" 
        onclick="btnChallenge_Click" /><br />
    <asp:Label ID="lblChallengeMessage" runat="server"></asp:Label>
    <asp:GridView id="grdChallengedTeams" runat="server" CssClass="table table-bordered" AutoGenerateColumns="false" >
        <Columns>
            <asp:BoundField DataField="TeamName" HeaderText="Lag" />
            <asp:TemplateField HeaderText="Har accepterat">
                    <ItemTemplate>
                        <%# GetStatusImage(bool.Parse(Eval("IsAccepted").ToString()), "Utmaningen är accepterad", "Har inte tagit ställning till utmaningen än") %>
                    </ItemTemplate>
                </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>

<div class="formPaneNR">
<asp:Label ID="lblMessage" runat="server"></asp:Label>
</div>
<div class="formButtons">
<asp:Button ID="btnCancel" runat="server" Text="Avbryt" ToolTip="Avbryt utan att spara ändringar" CssClass="actionButton" />
<asp:Button ID="btnSave" runat="server" Text="Spara" ToolTip="Spara utmaningen" 
        CssClass="actionButton" onclick="btnSave_Click" />
</div>
</asp:Content>
