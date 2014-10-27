<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminTournamentEdit.aspx.cs" Inherits="inti2008.Web.AdminTournamentEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera tävlingar</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Pilla på tävlingen</h4>
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Namn:
            </td>
            <td class="input">
                <asp:TextBox ID="TournamentName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Beskrivning:
            </td>
            <td class="input">
                <asp:TextBox ID="TournamentDescription" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Börja visa:
            </td>
            <td class="input">
                <asp:TextBox ID="PublicateDate" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Öppna för nya lag:
            </td>
            <td class="input">
                <asp:TextBox ID="StartRegistration" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Deadline för nya lag:
            </td>
            <td class="input">
                <asp:TextBox ID="EndRegistration" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Antal omgångar:
            </td>
            <td class="input">
                <asp:TextBox ID="NmbrOfDays" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Antal lag:
            </td>
            <td class="input">
                <asp:TextBox ID="NmbrOfClubs" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Antal byten:
            </td>
            <td class="input">
                <asp:TextBox ID="NmbrOfTransfers" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Budget:
            </td>
            <td class="input">
                <asp:TextBox ID="Budget" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Inkudera manager:
            </td>
            <td class="input">
                <asp:CheckBox ID="IncludeManager" runat="server" />
            </td>
        </tr>
    </table>
</div>
<div class="formButtons">
<asp:Button ID="btnCancel" runat="server" Text="Avbryt" CssClass="actionButton"
        ToolTip="Gå tillbaka utan att spara ändringar" onclick="btnCancel_Click" />
<asp:Button ID="btnResetValidFromOnTeams" runat="server" Text="Ändra datum på lagen" CssClass="actionButton"
OnClick="btnResetValidFromOnTeams_Click" />
<asp:Button ID="btnSave" runat="server" Text="Spara" ToolTip="Spara ändringar" CssClass="actionButton"
        onclick="btnSave_Click" />
</div>
<div class="formPane">
    <asp:Label ID="lblMatchEditMessage" runat="server"></asp:Label>
</div>
<div class="formButtons">
<asp:Button ID="btnGenerateMatches" runat="server" Text="Generera matcher" 
        ToolTip="Generera matcher - alla möter alla" CssClass="actionButton" 
        onclick="btnGenerateMatches_Click" />
 <asp:Button ID="btnEditMatches" runat="server" Text="Editera matcher"
        tooltip="Gå till sidan dä du kan editera tävlingens matcher" 
        CssClass="actionButton" onclick="btnEditMatches_Click" />
<asp:Button ID="btnBatchEditMatches" runat="server" Text="Editera många matcher"
        tooltip="Gå till sidan där du kan editera tävlingens matcher, en omgång i taget" 
        CssClass="actionButton" onclick="btnBatchEditMatches_Click" />
</div>
<asp:Panel id="pnlTransferPeriods" runat="server">
<div class="formPaneNR">
<h2>transfer-fönster</h2>
<asp:GridView ID="grdTransferPeriods" runat="server" AutoGenerateColumns="false" 
        AllowPaging="false"
        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" 
        onrowcommand="grdTransferPeriods_RowCommand" 
        onrowdatabound="grdTransferPeriods_RowDataBound" >
        <Columns>
            <asp:ButtonField Text="Click" CommandName="Click" visible="false" />
            <asp:BoundField DataField="GUID" visible="false" />
            <asp:BoundField DataField="Name" HeaderText="Namn" />
            <asp:BoundField DataField="Description" HeaderText="Beskrivning" />
            <asp:BoundField DataField="StartDate" HeaderText="Startdatum" />
            <asp:BoundField DataField="EndDate" HeaderText="Slutdatum" />
        </Columns>
    </asp:GridView>
</div>
<asp:Panel ID="pnlEditTransferWindow" runat="server">
    <div class="formPaneNR">
        <table>
            <tr>
                <td class="description">
                    Namn:
                </td>
                <td class="input">
                    <asp:TextBox ID="txtTransferWindowName" runat="server" MaxLength="50" />
                </td>
            </tr>
            <tr>
                <td class="description" colspan="2">
                    Beskrivning:
                </td>
            </tr>
            <tr>
                <td class="wideDescription" colspan="2">
                    <asp:TextBox ID="txtTransferWindowDescription" runat="server" MaxLength="255" CssClass="textInput" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="description">
                    Start-datum:
                </td>
                <td class="input">
                    <asp:TextBox ID="txtTransferWindowStartDate" runat="server" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="description">
                    Slut-datum:
                </td>
                <td class="input">
                    <asp:TextBox ID="txtTransferWindowEndDate" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btnSaveTransferWindow" runat="server" CssClass="actionButton"
            Text="Spara" ToolTip="Spara transferfönstret" 
            onclick="btnSaveTransferWindow_Click" />
    </div>
</asp:Panel>
<div class="formButtons">
<asp:Button id="btnAddTransferWindow" runat="server" Text="Lägg till" 
        ToolTip="Lägg till ett bytesfönster" CssClass="actionButton" 
        onclick="btnAddTransferWindow_Click" />
</div>
</asp:Panel>
<asp:Panel id="pnlClubs" runat="server">
<div class="formPaneNR">
<h2>lag i turneringen</h2>
<asp:GridView ID="grdClubs" runat="server" AutoGenerateColumns="false" 
        AllowPaging="false" onrowcommand="grdClubs_RowCommand" 
        onrowdatabound="grdClubs_RowDataBound"
        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" >
        <Columns>
            <asp:ButtonField Text="Click" CommandName="Click" visible="false" />
            <asp:BoundField DataField="GUID" visible="false" />
            <asp:BoundField DataField="ShortName" HeaderText="Förkortn." />
            <asp:BoundField DataField="Name" HeaderText="Namn" />
        </Columns>
    </asp:GridView>
</div>
<div class="formButtons">
<asp:Button id="btnAddNewClub" runat="server" Text="Lägg till" 
        ToolTip="Lägg till ett lag" CssClass="actionButton" 
        onclick="btnAddNewClub_Click" />
</div>
</asp:Panel>
</asp:Content>
