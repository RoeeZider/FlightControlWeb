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
        public ConcurrentDictionary<string, string> Get()
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
        public void Post([FromBody] Server s)
        {
            flightManager.AddServer(s);
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
        public void Delete(string id)
        {
            flightManager.DeleteServer(id);
        }
    }
}
