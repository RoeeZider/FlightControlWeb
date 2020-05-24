using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        

        public int Passengers { get; set; }
        public string Company_name { get; set; }
        [JsonPropertyName("initial_location")]
        public InitialLocation initialLocation { get; set; }
        [JsonPropertyName("segments")]
        public List<Segment> segments { get; set;}

    }
}
