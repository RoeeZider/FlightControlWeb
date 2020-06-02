/// <reference path="jquary.min.js" />
function postFlight(flightPlan) {
    try {
        JSON.parse(flightPlan);
    } catch (e) {
        alert("the text is not in a json format");
    }

    const url = "/api/FlightPlan";

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: flightPlan,
        success: function (data) {
            alert("file uploaded successfuly");
        },
        error: function (xhr) { alert("Request Error!\nURL: " + url + "\nError: " + xhr.status + " - " + xhr.title); },
    });
}

function delete_flight(id) {
    const url = "/api/Flights/" + id;
    $.ajax({
        type: "DELETE",
        url: url,
        success:
            function () {
                rowToRemove.remove();
                alert("flight " + flightId + " was deleted successfuly");

            },
        error: function (xhr) { alert("Request Error!\nURL: " + url + "\nError: " + xhr.status + " - " + xhr.title); },
    });
}


$(document).ready(function () {
    getFlights();
    setInterval(function () {
        getFlights();
    }, 1000);
});


function getFlights() {
    let d = new Date(Date().toString('en-US', { timeZone: "Etc/GMT-0" }));
    let date = d.toISOString().replace(".000", "");
    let url = "/api/Flights?relative_to=" +date ;

    $.ajax({
        type: "GET",
        url: url,
        datatype: 'json',
        success: updateFlights,
        error: function (xhr) { alert("Request Error!\nURL: " + url + "\nError: " + xhr.status + " - " + xhr.title); },
    });
}


function readFiles() {
    let file = document.getElementById("fileItem").files[0];
    let reader = new FileReader();

    reader.readAsText(file);

    var dataString;
    reader.onload = function () {
        dataString = reader.result.replace('/r', '');
        postFlight(dataString);
    };

    reader.onerror = function () {
        alert(reader.error);
    };
}