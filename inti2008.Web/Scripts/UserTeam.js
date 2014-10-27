
//viewmodel for user team
var userTeamViewModel = {
        Id: null,
        Name: ko.observable(),
        Description: ko.observable()
};


function LoadUserTeam(teamId) {
    var divUserTeam = $("#userTeam");
    if (!divUserTeam) return;

    var sHtml = "<span id='teamName' data-bind='text: \"Name\"' />"
    sHtml += "<span id='teamDescription' data-bind='text: \"Description\"' />"

    $("#userTeam").html(sHtml);

    $.getJSON("/api/inti/GetUserTeam.ashx?UserTeamId=" + teamId, function (data) {
        userTeamViewModel.Id = data.Id;
        userTeamViewModel.Name(data.Name);
        userTeamViewModel.Description(data.Description);

        ko.applyBindings(userTeamViewModel, $("#userTeam")[0]);
    });


}

