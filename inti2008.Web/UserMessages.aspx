<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserMessages.aspx.cs" Inherits="inti2008.Web.UserMessages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Meddelanden</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4><asp:label ID="Header" runat="server"></asp:label></h4>
<div class="row">
        <div class="col-md-12">
            <h5>Meddelanden</h5>
            <asp:GridView ID="grdNewMessages" runat="server" AutoGenerateColumns="false" 
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow" 
                onrowcommand="grdNewMessages_RowCommand" 
                onrowdatabound="grdNewMessages_RowDataBound" >
                <Columns>
                    <asp:ButtonField CommandName="Click" Visible="false" />
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="FromName" Visible="true" HeaderText="Från" />
                    <asp:BoundField DataField="Header" Visible="true" HeaderText="Ämne" />
                    <asp:BoundField DataField="SentDate" Visible="true" HeaderText="Skickat" />
                    <asp:TemplateField HeaderText="Läst">
                        <ItemTemplate>
                            <%# GetMailReadStatusIcon((DateTime?)Eval("ReadOn"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="btnSendMessage" runat="server" Text="Nytt meddelande" 
                ToolTip="Skicka ett nytt meddelande" CssClass="btn btn-primary" 
                onclick="btnSendMessage_Click" />
        </div>
    </div>
</asp:Content>
