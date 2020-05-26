using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using System.Collections.Concurrent;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class serversController : ControllerBase
    {
        IFlightManager flightManager;

        public serversController(IFlightManager flightManager)
        {
            this.flightManager = new FlightManager();
        }


        // GET: api/servers
        [HttpGet]
        public List<Server> Get()
        {
            return flightManager.GetServers();
        }
        /*
        // GET: api/servers/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST: api/servers
        [HttpPost]
        public ActionResult Post([FromBody] Server s)
        {
            bool ok=flightManager.AddServer(s);
            if (!ok) return BadRequest("server isnt valid");
            return Ok(s);
        }
        /*
        // PUT: api/servers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        */

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {

            bool ok = flightManager.DeleteServer(id);
            if (!ok) return NotFound(id);
            return Ok(id);
        }
    }
}
