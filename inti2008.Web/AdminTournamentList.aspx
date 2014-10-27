<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminTournamentList.aspx.cs" Inherits="inti2008.Web.AdminTournamentList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Tävlingar</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Tävlingar</h4>
<div class="formLoosePane">
<asp:GridView ID="grdTournaments" runat="server" AutoGenerateColumns="false" 
        AllowPaging="false" onrowcommand="grdTournaments_RowCommand" 
        onrowdatabound="grdTournaments_RowDataBound"
        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" >
        <Columns>
            <asp:ButtonField Text="Click" CommandName="Click" visible="false" />
            <asp:BoundField DataField="GUID" visible="false" />
            <asp:BoundField DataField="Name" HeaderText="Tävling" />
            <asp:BoundField DataField="PublicateDate" HeaderText="Börja visa" />
            <asp:BoundField DataField="StartRegistration" HeaderText="Öppna för nya lag" />
            <asp:BoundField DataField="EndRegistration" HeaderText="Deadline" />
        </Columns>
    </asp:GridView>
</div>
<div class="formButtons">
<asp:Button ID="btnAddNewTournament" runat="server" Text="Lägg till" 
        ToolTip="Lägg till tävling" CssClass="actionButton" 
        onclick="btnAddNewTournament_Click" />
</div>
</asp:Content>
