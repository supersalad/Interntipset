<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminEditUser.aspx.cs" Inherits="inti2008.Web.AdminEditUser" %>

<%@ Register Src="ViewChangeLog.ascx" TagName="ViewChangeLog" TagPrefix="inti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Editera användare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:Panel ID="pnlEditUser" runat="server" Visible="false">
        <div class="row">
            <div class="alert alert-success" id="divMessage" runat="server">
            </div>
        </div>


        <div class="form-horizontal" role="form">
            <div class="col-md-6">
                <div class="form-group">
                    <label for="<%=UserName.ClientID %>">Användarnamn:</label>
                    <asp:TextBox ID="UserName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="<%=FirstName.ClientID %>">Förnamn:</label>
                    <asp:TextBox ID="FirstName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="<%=LastName.ClientID %>">Efternamn:</label>
                    <asp:TextBox ID="LastName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <asp:Button ID="btnSave" runat="server" Text="Spara" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnResetPassword" runat="server" Text="Nytt lösenord" CssClass="btn btn-warning" OnClick="btnResetPassword_Click" />
            </div>
            <div class="col-md-6">
                <asp:GridView ID="grdPermissions" runat="server" AutoGenerateColumns="false"
                    AllowPaging="false" CssClass="table table-bordered"
                    SelectedRowStyle-CssClass="GridViewSelectedRow">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Perm." />
                        <asp:BoundField DataField="Description" HeaderText="Beskrivning" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkPermission" runat="server" Checked='<%# Bind("HasPermission") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="row">
            <h3>Ändringslogg</h3>
            <inti:ViewChangeLog ID="userChangeLog" runat="server" />
        </div>
    </asp:Panel>
    <div class="row">
        <div class="col-md-12">
            <asp:GridView ID="grdUsers" runat="server" AutoGenerateColumns="false"
                AllowPaging="false" OnRowCommand="grdUsers_RowCommand"
                OnRowDataBound="grdUsers_RowDataBound"
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow">
                <Columns>
                    <asp:ButtonField Text="Click" CommandName="Click" Visible="false" />
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="UserName" HeaderText="Användare" />
                    <asp:BoundField DataField="FirstName" HeaderText="Förnamn" />
                    <asp:BoundField DataField="LastName" HeaderText="Efternamn" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
