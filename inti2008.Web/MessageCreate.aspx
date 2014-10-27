<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MessageCreate.aspx.cs" Inherits="inti2008.Web.MessageCreate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Meddelande</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:Panel ID="pnlEditControls" runat="server">
    <div class="formLoosePane">
        <table>
            <tr>
                <td class="description">
                    Ämne:
                </td>
                <td class="input">
                    <asp:TextBox ID="Header" runat="server" MaxLength="99"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="description">
                    Mottagare:
                </td>
                <td class="input">
                    <asp:DropDownList ID="drpRecipient" runat="server"></asp:DropDownList>
                </td>
            </tr>
         </table>
         <table>
            <tr>
                <td>
                    <asp:TextBox ID="Body" runat="server" CssClass="textInput" TextMode="MultiLine" MaxLength="4000" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formPaneNR">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>
    <div class="formButtons">
        <asp:Button ID="btnCancel" runat="server" text="Avbryt" 
            ToolTip="Ångra utan att spara" CssClass="actionButton" 
            onclick="btnCancel_Click" />
        <asp:Button ID="btnSend" runat="server" Text="Skicka" 
            ToolTip="Skicka meddelandet" CssClass="actionButton" onclick="btnSend_Click" />
    </div>
</asp:Panel>
<asp:Panel ID="pnlSuccessControls" runat="server" Visible="false">
<div class="formPane">
    <asp:Label ID="lblSuccessMessage" runat="server"></asp:Label>
</div>
</asp:Panel>
</asp:Content>
