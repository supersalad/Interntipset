<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserTeamView.aspx.cs" Inherits="inti2008.Web.UserTeamView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title><%=Header.Text %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>
        <asp:Label ID="Header" runat="server"></asp:Label></h4>
    <!-- lista användarens lag -->
    <div class="row">
        <div class="col-md-4">
            <h5>Lag</h5>
            <asp:GridView ID="UsersTeams" runat="server" AutoGenerateColumns="false"
                OnRowCommand="UsersTeams_RowCommand" OnRowDataBound="UsersTeams_RowDataBound"
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow">
                <Columns>
                    <asp:ButtonField CommandName="Click" Visible="false" />
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="Name" Visible="true" HeaderText="Lag" />
                </Columns>
            </asp:GridView>
        </div>
        
        <asp:Panel ID="pnlUserSettings" runat="server" Visible="false">
        <div class="col-md-4">

            <h5>Mina uppgifter</h5>
            <a href="UserSettings.aspx">Ändra användaruppgifter</a><br />
            <a href="ChangePassword.aspx">Byt lösenord</a><br />
            <asp:LinkButton ID="lnkMessages" runat="server" OnClick="lnkMessages_OnClick">Meddelanden</asp:LinkButton>
        </div>

        <div class="col-md-4">
            <h5>Mina privata uppgörelser</h5>
            <p>Här visas interna tävlingar som du själv satt igång.</p>
            <asp:GridView ID="grdMyTournaments" runat="server" AutoGenerateColumns="false"
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow"
                OnRowCommand="grdMyTournaments_RowCommand" OnRowDataBound="grdMyTournaments_RowDataBound">
                <Columns>
                    <asp:ButtonField CommandName="Click" Visible="false" />
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="Name" Visible="true" HeaderText="Turnering" />
                </Columns>
            </asp:GridView>
            <asp:Button ID="btnNewTournament" runat="server" Text="Ny intern turnering"
                ToolTip="Skapa en ny intern turnering" CssClass="btn"
                OnClick="btnNewTournament_Click" />

            <div>
                <h5>Privata uppgörelser</h5>
                <p>Här visas privata uppgörelser som något av dina lag deltar i.</p>
                <p>
                    <asp:Label ID="lblPrivateTournaments" runat="server"></asp:Label></p>
                <asp:GridView ID="grdPrivateTournaments" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-bordered"
                    SelectedRowStyle-CssClass="GridViewSelectedRow"
                    OnRowCommand="grdPrivateTournaments_RowCommand" OnRowDataBound="grdPrivateTournaments_RowDataBound">
                    <Columns>
                        <asp:ButtonField CommandName="Click" Visible="false" />
                        <asp:BoundField DataField="GUID" Visible="false" />
                        <asp:BoundField DataField="Name" Visible="true" HeaderText="Turnering" />
                        <asp:BoundField DataField="TeamName" Visible="true" HeaderText="Lag" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <%# GetStatusImage(bool.Parse(Eval("IsAccepted").ToString()), "Utmaningen är accepterad", "Du har inte tagit ställning till utmaningen än") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField CommandName="Accept" HeaderText="Acceptera" ButtonType="Image" ImageUrl="~/img/icon_accept.gif" Text="Acceptera" />
                        <asp:ButtonField CommandName="Deny" HeaderText="Avstå" ButtonType="Image" ImageUrl="~/img/action_stop.gif" Text="Avstå" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>
    </div>
    

    <div class="row">
        <asp:PlaceHolder ID="plhAddTeams" runat="server"></asp:PlaceHolder>
    </div>
</asp:Content>
