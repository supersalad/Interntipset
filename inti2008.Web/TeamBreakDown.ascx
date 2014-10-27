<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamBreakDown.ascx.cs" Inherits="inti2008.Web.TeamBreakDown" %>

<h4><asp:Label runat="server" ID="lblHeader"></asp:Label> </h4>
<div id="teamPreview" class="list-group"></div>


<script id="teamTemplate" type="text/x-jsrender">
            <a href="/Team/{{>TeamGuid}}" class="list-group-item {{>CustomClass}}">{{>TeamName}} - {{>Manager}}</a>
            {{for Athletes tmpl="subAthleteTemplate"/}}

    </script>
    
    <script id="subAthleteTemplate" type="text/x-jsrender">
        <a href="/Player/{{>AthleteClubGuid}}" class="list-group-item"><small>{{>Name}}</small></a>
    </script>

    <script src="/js/jsrender.min.js" type="text/javascript"></script>    
    <script type="text/javascript">
        var data = <%=PreviewData%>;

        $(document).ready(function() {
            //do not run if no preview
            if (!data) return;

            //init templates
            $.templates("teamTemplate", {
                markup: "#teamTemplate",
                templates: {
                    subAthleteTemplate: "#subAthleteTemplate"
                }
            });

            $("#teamPreview").html($.render.teamTemplate(data.Teams));
            
        });


    </script>
