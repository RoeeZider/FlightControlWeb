using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("company_name")]
        public string Company_name { get; set; }
        [JsonPropertyName("initial_location")]
        public InitialLocation InitialLocation { get; set; }
        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set;}

        public FlightPlan(int passengers, string companyName, InitialLocation location, List<Segment> segments)
        {
           
            Passengers = passengers;
            Company_name = companyName;
            InitialLocation = location;
            Segments = segments;
        }

    }
}
