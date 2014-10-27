<%@ Page Title="" Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="MediaPlayer.aspx.cs" Inherits="inti2008.Web.MediaPlayer" %>

<%@ Register Assembly="System.Web.Silverlight" Namespace="System.Web.UI.SilverlightControls"
    TagPrefix="asp" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Media</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:ScriptManager ID="scriptManager" runat="server" />
    
<h1><asp:Label ID="lblHeader" runat="server" /></h1>
<p><asp:Label ID="lblDescription" runat="server" /></p>


<div>
<asp:MediaPlayer ID="_mediaPlayer" runat="server" Height="680px" ScaleMode="Stretch"
        Version="2.0" Width="882px" MediaSkinSource="~/media/Professional.xaml">
    </asp:MediaPlayer>
</div>
</asp:Content>
