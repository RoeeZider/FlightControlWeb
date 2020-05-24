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
        public void Delete(string id)
        {
            flightManager.DeleteFlight(id);
        }

        [HttpGet]
        public IEnumerable<Flight> GetAllFlights()
        {
            List<Flight> flights = new List<Flight>();
            List<Flight> temp = new List<Flight>();
            flights = flightManager.GetInternalFlights(Request.Query["relative_to"]);
            if (!Request.Query.ContainsKey("sync_all"))
            {
                return flights;
            }
            temp= flightManager.GetAllFlights(Request.Query["relative_to"]);
            foreach(Flight f in temp)
            {
                flights.Add(f);
            }
            return flights;
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
