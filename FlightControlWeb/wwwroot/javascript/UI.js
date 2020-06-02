let markersColor = [];
let markers_on_map = [];
let map
let chosen_marker;
let chosen_flight;
let flightPlanCoordinates = [];
let flightPath
let flightPathId

function initMap() {
    var uluru = { lat: 30.00, lng: 30.00 };
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 4,
        center: uluru
    });
}

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
           
            marker = new google.maps.Marker({
                map: map,
                position: { lat: flight.latitude, lng: flight.longitude },
                id: flight.flight_id,
                icon: {
                    url: "https://www.google.com/mapfiles/marker_green.png"
                }
            });
            google.maps.event.addListener(marker, 'click', function () {
                showFlight(flight.flight_id);
                $('tr').removeClass('highlight');
                $("#" + flight.flight_id).addClass('highlight');
                 
                for (var i = 0; i < markers_on_map.length; i++) {
                    markers_on_map[i].setIcon("https://www.google.com/mapfiles/marker_green.png");
                }
                this.setIcon("http://maps.google.com/mapfiles/ms/icons/red-dot.png");
            });
            markers_on_map.push(marker);

        }
        $("#x"+flight.flight_id).click(function () {
            $("#"+flight.flight_id).remove();
            deleteFlight(flight.flight_id);
            //delete from markers array
            let i = markers_on_map.findIndex(x => x.get("id") === flight.flight_id);
            markers_on_map[i].setMap(null);
            markers_on_map.splice(i, 1);
            showMarkers(map);
            //delete polyline
            if (flightPathId == flight.flight_id) {
                flightPath.setMap(null);
                flightPlanCoordinates = [];
            }
            clearDetails();



        });
        $('tr').click(function () {
            showFlight(flight.flight_id);
            $('tr').removeClass('highlightred');
            $(this).addClass('highlightred');
            for (var i = 0; i < markers_on_map.length; i++) {
                markers_on_map[i].setIcon("https://www.google.com/mapfiles/marker_green.png");
            }
            c_marker = markers_on_map.find(x => x.get("id") === flight.flight_id);
            let u = c_marker.get("id");
            c_marker.setIcon("http://maps.google.com/mapfiles/ms/icons/red-dot.png")
            c_marker = null;
            showMarkers(map);

        });

        // var myLatLng = new google.maps.Map.LatLng(flight.latitude, flight.longitude);

    };
    if ((flightPathId != null) && (!$("#" + flightPathId).length)) {
        flightPlanCoordinates = [];
        flightPath.setMap(null);
    }

}
function showMarkers(map) {
    //var bounds = new google.maps.LatLngBounds();
    if (markers_on_map) {
        for (var i = 0; i < markerArray.length; i++) {
            bounds.extend(markerArray[i].getPosition())
            markerArray[i].setMap(map);
        };
    }
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



}
function deleteFlight(flightId) {
    //  $("#flight_list") tr#$(flightId)').remove();
    delete_flight(flightId);
}