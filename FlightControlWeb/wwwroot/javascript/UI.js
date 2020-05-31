function updateFlights(flights) {
    let table = document.getElementById('flight_list');
    for (const flight of flights) {
        if (flight == null) {
            continue;
        }
        if (table.rows[flight.flight_id]) {
            continue;
        } else {
          
            let newRow = $("<tr id=" + flight.flight_id + ">");
            let cols = "";
            cols += '<td informative>' + flight.flight_id + '</td>';
            cols += '<td informative>' + flight.companyName + '</td>';
            if (!flight.is_external) {
                cols += '<td> <input type="button" class="ibtnDel btn btn-md btn-danger " value="X" id="' + "x" + flight.flight_id + '"></td >';

            }
  
            newRow.append(cols);
            $('#flight_list > tbody:last-child').append(newRow);
         
        }
        $("#x"+flight.flight_id).click(function () {
            $("#"+flight.flight_id).remove();
            deleteFlight(flight.flight_id);
        });
        $('tr').click(function () {
            showFlight(flight.flight_id);
            $('tr').removeClass('highlight');
            $(this).addClass('highlight');

        });
    };
}
function showFlight(idFlight) {
    const url = "/api/FlightPlan/" + idFlight;

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            success: function (flightPlan) {
                const c = flightPlan;
                const start_loc = flightPlan.initial_location;
                const end_loc = flightPlan.segments[flightPlan.segments.length - 1];
                const start_time = new Date(start_loc.date_time);
                const end_time = new Date(start_time);
                for (const segment of flightPlan.segments) {
                    end_time.setSeconds(end_time.getSeconds() + segment.timespan_seconds);
                }
                $("#flight_info_id").html(flightPlan.id);
                $("#details_takeoff").html(start_loc.latitude + ", " + start_loc.longitude);
                $("#details_landing").html(end_loc.latitude + ", " + end_loc.longitude);
                $("#details_start_point").html(start_time.toISOString().replace(".000Z",""));
                $("#details_end_point").html(end_time.toISOString().replace(".000Z",""));
                $("#details_passengers").html(flightPlan.passengers);
                $("#details_company").html(flightPlan.company_name);
            },
        });
    
    // when click on row of flight - change color.


    // get plane from map and change its color.



    // TODO: show flight path
}
function deleteFlight(flightId) {
  //  $("#flight_list") tr#$(flightId)').remove();
    delete_flight(flightId);
}