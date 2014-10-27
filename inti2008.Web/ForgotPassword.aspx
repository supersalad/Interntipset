<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="inti2008.Web.ForgotPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Glömt lösenord</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<div class="row">
    <div class="col-md-4">
        <form role="form">
            <p><strong>Fyll i din epostaddress nedan och klicka på "skicka". Ett nytt lösenord kommer skickas till dig.</strong></p>
            <div class="form-group">
                <label for="<%=Email.ClientID %>">Email</label>
                          <asp:TextBox ID="Email" runat="server" CssClass="form-control" />
            </div>
            <asp:Button ID="Send" runat="server" Text="Skicka" ToolTip="Skicka lösenordet" CssClass="btn btn-primary"
        onclick="Send_Click" />
        </form>
        
        
    </div>

</div>
    <div class="row">
        <div class="col-md-4">
            <asp:label ID="lblMessage" runat="server"></asp:label>
        </div>
    </div>
</asp:Content>
