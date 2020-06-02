using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
/*
        public FlightPlanController()
        {
        }
        */
        // GET: api/FlightPlan
        [HttpGet]
        public ConcurrentDictionary<string, FlightPlan> Get()
        {
            return flightManager.GetDic();
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult< FlightPlan>> Get(string id)
        {
            FlightPlan fp= flightManager.GetFlightPlanById(id);
            if (fp != null) return Ok(fp);

            fp = await flightManager.GetFlightPlanByServer(id);
            if (fp != null) return Ok(fp);
            return NotFound(id);
        }


        //// POST: api/FlightPlan
        [HttpPost]
        public ActionResult addFlightPlan([FromBody] FlightPlan f)
        {
            bool ok=flightManager.AddFlighttPlan(f);
            if (!ok) return BadRequest("flightPlan isnt valid");
            return Ok(f);
        }
    }
}
