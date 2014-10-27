<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminApproveUserTeams.aspx.cs" Inherits="inti2008.Web.AdminApproveUserTeams" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Godkänn lag</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>Godkänn lag</h4>
    <asp:Label ID="lblTournament" runat="server"></asp:Label>
<div class="formLoosePane">
    <asp:GridView ID="grdUserTeams" runat="server" AutoGenerateColumns="false" 
        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" 
        onrowcommand="grdUserTeams_RowCommand" 
        onrowdatabound="grdUserTeams_RowDataBound" >
        <Columns>
            <asp:ButtonField Text="Click" CommandName="Click" visible="false" />
            <asp:BoundField DataField="GUID" HeaderText="GUID" />
            <asp:BoundField DataField="Name" HeaderText="Lagnamn" />
            <asp:HyperLinkField DataTextField="TeamManager" DataNavigateUrlFields="TeamManagerMail" DataNavigateUrlFormatString="mailto:{0}" HeaderText="LagKapten" />
            <asp:HyperLinkField DataTextField="TeamManagerMail" DataNavigateUrlFields="TeamManagerMail" DataNavigateUrlFormatString="mailto:{0}" HeaderText="Mail" />
            <asp:BoundField DataField="IsActive" HeaderText="Aktiverat" Visible="true" ItemStyle-Width="0px" />
            <asp:BoundField DataField="IsPaid" HeaderText="Betalt" Visible="true" ItemStyle-Width="0px" />
            <asp:BoundField DataField="BonusPoints" HeaderText="Bonus" Visible="true" ItemStyle-Width="0px" />
            <asp:TemplateField HeaderText="Bild">
                <ItemTemplate>
                    <img width="30" src='<%# String.Format("img\\user\\{0}", Eval("Picture"))%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:ButtonField Text="Aktivera" CommandName="Activate" />
            <asp:ButtonField Text="Markera som betalt" CommandName="MarkAsPaid" />
            <asp:ButtonField Text="Bonuspoäng" CommandName="Bonus" />
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
