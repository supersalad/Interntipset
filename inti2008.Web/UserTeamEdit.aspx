<%@ Page Language="C#" MasterPageFile="~/inti2008.Master" AutoEventWireup="true" CodeBehind="UserTeamEdit.aspx.cs" Inherits="inti2008.Web.UserTeamEdit" %>

<%@ Register Src="ViewChangeLog.ascx" TagName="ViewChangeLog" TagPrefix="inti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Editera lag</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h4>Editera lag</h4>
    
        <div class="row">
            <div class="col-md-4">
                <p>
                    <asp:Label ID="lblTournamentInfo" runat="server"></asp:Label>
                </p>
                <p>
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </p>
                <p>
                    <asp:Button ID="btnActivate" runat="server" CssClass="btn btn-danger"
                        Text="Skicka laget"
                        ToolTip="Klicka här när du är nöjd med laget och inte vill ändra något mer"
                        OnClick="btnActivate_Click" />
                </p>
                <p>
                    <asp:Label ID="lblPaymentInfo" runat="server"></asp:Label>
                </p>
                <p>
                    <asp:Label ID="lblTransferPeriodInfo" runat="server"></asp:Label>
                </p>
            </div>


            <asp:Panel ID="pnlSavedTeamInfo" runat="server" Visible="false">
                <div class="col-md-4">
                    <h3>
                        <asp:Label ID="lblTeamName" runat="server"></asp:Label></h3>
                    <p>
                        <asp:Label ID="lblTeamDescription" runat="server"></asp:Label>
                    </p>
                    <p>
                        <asp:Label ID="lblTeamSystem" runat="server"></asp:Label>
                    </p>
                    <p>
                        <asp:Label ID="lblTeamBudget" runat="server"></asp:Label>
                    </p>
                    <p>
                        <asp:Label ID="lblNmbrOfTransfers" runat="server"></asp:Label>
                    </p>
                </div>
            </asp:Panel>

            <div class="col-md-4">
                <div>
                    <asp:Image ID="imgTeamImage" runat="server" />
                </div>
                <div>
                    <asp:Button ID="btnUploadImage" runat="server" Text="Ladda upp bild"
                        CssClass="btn" OnClick="btnUploadImage_Click" />
                </div>
                <div>
                    <asp:Label ID="lblUploadImageInfo" runat="server" />
                </div>
            </div>
        

        <asp:Panel ID="pnlTeamEdit" runat="server" Visible="false">
            <div class="col-md-4">
                <form role="form">
                      <div class="form-group">
                        <label for="<%=TeamName.ClientID %>">Lagets namn:</label>
                        <asp:TextBox ID="TeamName" runat="server" MaxLength="49" CssClass="form-control"></asp:TextBox>
                      </div>
                    <div class="form-group">
                        <label for="<%=TeamDescription.ClientID %>">Presentation:</label>
                        <asp:TextBox ID="TeamDescription" runat="server" MaxLength="1023" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>    
                    </div>
                    <div class="form-group">
                        <asp:Button ID="btnDeleteTeam" runat="server" CssClass="btn btn-warning"
                            Text="Ta bort laget" ToolTip="Ta bort detta laget helt och hållet"
                            OnClick="btnDeleteTeam_Click" Visible="false" />
                        <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Spara"
                            ToolTip="Spara ändringarna i laget" OnClick="btnSave_Click" />    
                    </div>
                </form>
                
            </div>

            <div class="col-md-4">
                <h5>Uppställning:</h5>
                <asp:Label ID="lblLineUpInfo" runat="server"></asp:Label>
                <asp:Label ID="lblLineUpAll" runat="server"></asp:Label>
                <asp:PlaceHolder ID="plhLineUpControls" runat="server"></asp:PlaceHolder>
            </div>

        </asp:Panel>

        <asp:Panel ID="pnlTeamTransfers" runat="server" Visible="false">
            <div class="col-md-4">
                <h5>Byten:</h5>
                <asp:Label ID="lblTransferInfo" runat="server"></asp:Label>
                <asp:PlaceHolder ID="plhTransferLineUp" runat="server"></asp:PlaceHolder>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnCommitTransfers" runat="server" Text="Genomför byten"
                                ToolTip="Klicka här för att genomföra bytena enligt ovan"
                                CssClass="btn btn-danger" OnClick="btnCommitTransfers_Click" />
                            <asp:Button ID="btnUndoTransfers" runat="server" Text="Ångra bytena"
                                ToolTip="Klicka här för att börja om från början med bytena"
                                CssClass="btn" OnClick="btnUndoTransfers_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlSavedLineUp" runat="server">
            <div class="col-md-4">
                <h5>Nuvarande uppställning:</h5>
                <p>
                    <asp:Label ID="lblLineUp" runat="server"></asp:Label>
                </p>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlFutureLineUp" runat="server">
            <div class="col-md-4">
                <h5>Kommande uppställning:</h5>
                <asp:Label ID="lblNextLineUpValid" runat="server"></asp:Label>
                <p>
                    <asp:Label ID="lblNextLineUp" runat="server"></asp:Label>
                </p>
            </div>
        </asp:Panel>
            
        <asp:Panel ID="pnlSuperAdmin" runat="server" Visible="false">
            <div class="col-md-4">
                <div>
                    <h4>Versioner</h4>
                    <asp:Label runat="server" ID="lblTeamVersions"></asp:Label>
                </div>
                
                <div>
                    <asp:TextBox runat="server" ID="txtVersionToRemove" CssClass="form-control"></asp:TextBox>
                </div>
                <div><asp:Button ID="btnCleanTeam" runat="server" CssClass="btn btn-warning"
                            Text="Kör Clean" ToolTip="Rensa kommande versioner"
                            OnClick="btnCleanTeam_Click" />    
                    <asp:Button runat="server" ID="btnRemoveVersion" CssClass="btn btn-danger" Text="Ta bort vald version" OnClick="btnRemoveVersion_OnClick"/>
                    </div>
                
            </div>
            

            </asp:Panel>
    </div>
    







    <div>
        <asp:Button ID="btnReOpenTransferTeam" runat="server" CssClass="btn"
            Text="Ångra genomförande av byten" ToolTip="Öppna laget för byten igen."
            Visible="false" OnClick="btnReOpenTransferTeam_Click" />
    </div>
    <div class="hidden-phone">
        <h6>Ändringslogg</h6>
        <inti:ViewChangeLog ID="teamChangeLog" runat="server" />
    </div>
    
    <script type="text/javascript">
        
        function DeletePlayer(athleteId, teamVersionId) {
            
            if (event) {
                event.preventDefault();
            }

            $.ajax({
                    type: "POST",
                    url: "UserTeamEdit.aspx/DeletePlayer",
                    data: '{athleteGuid: "' + athleteId + '", teamVersionId: "' + teamVersionId +'" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function() {
                        location.reload();
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                    });
        }

    </script>
</asp:Content>
