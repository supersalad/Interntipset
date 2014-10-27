<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Rss.ascx.cs" Inherits="inti2008.Web.Rss" %>
<asp:DataList ID="RssOutput" runat="server" DataSourceID="RssData" OnItemDataBound="RssOutput_ItemDataBound" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" CellPadding="4">
    <ItemTemplate>
        <asp:HyperLink ID="lnkTitle" runat="server" NavigateUrl='<%# XPath("link") %>' Target="_blank" Text='<%# XPath("title") %>'></asp:HyperLink>
        <asp:Label ID="lblPubDate" runat="server">(PubDate)</asp:Label><br />
        <asp:Label ID="lblDescription" runat="server" Text='<%# XPath("description") %>'></asp:Label>
    </ItemTemplate>
</asp:DataList>

<asp:XmlDataSource CacheDuration="600" ID="RssData" runat="server" XPath="/rss/channel/item">
</asp:XmlDataSource>
