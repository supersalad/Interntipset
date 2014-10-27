<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminPlayerImport.aspx.cs" Inherits="inti2008.Web.AdminPlayerImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Importera spelare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<script type="text/javascript">
    function ChangeCheckBoxState(id, checkState) {
        var cb = document.getElementById(id);
        if (cb != null)
            cb.checked = checkState;
    }

    function ChangeAllCheckBoxStates(checkState) {
        // Toggles through all of the checkboxes defined in the CheckBoxIDs array
        // and updates their value to the checkState input parameter
        if (CheckBoxIDs != null) {
            for (var i = 0; i < CheckBoxIDs.length; i++)
                ChangeCheckBoxState(CheckBoxIDs[i], checkState);
        }
    }
</script>


<asp:ScriptManager ID="scriptMan" runat="server"></asp:ScriptManager>
<h4>Importera spelare</h4>
<div class="formPane formFloat">
<table>
    <tr>
        <td class="description">
            Välj tävling:
        </td>
        <td class="input">
            <asp:DropDownList ID="drpTournament" runat="server"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Klistra in en komma-separerad lista med spelare du vill importera.<br />
            <i>Förnamn,Efternamn,lag (ex. MU),position (MGR,GK,D,M eller S),pris (i pund bara siffror)</i>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtPlayersToImport" runat="server" TextMode="MultiLine" Rows="30" Width="300" CssClass="wideDescription"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td></td>
        <td align="right">
            <asp:Button ID="btnImportPlayers" runat="server" CssClass="actionButton" OnClick="btnImportPlayers_Click" Text="Importera" />
        </td>
    </tr>
    
</table>
</div>
<div class="formWidePane formFloat">
<asp:UpdatePanel ID="updPlayerImport" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <div>
            <asp:GridView ID="grdAthletesToImport" runat="server" AutoGenerateColumns="false"
            CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow"
                OnDataBound="grdAthletesToImport_DataBound">
                <Columns>
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="InText" Visible="true" HeaderText="Indata" />
                    <asp:BoundField DataField="MainText" Visible="true" HeaderText="Spelare" />
                    <asp:BoundField DataField="ImportAction" Visible="true" HeaderText="Åtgärd" />
                    <asp:TemplateField Visible="true">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkCheckAll" runat="server" Text="Utför" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkProceed" runat="server" />    
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Är importerad">
                    <ItemTemplate>
                        <%# GetStatusImage(bool.Parse(Eval("Done").ToString()), "Spelaren är importerad", "Ej importerad") %>
                    </ItemTemplate>
                </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
    <asp:UpdatePanel ID="updProcessButton" runat="server" UpdateMode="Always">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnImportPlayers" />
    </Triggers>
    <ContentTemplate>
        <div>
        <asp:Button ID="btnProcessImport" runat="server" Visible="false" 
        Text="Lägg till/uppdatera markerade spelare" CssClass="actionButton"
        OnClick="btnPocessImport_Click" />
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
