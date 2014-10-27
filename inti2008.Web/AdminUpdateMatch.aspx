<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="AdminUpdateMatch.aspx.cs" Inherits="inti2008.Web.AdminUpdateMatch" %>
<%@ Register src="ViewChangeLog.ascx" tagname="ViewChangeLog" tagprefix="inti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Uppdatera matcher</title>
    <script type="text/javascript">

        function CheckIfZero(value, checkbox) {
            if (value == '0')
                checkbox.checked = true;
            else
                checkbox.checked = false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>Uppdatera matcher</h4>
    <p>Välj match nedan och fyll på med resultat och vem som gjorde vad. Omgång och datum har betydelse för vem som får poäng i samband med byten, så kontrollera att det är korrekt.</p>
    <asp:ScriptManager ID="scriptManager1" runat="server"></asp:ScriptManager>
    
    <asp:Panel ID="pnlSelectDayAndMatch" runat="server">
        <div class="formLoosePane">
            <h5>Välj omgång och match</h5>
            <table>
                <tr>
                    <td class="description">
                        Omgång:
                    </td>
                    <td class="input">
                        <asp:DropDownList ID="drpTourDay" runat="server" AutoPostBack="true"
                            onselectedindexchanged="drpTourDay_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="description">
                        Match:
                    </td>
                    <td class="input">
                        <asp:DropDownList ID="drpMatches" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnReDistributePointsForFullDay" runat="server" Text="Dela ut poäng för hela omgången"
                            ToolTip="Ta bort och dela ut poäng på nytt för alla uppdaterade matcher i omgången"
                            CssClass="actionButton" Visible="true" onclick="btnReDistributePointsForFullDay_Click" />                           
                    </td>
                    <td>
                        <asp:Button ID="btnSelectMatch" runat="server" Text="Uppdatera match" 
            ToolTip="Uppdatera den valda matchen" 
            CssClass="actionButton" Visible="true" onclick="btnSelectMatch_Click" />
                    </td>
                </tr>
            </table>
            
            
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlMatchHeader" runat="server" Visible="false">
        <div class="formLoosePane">
            <h5>Matchresultat</h5>
            <table>
                <tr>
                    <td>Hemmalag</td>
                    <td>Bortalag</td>
                    <td>Omgång</td>
                    <td>Matchdatum</td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblHomeTeam" runat="server"></asp:Label></td>
                    <td><asp:Label ID="lblAwayTeam" runat="server"></asp:Label></td>
                    <td><asp:DropDownList ID="drpMatchTourDay" runat="server"></asp:DropDownList></td>
                    <td><asp:TextBox ID="txtMatchMatchDate" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Mål:<asp:TextBox ID="txtHomeTeamGoals" runat="server" type="number"></asp:TextBox></td>
                    <td>Mål:<asp:TextBox ID="txtAwayTeamGoals" runat="server" type="number"></asp:TextBox></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Tilldela hålla-nollan-poäng:<asp:CheckBox ID="chkAddCleanSheetScoreHome" runat="server" /></td>
                    <td><asp:CheckBox ID="chkAddCleanSheetScoreAway" runat="server" /></td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnUpdateMatchHeader" runat="server" Text="Uppdatera" 
                ToolTip="Uppdatera matchinfo och gå vidare" CssClass="actionButton" 
                onclick="btnUpdateMatchHeader_Click" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlDefenderPoints" runat="server" Visible="false">
        <div class="formLoosePane">
            <h5>Poäng för att hålla nollan</h5>
            <p><asp:Label ID="lblHomeClubDefPoints" runat="server"></asp:Label></p>
            <asp:GridView ID="grdHomeDefenderPoints" runat="server" 
                AutoGenerateColumns="false"
                CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="AthleteName" HeaderText="Spelare" />
                    <asp:BoundField DataField="Position" HeaderText="Position" />
                    <asp:TemplateField HeaderText="Höll nollan">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCleanSheet" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p><asp:Label ID="lblAwayClubDefPoints" runat="server"></asp:Label></p>
            <asp:GridView ID="grdAwayDefenderPoints" runat="server" 
                AutoGenerateColumns="false"
                CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="AthleteName" HeaderText="Spelare" />
                    <asp:BoundField DataField="Position" HeaderText="Position" />
                    <asp:TemplateField HeaderText="Höll nollan">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCleanSheet" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Button ID="btnUpdateDefenderPoints" runat="server" Text="Uppdatera" 
                Tooltip="Uppdatera poäng för noll-hållare" CssClass="actionButton" 
                onclick="btnUpdateDefenderPoints_Click" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlPlayerPoints" runat="server" Visible="false">
        <div class="formLoosePanes">
            <h5>Poäng för mål, självmål och rött kort</h5>
            <p><asp:Label ID="lblHomeGoalPoints" runat="server"></asp:Label></p>
            <asp:GridView ID="grdHomeGoalPoints" runat="server" 
                AutoGenerateColumns="false"
                CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="AthleteName" HeaderText="Spelare" />
                    <asp:BoundField DataField="Position" HeaderText="Position" />
                    <asp:TemplateField HeaderText="Mål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Straffmål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpPenGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Självmål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpOwnGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rött kort">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRedCard" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Straffmiss">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpPenMissed" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p><asp:Label ID="lblAwayGoalPoints" runat="server"></asp:Label></p>
            <asp:GridView ID="grdAwayGoalPoints" runat="server" 
                AutoGenerateColumns="false"
                CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="GUID" Visible="false" />
                    <asp:BoundField DataField="AthleteName" HeaderText="Spelare" />
                    <asp:BoundField DataField="Position" HeaderText="Position" />
                    <asp:TemplateField HeaderText="Mål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Straffmål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpPenGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Självmål">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpOwnGoals" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rött kort">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRedCard" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Straffmiss">
                        <ItemTemplate>
                            <asp:DropDownList ID="drpPenMissed" runat="server" CssClass="short">
                                <asp:ListItem Text="0" Value="0" Selected=True></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Button ID="btnUpdateGoalPoints" runat="server" Text="Uppdatera" 
                ToolTip="Uppdatera poängen och gå vidare" CssClass="actionButton" 
                onclick="btnUpdateGoalPoints_Click" />
        </div>
    </asp:Panel>
    
    
    
    
    <div class="formPaneNR">
        <asp:Label ID="lblUpdateProgress" runat="server"></asp:Label>
    </div>
    
    <div class="formButtons">
        <asp:Button ID="btnClearTeamPoints" runat="server" Text="Uppdatera lagpoäng"
            Tooltip="Ta bort lag poäng och uppdatera igen (används om något lag inte var godkänt)"
            CssClass="actionButton" Visible="false" 
            onclick="btnClearTeamPoints_Click" />
        <asp:Button ID="btnClearPoints" runat="server" Text="Rensa poänghändelser" 
            ToolTip="Ta bort poängen för den valda matchen, så du kan lägga in dem på nytt" 
            CssClass="actionButton" Visible="false" onclick="btnClearPoints_Click" />
        <asp:Button ID="btnDistributePoints" runat="server" Text="Dela ut poäng" 
            ToolTip="Dela ut poäng till de tävlande baserat på det som matats in här" 
            CssClass="actionButton" Visible="false" onclick="btnDistributePoints_Click" />
    </div>
    <div class="formLoosePane">
        <h3>Ändringslogg</h3>
        <inti:ViewChangeLog ID="matchChangeLog" runat="server" />
    </div>
</asp:Content>
