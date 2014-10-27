<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminEditMatch.aspx.cs" Inherits="inti2008.Web.AdminEditMatch" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera matcher</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:LinqDataSource ID="ldsClubs" runat="server" ContextTypeName="inti2008.Data.IntiDataContext" TableName="Inti_Club" 
Where="TournamentGUID == Guid(@TournamentGUID)">
<WhereParameters>
    <asp:SessionParameter SessionField="rp_GUID" Type="Object" Name="TournamentGUID" />
</WhereParameters>
</asp:LinqDataSource>
<h4>Editera matcher</h4>
<div class="formLoosePane">
    <asp:GridView ID="grdMatches" runat="server"
        AutoGenerateColumns="false" 
        CssClass="table table-bordered"
        onrowdatabound="grdMatches_RowDataBound">
        <Columns>
            <asp:BoundField DataField="GUID" Visible="false" />
            <asp:TemplateField HeaderText="Hemmalag">
                <ItemTemplate>
                    <asp:DropDownList ID="drpHomeTeam" runat="server" DataSourceID="ldsClubs" DataValueField="GUID" DataTextField="Name" SelectedValue='<%# Bind("HomeClub") %>' Enabled="false"></asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Bortalag">
                <ItemTemplate>
                    <asp:DropDownList ID="drpAwayTeam" runat="server" DataSourceID="ldsClubs" DataValueField="GUID" DataTextField="Name" SelectedValue='<%# Bind("AwayClub") %>' Enabled="false"></asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Omgång">
                <ItemTemplate>
                    <asp:DropDownList ID="drpDay" runat="server" SelectedValue='<%# Bind("TourDay") %>'>
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
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Matchdatum">
                <ItemTemplate>
                    <asp:TextBox ID="txtMatchDate" runat="server" Text='<%# Bind("MatchDate") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:TextBox ID="txtOrgTourDay" runat="server" Text='<%# Bind("TourDay") %>' CssClass="hidden" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:TextBox ID="txtOrgMatchDate" runat="server" Text='<%# Bind("MatchDate") %>' CssClass="hidden" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
<div class="formButtons">
    <asp:Button ID="btnSave" runat="server" CssClass="actionButton" Text="Spara" 
        ToolTip="Spara ändringar ovan" onclick="btnSave_Click" />
</div>
<div class="formPaneNR">
    <h2>Lägg till match</h2>
    <table>
        <tr>
            <td>Hemmalag</td>
            <td>Bortalag</td>
            <td>Omgång</td>
            <td>Matchdatum</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="drpAddMatchHomeTeam" runat="server"></asp:DropDownList></td>
            <td><asp:DropDownList ID="drpAddMatchAwayTeam" runat="server"></asp:DropDownList></td>
            <td><asp:DropDownList ID="drpAddMatchTourDay" runat="server"></asp:DropDownList></td>
            <td><asp:TextBox ID="txtAddMatchMatchDate" runat="server"></asp:TextBox></td>
        </tr>
    </table>
</div>
<div class="formButtons">
    <asp:Button ID="btnAddMatch" runat="server" Text="Lägg till match" 
        ToolTip="Lägg till match mellan lagen ovan" CssClass="actionButton" 
        onclick="btnAddMatch_Click"  />
</div>
</asp:Content>
