<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserView.aspx.cs" Inherits="inti2008.Web.UserView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title><%=UserName.Text %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div class="row">
        <div class="col-md-12">
            <h1>
                <asp:Label ID="UserName" runat="server"></asp:Label></h1>
            <asp:GridView ID="UserTeams" runat="server" AutoGenerateColumns="false"
                OnRowCommand="UserTeams_RowCommand"
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow">
                <Columns>
                    <asp:CommandField text="SingleClick" commandName="SingleClick" Visible="false" />
                    <asp:BoundField DataField="Name" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
