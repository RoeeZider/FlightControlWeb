using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Server
    {
        [Key]
        [JsonPropertyName("ServerId")]
        public string ServerId { get; set; }

        [JsonPropertyName("ServerURL")]
        public string ServerURL { get; set; }
    }
}
