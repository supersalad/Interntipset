<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="ForumViewThread.aspx.cs" Inherits="inti2008.Web.ForumViewThread" %>
<%@ Import Namespace="System.Activities.Statements" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Forum</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:Panel ID="pnlViewForumItems" runat="server" Visible="false">
    <asp:Repeater ID="rptForum" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="row">
            <div class="col-md-12">
                <strong><a href="ForumViewThread.aspx?threadGUID=<%#DataBinder.Eval(Container.DataItem, "GUID")%>"><%#DataBinder.Eval(Container.DataItem, "Header")%></a></strong>
                <p><%#DataBinder.Eval(Container.DataItem,"Body")%></p>
                <small><%#DataBinder.Eval(Container.DataItem,"Author")%>, <%#DataBinder.Eval(Container.DataItem, "PostedDate")%></small>
                <br/>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
</asp:Panel>

    <br/>
<asp:Panel ID="pnlAddForumItem" runat="server" Visible="false">
    <div>
        <p><asp:Label ID="lblMessage" runat="server"></asp:Label></p>
        
        <form role="form">
             <div class="form-group">
                <label for="<%=Header.ClientID %>">Rubrik:</label>
                <asp:TextBox ID="Header" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            <div class="form-group">
                <asp:TextBox ID="Body" runat="server" MaxLength="1023" CssClass="form-control" TextMode="MultiLine" />
            </div>
            
            <div class="form-group">
                <asp:Button ID="btnCreateForumItem" runat="server" Text="Skicka inlägg" 
                    ToolTip="Skapa ett nytt inlägg"  CssClass="btn btn-primary" Visible="false" 
                    onclick="btnCreateForumItem_Click" />
                <asp:Button ID="btnReply" runat="server" Text="Svara" 
                    ToolTip="Skriv ett svar på inlägget"  CssClass="btn btn-primary" visible="false" 
                    onclick="btnReply_Click" />
            </div>
        </form>
    </div>    
</asp:Panel>
</asp:Content>
