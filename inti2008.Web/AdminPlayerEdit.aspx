<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminPlayerEdit.aspx.cs" Inherits="inti2008.Web.AdminPlayerEdit" validateRequest="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera spelare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<h1>Editera spelare</h1>
<div class="formLoosePane">
    <table>
        <tr>
            <td class="description">
                Förnamn:
            </td>
            <td class="input">
                <asp:TextBox ID="FirstName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Efternamn:
            </td>
            <td class="input">
                <asp:TextBox ID="LastName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
            </td>
            <td>
                <asp:Label ID="PlayerEditMessage" runat="server" />
            </td>
        </tr>
    </table>
</div>

    <asp:UpdatePanel ID="updTournament" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="formLoosePane">
                 <table>
                    <tr>
                        <td class="description">
                            Välj tävling:
                        </td>
                        <td class="input">
                            <asp:DropDownList ID="drpTournament" runat="server" AutoPostBack="true"
                                onselectedindexchanged="drpTournament_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                    </tr>
                 </table>
                 <table>
                    <tr>
                        <td class="description">
                            <b>Tävling</b>
                        </td>
                        <td class="input">
                            <b>Lag</b>
                        </td>
                        <td class="input">
                            <b>Position</b>
                        </td>
                        <td class="input">
                            <b>Pris</b>
                        </td>
                        <td class="input">
                            <b>Aktiv</b>
                        </td>
                    </tr>
                    <tr>
                        <td class="description">
                            <asp:Label ID="lblTournament" runat="server"></asp:Label>
                            <asp:TextBox ID="AthleteClubGUID" runat="server" Visible="false"></asp:TextBox>     
                        </td>
                        <td class="input">
                            <asp:DropDownList ID="drpClubs" runat="server"></asp:DropDownList>
                        </td>
                        <td class="input">
                            <asp:DropDownList ID="drpPosition" runat="server"></asp:DropDownList>
                        </td>
                        <td class="input">
                            <asp:TextBox ID="Price" runat="server"></asp:TextBox>
                        </td>
                        <td class="input">
                            <asp:CheckBox ID="IsActive" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>        
        </ContentTemplate>
    </asp:UpdatePanel>

<div class="formButtons">
    <asp:Button ID="btnCancel" runat="server" Text="Avbryt" 
        ToolTip="Avsluta utan att spara ändringar" CssClass="actionButton" 
        onclick="btnCancel_Click" />
    <asp:Button ID="btnSave" runat="server" Text="Spara" ToolTip="Spara ändringar" 
        CssClass="actionButton" onclick="btnSave_Click" />
</div>
</asp:Content>
