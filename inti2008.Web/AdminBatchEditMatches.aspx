<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminBatchEditMatches.aspx.cs" Inherits="inti2008.Web.AdminBatchEditMatches" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera matcher</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">

<h4>Sätt omgång, datum på matcher</h4>
<asp:Label ID="tournamentName" runat="server"></asp:Label>
<div class="formPane">
    <i><%=TourId.ToString() %></i>
    <table>
        <tr>
            <td class="description">
                Omgång
            </td>
            <td class="input">
                <asp:DropDownList ID="drpDay" runat="server">
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                        <asp:ListItem Text="7" Value="7"></asp:ListItem>
                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                        <asp:ListItem Text="9" Value="9"></asp:ListItem>
                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                        <asp:ListItem Text="11" Value="11"></asp:ListItem>
                        <asp:ListItem Text="12" Value="12"></asp:ListItem>
                        <asp:ListItem Text="13" Value="13"></asp:ListItem>
                        <asp:ListItem Text="14" Value="14"></asp:ListItem>
                        <asp:ListItem Text="15" Value="15"></asp:ListItem>
                        <asp:ListItem Text="16" Value="16"></asp:ListItem>
                        <asp:ListItem Text="17" Value="17"></asp:ListItem>
                        <asp:ListItem Text="18" Value="18"></asp:ListItem>
                        <asp:ListItem Text="19" Value="19"></asp:ListItem>
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="21" Value="21"></asp:ListItem>
                        <asp:ListItem Text="22" Value="22"></asp:ListItem>
                        <asp:ListItem Text="23" Value="23"></asp:ListItem>
                        <asp:ListItem Text="24" Value="24"></asp:ListItem>
                        <asp:ListItem Text="25" Value="25"></asp:ListItem>
                        <asp:ListItem Text="26" Value="26"></asp:ListItem>
                        <asp:ListItem Text="27" Value="27"></asp:ListItem>
                        <asp:ListItem Text="28" Value="28"></asp:ListItem>
                        <asp:ListItem Text="29" Value="29"></asp:ListItem>
                        <asp:ListItem Text="30" Value="30"></asp:ListItem>
                        <asp:ListItem Text="31" Value="31"></asp:ListItem>
                        <asp:ListItem Text="32" Value="32"></asp:ListItem>
                        <asp:ListItem Text="33" Value="33"></asp:ListItem>
                        <asp:ListItem Text="34" Value="34"></asp:ListItem>
                        <asp:ListItem Text="35" Value="35"></asp:ListItem>
                        <asp:ListItem Text="36" Value="36"></asp:ListItem>
                        <asp:ListItem Text="37" Value="37"></asp:ListItem>
                        <asp:ListItem Text="38" Value="38"></asp:ListItem>
                    </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="description">
                Datum:
            </td>
            <td class="input">
                <asp:TextBox ID="txtDate" runat="server" type="date"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="description">
                Lag (hemmalag-bortalag), förkortning:
            </td>
            <td class="input">
                <asp:TextBox TextMode="MultiLine" Rows="11" ID="txtMatches" runat="server"></asp:TextBox>
            </td>
        </tr>
   </table>
</div>
<div class="formButtons">
    <asp:Button ID="btnSave" CssClass="actionButton"
     runat="server" text="spara" onclick="btnSave_Click"/>
</div>
<div class="formPane">
    <table>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblOutput" runat="server" />
            </td>
        </tr>
    </table>
</div>

</asp:Content>
