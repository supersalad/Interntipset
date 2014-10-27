<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="inti2008.Web.News" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Nyheter</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h3>Nyheter</h3>
    <div class="row">
        <div class="col-md-10">
            <asp:Repeater runat="server" ID="rptNews">
                <HeaderTemplate>
                    <div class="list-group">
                        <%=(PageNumber >0) ? "<a href='/NewsList/" + (PageNumber - 1) + "' class='list-group-item'>Senare nyheter</a>" : String.Empty %>

                </HeaderTemplate>
                <ItemTemplate>
                    <a href='/News/<%#Eval("GUID")%>' class="list-group-item">
                        <div class="list-group-item-heading">
                            <%# ParseOutput(Eval("Header").ToString())%>
                        </div>
                        <div class="list-group-item-text">
                            <%# Eval("Body").ToString()%>
                        </div>
                        <div class="list-group-item-text">
                            <%# ParseDateOutput(Eval("ValidFrom").ToString()) %>
                        </div>
                    </a>
                </ItemTemplate>
                <FooterTemplate>
                    <%=(LastPage) ? "<a href='/NewsList/" + (PageNumber + 1) + "' class='list-group-item'>Äldre nyheter</a>" : String.Empty %>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="col-md-2"></div>
    </div>

    <asp:GridView ID="grdNews" runat="server" AllowPaging="true" AutoGenerateColumns="false"
        PageSize="10"
        OnPageIndexChanging="grdNews_OnPageIndexChanging">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <h4><%# ParseOutput(Eval("Header").ToString())%></h4>
                    <p><%# ParseOutput(Eval("Body").ToString())%></p>
                    <br />
                    <br />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="100px" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# ParseDateOutput(Eval("ValidFrom").ToString()) %>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
