<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminPlayerList.aspx.cs" Inherits="inti2008.Web.AdminPlayerList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <title>Editera spelare</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
<asp:ScriptManager ID="scriptMan" runat="server"></asp:ScriptManager>
<h4>Sök spelare att uppdatera</h4>
<p>Sök spelare att editera nedan</p>
    <asp:UpdatePanel ID="updSearchPlayer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="formPane">
                <table>
                    <tr>
                        <td class="description">
                            Sök:
                        </td>
                        <td class="input">
                            <asp:TextBox ID="SearchInput" runat="server" autocomplete="off" AutoCompleteType="None"></asp:TextBox>
                            <cc1:AutoCompleteExtender ID="AutoCompleteExtenderSearchInput" 
                                    runat="server" 
                                    BehaviorID="AutoCompleteEx"
                                    TargetControlID="SearchInput" 
                                    ServiceMethod="CompletePlayers"
                                    ServicePath="Completion.asmx" 
                                    MinimumPrefixLength="2" 
                                    CompletionInterval="500" 
                                    EnableCaching="true" 
                                    CompletionSetCount="10"
                                    DelimiterCharacters=";,: " 
                                    CompletionListCssClass="autocomplete_completionListElement"
                                    CompletionListItemCssClass="autocomplete_listItem" 
                                    CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem">
                                    <Animations>
                                        <OnShow>
                                            <Sequence>
                                                <%-- Make the completion list transparent and then show it --%>
                                                <OpacityAction Opacity="0" />
                                                <HideAction Visible="true" />
                                                
                                                <%--Cache the original size of the completion list the first time
                                                    the animation is played and then set it to zero --%>
                                                <ScriptAction Script="
                                                    // Cache the size and setup the initial size
                                                    var behavior = $find('AutoCompleteEx');
                                                    if (!behavior._height) {
                                                        var target = behavior.get_completionList();
                                                        behavior._height = target.offsetHeight - 2;
                                                        target.style.height = '0px';
                                                    }" />
                                                
                                                <%-- Expand from 0px to the appropriate size while fading in --%>
                                                <Parallel Duration=".4">
                                                    <FadeIn />
                                                    <Length PropertyKey="height" StartValue="0" EndValueScript="$find('AutoCompleteEx')._height" />
                                                </Parallel>
                                            </Sequence>
                                        </OnShow>
                                        <OnHide>
                                            <%-- Collapse down to 0px and fade out --%>
                                            <Parallel Duration=".4">
                                                <FadeOut />
                                                <Length PropertyKey="height" StartValueScript="$find('AutoCompleteEx')._height" EndValue="0" />
                                            </Parallel>
                                        </OnHide>       
                                    </Animations>
                                </cc1:AutoCompleteExtender>
                        </td>
                        <td class="input">
                            <asp:Button ID="btnSearch" runat="server" Text="Sök" CssClass="actionButton" 
                                onclick="btnSearch_Click" />
                            <asp:Button ID="btnAddNew" runat="server" Text="Lägg till" 
                                CssClass="actionButton" onclick="btnAddNew_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="formPane">
                <asp:Button ID="btnImport" runat="server" Text="Importera" CssClass="actionButton"
                    onclick="btnImport_Click" />
            </div>
            <asp:Panel ID="pnlSearchResult" runat="server">
                <div class="formPaneNR">
                    <h2>Sökresultat</h2>
                    <asp:GridView ID="grdPlayers" runat="server" AutoGenerateColumns="false" 
                        onrowdatabound="grdPlayers_RowDataBound" 
                        onrowcommand="grdPlayers_RowCommand"
                        CssClass="table table-bordered"
        SelectedRowStyle-CssClass="GridViewSelectedRow" >
                        <Columns>
                            <asp:ButtonField Text="Click" CommandName="Click" visible="false" />
                            <asp:BoundField DataField="GUID" visible="false" />
                            <asp:BoundField DataField="FirstName" HeaderText="Förnamn" />
                            <asp:BoundField DataField="LastName" HeaderText="Efternamn" />
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
