<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumEntries.ascx.cs" Inherits="inti2008.Web.ForumEntries" %>
<h4>Senaste foruminläggen</h4>
<asp:Repeater ID="rptForum" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="row">
            <div class="col-md-12">
                <strong><a href="ForumViewThread.aspx?threadGUID=<%#DataBinder.Eval(Container.DataItem, "ThreadGUID")%>"><%#DataBinder.Eval(Container.DataItem, "Header")%></a></strong>
                <p><%#DataBinder.Eval(Container.DataItem,"Body")%></p>
                <small><%#DataBinder.Eval(Container.DataItem,"Author")%>, <%#DataBinder.Eval(Container.DataItem, "PostedDate")%></small>
                <br/>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
