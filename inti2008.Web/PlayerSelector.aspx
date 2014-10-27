<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="PlayerSelector.aspx.cs" Inherits="inti2008.Web.PlayerSelector" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Välj spelare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Välj spelare</h4>
<p>Klicka på den spelare du vill ha</p>
<asp:GridView ID="grdPlayers" runat="server" AutoGenerateColumns="false" 
        onrowcommand="grdPlayers_RowCommand" onrowdatabound="grdPlayers_RowDataBound"
        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" >
<Columns>
<asp:ButtonField CommandName="Click" Visible="false" />
<asp:BoundField DataField="GUID" Visible="false" />
<asp:BoundField DataField="PlayerName" HeaderText="Spelare" />
<asp:BoundField DataField="ClubShortName" HeaderText="Lag" />
<asp:BoundField DataField="Position" HeaderText="Position" />
<asp:BoundField DataField="Price" HeaderText="Pris" />
</Columns>
</asp:GridView>
</asp:Content>
