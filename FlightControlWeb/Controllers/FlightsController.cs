using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        IFlightManager flightManager;

        public FlightsController(IFlightManager flightManager)
        {
            this.flightManager = new FlightManager();
        }

        // DELETE: api/Flight/id
        [HttpDelete("{id}")]
        //[Url=api/Flight/id]
        public ActionResult Delete(string id)
        {

            //fix this?
            bool ok=flightManager.DeleteFlight(id);
            if (!ok) return NotFound(id);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<Flight>>> GetAllFlights()
        {
           
            List<Flight> flights =  new List<Flight>();

            List<Flight> temp = new List<Flight>();
            flights = flightManager.GetInternalFlights(Request.Query["relative_to"]);
            if (!Request.Query.ContainsKey("sync_all"))
            {
                return Ok( flights);
            }
            var external = await flightManager.GetAllFlights(Request.Query["relative_to"]);
            flights.AddRange(external);
            return Ok(flights);
        }

        /*
       // GET: api/Flights
       [HttpGet]
       public IEnumerable<Flight> GetAllFlights()
       {
           return flightManager.GetAllFlights();
       }


        // GET: api/Flights/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Flights
        [HttpPost]
        public Flight addFlight([FromBody] Flight f)
        {
            flightManager.AddFlightt(f);
            return f;
        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /*
       

         */
    }
}
