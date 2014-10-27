<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="inti2008.Web.SignUp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Registrera dig</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:Panel id="pnlForm" runat="server">
        <div class="row">
            <div class="col-md-4">
                <p><strong>Fyll i din epostadress, lösenord och namn. Klicka sedan på Registrera mig för att registrera dig.</strong></p>
                <form role="form">
                    <div class="form-group">
                        <label for="<%=Email.ClientID %>">Email</label>
                        <asp:TextBox ID="Email" runat="server" CssClass="form-control" />
                        
                      </div>
                      <div class="form-group">
                        <label for="<%=Password.ClientID %>">Lösenord</label>
                          <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="form-control" />
                        
                      </div>
                    <div class="form-group">
                        <label for="<%=PasswordConfirm.ClientID %>">Bekräfta lösenord</label>
                          <asp:TextBox ID="PasswordConfirm" runat="server" TextMode="Password" CssClass="form-control" />
                        
                      </div>
                    <div class="form-group">
                        <label for="<%=FirstName.ClientID %>">Förnamn</label>
                          <asp:TextBox ID="FirstName" runat="server" CssClass="form-control" />
                        
                      </div>
                    <div class="form-group">
                        <label for="<%=LastName.ClientID %>">Efternamn</label>
                          <asp:TextBox ID="LastName" runat="server" CssClass="form-control" />
                        
                      </div>
                        <asp:Panel ID="pnlButtons" runat="server">
    
        <asp:Button ID="SignMeUp" runat="server" Text="Registrera mig" 
                ToolTip="Registrera denna användare" onclick="SignMeUp_Click"  CssClass="btn btn-primary" />
    
</asp:Panel>
                </form>        
            </div>
        </div>
        
    
</asp:Panel>
<div class="formPaneNR">
    <table>
        <tr>
            <td class="wideDescription" colspan="2">
                <asp:label ID="lblMessage" runat="server"></asp:label>
            </td>
        </tr>
    </table>
</div>

</asp:Content>
