<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminEditClub.aspx.cs" Inherits="inti2008.Web.AdminEditClub" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera klubb</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h1>Editera lag</h1>
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Namn:
            </td>
            <td class="input">
                <asp:TextBox ID="ClubName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Förkortning:
            </td>
            <td class="input">
                <asp:TextBox ID="ClubShortName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Beskrivning:
            </td>
            <td class="input">
                <asp:TextBox ID="ClubDescription" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div class="formButtons">
    <asp:Button ID="btnCancel" runat="server" CssClass="actionButton" Text="Avbryt" 
        ToolTip="Avbryt utan att spara ändringar" onclick="btnCancel_Click" />
    <asp:Button ID="brnSave" runat="server" CssClass="actionButton" Text="Spara" 
        ToolTip="Spara ändringar" onclick="brnSave_Click" />
</div>
</asp:Content>
