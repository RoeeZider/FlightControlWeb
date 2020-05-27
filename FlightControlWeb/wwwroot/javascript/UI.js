function updateFlights(flights) {
    flights.forEach(function (flight, i) {
        // Get flight id.
        let idFlight = flight.flight_Id;
        if (!flight.is_external) {
            $('#flight_list').append('<tr onClick="showFlight(id)" id="' + idFlight + '"><td>' + idFlight + '</td><td> '
                + flight.company_Name + '</td><td id="deleteButton"><button type="button" class="btn" value="Delete" id="' + "deleteRow" + count +
                '"><span class="fa fa-close"></span><span class="submit - text"> Delete</span></button></td></tr>');
        }
        let x = document.getElementById("deleteRow" + count);
        x.addEventListener("click", function () {
            deleteRow(idFlight, x);
        });
    });
}
function showFlight(idFlight) {
    // when click on row of flight - change color.


    // get plane from map and change its color.



    // TODO: show flight path
}