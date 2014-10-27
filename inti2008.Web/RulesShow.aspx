<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="RulesShow.aspx.cs" Inherits="inti2008.Web.RulesShow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Regler</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4><asp:Label ID="lblRulesHeader" runat="server"></asp:Label></h4>            
    <asp:Repeater ID="rptRules" runat="server">
        <HeaderTemplate>
            
        </HeaderTemplate>
        <ItemTemplate>
            <div class="row">
            <div class="col-md-12">
                <strong><%#DataBinder.Eval(Container.DataItem, "Header")%></strong>
                <p><%#DataBinder.Eval(Container.DataItem,"Body")%></p>
            </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            
        </FooterTemplate>
    </asp:Repeater>
    
    <asp:Repeater ID="rptTransferWindows" runat="server">
        <HeaderTemplate>
            <h4>Bytesfönster</h4>            
        </HeaderTemplate>
        <ItemTemplate>
            <div class="row">
            <div class="col-md-12">
                <strong><%#DataBinder.Eval(Container.DataItem, "Header")%></strong>
                <p><%#DataBinder.Eval(Container.DataItem,"Body")%></p>
            </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            
        </FooterTemplate>
    </asp:Repeater>


</asp:Content>
