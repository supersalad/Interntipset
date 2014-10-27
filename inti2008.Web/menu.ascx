<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="menu.ascx.cs" Inherits="inti2008.Web.menu" %>
<div class="btn-toolbar" role="toolbar">
  <div class="btn-group">
    <a href="/Default.aspx" class="btn btn-default" >Start</a>
    
    <a href="/RulesShow.aspx" class="btn btn-default">Regler</a>

    <a href="/Standing.aspx" class="btn btn-default">Ställning</a>
    
    <asp:Button ID="BtnMyPage" runat="server"  Text="Min sida" 
        onclick="BtnMyPage_Click" CssClass="btn btn-default" />

    <asp:ImageButton ID="BtnNewMessage" runat="server" ImageUrl="img\email.png" Visible="false" 
        onclick="BtnNewMessage_Click" CssClass="btn btn-default" />

    <a href="/PlayerList.aspx" class="btn btn-default">Spelare</a>
    
    <a href="/ForumViewThreads.aspx" class="btn btn-default">Forum</a>  
  </div>
  <div class="btn-group pull-right">
      <a href="/AdminMain.aspx" runat="server" ID="lnkAdmin" class="btn btn-default">Admin</a>
    
        <asp:Button ID="BtnSignOut" runat="server" CssClass="btn btn-default" Text="Logga ut" onclick="BtnSignOut_Click" />

        <a href="/SignIn.aspx" runat="server" ID="lnkSignIn" class="btn btn-default">Logga in</a>
    
        <a href="/SignUp.aspx" runat="server" ID="lnkSignUp" class="btn btn-default">Registrera</a>
  </div>
</div>

    
    
    
    