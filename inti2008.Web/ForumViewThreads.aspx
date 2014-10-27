<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="ForumViewThreads.aspx.cs" Inherits="inti2008.Web.ForumViewThreads" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Forum</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<h4>Forumet</h4>
<asp:Panel ID="pnlChooseCategory" runat="server">
<div class="formPane">
    <table>
        <tr>
            <td class="description">
                Välj kategori:
            </td>
            <td class="input">
                <asp:DropDownList ID="drpForumCategory" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="drpForumCategory_SelectedIndexChanged"></asp:DropDownList>
            </td>
        </tr>
    </table>
</div>
</asp:Panel>

    <asp:Repeater ID="rptForum" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="row">
            <div class="col-md-12">
                <strong><a href="ForumViewThread.aspx?threadGUID=<%#DataBinder.Eval(Container.DataItem, "GUID")%>"><%#DataBinder.Eval(Container.DataItem, "Header")%></a></strong>
                <br/>
                <small><%#DataBinder.Eval(Container.DataItem, "PostedDate")%></small>
                <br/>
                 <%# (bool)DataBinder.Eval(Container.DataItem, "IsDeletable") ? (@"<a href='ForumViewThreads.aspx?deleteGUID=" + DataBinder.Eval(Container.DataItem, "GUID") + "'>ta bort</a>") : string.Empty %>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<br/>
    <br/>
    <br/>
<div class="row">
    <div class="col-md-12">
    <asp:Button ID="btnAddNewThread" runat="server" text="Nytt inlägg" 
        ToolTip="Skapa nytt inlägg i forumet" onclick="btnAddNewThread_Click" CssClass="btn btn-primary" />
        </div>
</div>
</asp:Content>
