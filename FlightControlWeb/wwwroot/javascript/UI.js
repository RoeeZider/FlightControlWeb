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


function startTime() {
  var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    m = checkTime(m);
    s = checkTime(s);
    document.getElementById('txt').innerHTML =h + ":" + m + ":" + s;
    var t = setTimeout(startTime, 500);
}

function checkTime(i) {
  if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
}

    
function reset() {
    if (markers_on_map.length > 0) {
        c_marker = markers_on_map.find(x => x.get("id") === chosen_marker);
        c_marker.setIcon("https://www.google.com/mapfiles/marker_green.png");
        c_marker.setMap(map);
        $('tr').removeClass('highlightred');
        chosen_marker = null;
        chosen_flight = null;

        clearDetails();
        flightPlanCoordinates = [];
        flightPath.setMap(null);
    }
}





function updateFlights(flights) {
  //  restart every x sec
    $("#tblBody").empty();
    showMarkers(null);
    markers_on_map = [];
  
    // add to flight list
    let table = document.getElementById('flight_list');
    for (const flight of flights) {
        if (flight == null) {
            continue;
        }
       // if (table.rows[flight.flight_id]) {
         //   continue;
        //} else {
          
        let newRow = $("<tr id=" + flight.flight_id + ">");
        let cols = "";
        cols += '<th informative scope="row">' + flight.flight_id + '</th>';
        cols += '<td informative>' + flight.companyName + '</td>';
        if (!flight.is_external) {
            cols += '<td> <input type="button" class="ibtnDel btn btn-md btn-danger " value="X" id="' + "x" + flight.flight_id + '"></td >';
        }
        cols += '</tr >';
        
        newRow.append(cols);
        $('#flight_list > tbody:last-child').append(newRow);
        //make it highlight if it the chosen one.
        if (flight.flight_id == chosen_flight)
            $("#" + flight.flight_id).addClass('highlightred');

        // add marker
        marker = new google.maps.Marker({
           map: map,
           position: { lat: flight.latitude, lng: flight.longitude },
           id: flight.flight_id,
           icon: {
              url: "https://www.google.com/mapfiles/marker_green.png"
           }
        });
        //if it is the chosen marker change color
        if (marker.get("id") == chosen_marker)
            marker.setIcon("http://maps.google.com/mapfiles/ms/icons/red-dot.png");
        //add event to marker
        google.maps.event.addListener(marker, 'click', function () {
            event.stopPropagation();
            chosen_marker = flight.flight_id;
            chosen_flight = flight.flight_id;
            showFlight(flight.flight_id);
            $('tr').removeClass('highlightred');
            $("#" + flight.flight_id).addClass('highlightred');
            for (var i = 0; i < markers_on_map.length; i++) {
                 if (markers_on_map[i] != chosen_marker)
                       markers_on_map[i].setIcon("https://www.google.com/mapfiles/marker_green.png");
            }
            this.setIcon("http://maps.google.com/mapfiles/ms/icons/red-dot.png");
        });
        markers_on_map.push(marker);

     //   }
        //add event to delete button
        $("#x" + flight.flight_id).click(function (e) {
            e.stopPropagation();
            //remove row
            $("#" + flight.flight_id).remove();
            //send to server
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
        //add event to row click
        $("#" + flight.flight_id).click(function (e) {
            e.stopPropagation();
            chosen_flight = flight.flight_id;
            chosen_marker = flight.flight_id;
      
            showFlight(flight.flight_id);
            $('tr').removeClass('highlightred');
            $(this).addClass('highlightred');
            for (var i = 0; i < markers_on_map.length; i++) {
                markers_on_map[i].setIcon("https://www.google.com/mapfiles/marker_green.png");
            }
            c_marker = markers_on_map.find(x => x.get("id") === flight.flight_id);
            let u = c_marker.get("id");
            c_marker.setIcon("http://maps.google.com/mapfiles/ms/icons/red-dot.png")
     
            showMarkers(map);
         
        });
      
       // var myLatLng = new google.maps.Map.LatLng(flight.latitude, flight.longitude);
     
    };

}
function showMarkers(map) {
    //var bounds = new google.maps.LatLngBounds();
    if (markers_on_map) {
        for (var i = 0; i < markers_on_map.length; i++) {
           // bounds.extend(markerArray[i].getPosition())
            markers_on_map[i].setMap(map);
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
                polyline(flightPlan.segments, start_loc.latitude, start_loc.longitude, idFlight);

            },
        });
    

}
function deleteFlight(flightId) {
  //  $("#flight_list") tr#$(flightId)').remove();
    delete_flight(flightId);
}

function clearDetails() {
    $("#flight_info_id").empty();
    $("#details_takeoff").empty();
    $("#details_landing").empty();
    $("#details_start_point").empty();
    $("#details_end_point").empty();
    $("#details_passengers").empty();
    $("#details_company").empty();
}

function polyline(segments,s_lat,s_lng,id) {
    if (flightPath != null)
        flightPath.setMap(null);
    flightPlanCoordinates = [];
    var s_coords = { lat: s_lat, lng: s_lng };
    flightPlanCoordinates.push(s_coords);
    for (let i = 0; i < segments.length;i++) {
        let y = segments[i];
        var coords = { lat: segments[i].latitude, lng: segments[i].longitude };
        flightPlanCoordinates.push(coords);
    }
    flightPath = new google.maps.Polyline({
        path: flightPlanCoordinates,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2
    });
    flightPathId = id;
    flightPath.setMap(map);
}