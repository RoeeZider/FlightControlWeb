using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {

        IFlightManager flightManager;

        public FlightPlanController(IFlightManager flightManager)
        {
            this.flightManager = new FlightManager();
        }

        // GET: api/FlightPlan
        [HttpGet]
        public ConcurrentDictionary<string, FlightPlan> Get()
        {
            return flightManager.GetDic();
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(string id)
        {
            return flightManager.GetFlightPlanById(id);
        }

        // POST: api/FlightPlan
        [HttpPost]
        public void addFlightPlan([FromBody] FlightPlan f)
        {
            flightManager.AddFlightt2(f);
        }

        // PUT: api/FlightPlan/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
