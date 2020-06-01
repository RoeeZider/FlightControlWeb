/// <reference path="jquary.min.js" />
function postFlight(jsonString) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", 'api/FlightPlan', true);

    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            // Request finished. Do processing here.
        }
    }
    xhr.send(jsonString);
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
    }, 10000);
});


function getFlights() {
    let d = new Date(Date().toString('en-US', { timeZone: "Etc/GMT-0" }));
    let date = d.toISOString().replace(".000", "");
    let url = "/api/Flights?relative_to=" + date + "&sync_all" ;

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
