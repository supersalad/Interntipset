<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserSettings.aspx.cs" Inherits="inti2008.Web.UserSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Inställningar</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<div class="formPane">
    <table>
        <tr>
            <td class="wideDescription" colspan="2">
                Här kan du ändra namn om det känns viktigt för dig.<br />
                Klicka <a href="ChangePassword.aspx" >här</a> för att byta lösenord.
            </td>
        </tr>
        <tr>
            <td class="description">
            Förnamn:
            </td>
            <td class="input">
            <asp:TextBox ID="FirstName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="description">
            Efternamn:
            </td>
            <td class="input">
            <asp:TextBox ID="LastName" runat="server" />
            </td>
        </tr>
    </table>
</div>
<div class="formPaneNR">
    <table>
        <tr>
            <td class="wideDescription" colspan="2">
                <asp:label ID="lblMessage" runat="server"></asp:label>
            </td>
        </tr>
    </table>
</div>
<div class="formButtons">
<asp:Button ID="Cancel" runat="server" Text="Avbryt" ToolTip="Gå tillbaka utan att spara ändringar"  CssClass="btn btn-warning" OnClick="Cancel_Click" />
<asp:Button ID="Save" runat="server" Text="Spara" ToolTip="Spara ändringar"  
        CssClass="btn btn-primary" onclick="Save_Click" />
</div>
</asp:Content>
