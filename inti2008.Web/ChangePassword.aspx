<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="inti2008.Web.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Ändra lösenord</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<div class="formPane">
    <table>
        <tr>
            <td class="wideDescription" colspan="2">
                Ange ditt nuvarande och ditt önskade lösenord nedan.
            </td>
        </tr>
        <tr>
            <td class="description">
            Nuvarande lösenord:
            </td>
            <td class="input">
            <asp:TextBox ID="PresentPassword" runat="server" TextMode="Password" />
            </td>
        </tr>
        <tr>
            <td class="description">
            Nytt lösenord:
            </td>
            <td class="input">
            <asp:TextBox ID="NewPassword" runat="server"  TextMode="Password"/>
            </td>
        </tr>
        <tr>
            <td class="description">
            Bekräfta:
            </td>
            <td class="input">
            <asp:TextBox ID="ConfirmNewPassword" runat="server"  TextMode="Password"/>
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
