<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" validateRequest="false" CodeBehind="AdminEditRules.aspx.cs" Inherits="inti2008.Web.AdminEditRules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera regler</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Editera reglerna för <asp:Label ID="lblSelectedTournament" runat="server"/></h4>
    <asp:GridView ID="grdRules" runat="server" AutoGenerateColumns="false"
        CssClass="table table-bordered"
        OnRowCommand="grdRules_RowCommand"
        OnRowEditing="grdRules_RowEditing"
        OnRowDeleting="grdRules_RowDeleting"
        OnRowCancelingEdit="grdRules_CancelEdit"
        OnRowUpdating="grdRules_RowUpdating">
        <Columns>
            <asp:BoundField DataField="Header" HeaderText="Rubrik" />
            <asp:BoundField DataField="Body" HeaderText="Brödtext" />
            <asp:BoundField DataField="SortOrder" HeaderText="Sorteringsordning" ReadOnly="true" />
            <asp:CommandField HeaderStyle-Width="150" EditText="Editera" ShowEditButton="true" 
            UpdateText="Spara" CancelText="Avbryt" ShowCancelButton="true" 
            DeleteText="Ta bort" ShowDeleteButton="true" 
            InsertText="Lägg till" NewText="Ny" ShowInsertButton="true" />
        </Columns>
    </asp:GridView>
</asp:Content>
