<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="AdminEditNews.aspx.cs" Inherits="inti2008.Web.AdminEditNews" %>

<%@ Register Src="ViewChangeLog.ascx" TagName="ViewChangeLog" TagPrefix="inti" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Editera nyheter</title>
    <script src="Scripts/nicEdit.js" type="text/javascript"></script>
    <script type="text/javascript">     bkLib.onDomLoaded(function () {
         nicEditors.allTextAreas({ iconsPath: 'Scripts/nicEditorIcons.gif', buttonList: ['bold', 'italic', 'underline', 'left', 'center', 'right', 'justify', 'ol', 'ul', 'subscript', 'superscript', 'strikethrough', 'removeformat', 'indent', 'outdent', 'hr', 'image', 'forecolor', 'xhtml'] });
     }); </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>Editera nyheter</h4>
    <asp:Panel ID="pnlNewsEditor" runat="server">
        <div class="row">

            <div class="col-md-6">
                <div class="form-horizontal" role="form">
                    <div class="form-group col-md-12">
                        <label for="<%=Header.ClientID %>">Rubrik:</label>
                        <asp:TextBox ID="Header" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group col-md-12">
                        <label for="<%=Body.ClientID %>">Brödtext:</label>
                        <asp:TextBox ID="Body" runat="server" TextMode="MultiLine" CssClass="form-control" Height="100"></asp:TextBox>
                    </div>

                    <p>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </p>
                    <asp:Button ID="btnSave" runat="server" Text="Spara" ToolTip="Spara nyheten"
                        OnClick="btnSave_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="btnTweet" runat="server" Text="Twittra" OnClick="btnTweet_Click" CssClass="btn" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-horizontal" role="form">
                    <div class="form-group col-md-8">
                        <label for="<%=ValidFrom.ClientID %>">Börja visa:</label>
                        <asp:TextBox ID="ValidFrom" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group col-md-8">
                        <label for="<%=ValidTo.ClientID %>">Sluta visa:</label>
                        <asp:TextBox ID="ValidTo" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <p></p>
                </div>
            </div>
        </div>

        <div class="row">

            <div class="col-md-12">
                <h5>Ändringslogg</h5>
                <inti:ViewChangeLog ID="newsChangeLog" runat="server" />
            </div>
        </div>


    </asp:Panel>
    <div class="row">
        <div class="col-md-12">
            <asp:Button ID="btnAddNew" runat="server" Text="Lägg till nyhet"
                CssClass="btn btn-primary" OnClick="btnAddNew_Click" />
        </div>

    </div>
    <div class="row">
        <div class="col-md-12">
            <h5>Sparade nyheter</h5>
            <asp:GridView ID="grdAllNews" runat="server" AutoGenerateColumns="false"
                AllowPaging="true" PageSize="20"
                OnPageIndexChanging="grdAllNews_PageIndexChanging"
                OnRowDataBound="grdAllNews_RowDataBound"
                OnRowCommand="grdAllNews_RowCommand"
                CssClass="table table-bordered"
                SelectedRowStyle-CssClass="GridViewSelectedRow">
                <Columns>
                    <asp:ButtonField Text="Click" CommandName="Click" Visible="false" />
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="Header" HeaderText="Nyhet" />
                    <asp:BoundField DataField="ValidFrom" HeaderText="Börja visa" />
                    <asp:BoundField DataField="ValidTo" HeaderText="Sluta visa" />
                </Columns>
            </asp:GridView>
        </div>

    </div>
</asp:Content>
