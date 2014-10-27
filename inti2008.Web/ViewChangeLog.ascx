<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewChangeLog.ascx.cs" Inherits="inti2008.Web.ViewChangeLog" %>
<asp:GridView ID="grdChangeLog" runat="server" AutoGenerateColumns="false" 
    CssClass="table table-bordered"
    SelectedRowStyle-CssClass="GridViewSelectedRow">
    <Columns>
        <asp:BoundField DataField="LogDate" Visible="true" HeaderText="Datum" />
        <asp:BoundField DataField="Action" Visible="true" HeaderText="Händelse" />
        <asp:BoundField DataField="Message" Visible="true" HeaderText="Meddelande" />
        <asp:BoundField DataField="UserName" Visible="true" HeaderText="Användare" />
        <asp:BoundField DataField="Client" Visible="true" HeaderText="Klientinfo" />
    </Columns>
</asp:GridView>
    