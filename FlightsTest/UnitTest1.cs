using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightsTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly FlightPlanController flightTest;
        //private readonly ConcurrentDictionary<string, FlightPlan> dic;
        private IFlightManager fm= new FlightManager();

        public UnitTest1()
        {
            flightTest = new FlightPlanController(fm);
          //  dic = new ConcurrentDictionary<string, FlightPlan>();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            InitialLocation location = new InitialLocation
            {
                Latitude = 33.244,
                Longitude = 31.12,
                Date_time= "2020-05-27T13:14:05Z"
            };


            List<Segment> segment = new List<Segment>
            {
                new Segment
            {
                Latitude = 12,
                Longitude = 14,
                Timespan_seconds = 65
            }
        };

            //expectaion - mock
            var fakeFlightPlane = new FlightPlan { 
                Company_name = "test1",
                Passengers = 200,
                InitialLocation = location,
                Segments = segment };
           
            
            fm.AddFlighttPlan(fakeFlightPlane);
            string id = fm.GetID();
             
            //async to the server
            var foo =  fm.GetFlightPlanById(id);
            Assert.IsTrue(fakeFlightPlane.Passengers == foo.Passengers);
            Assert.IsTrue(fakeFlightPlane.Company_name == foo.Company_name);

            //flights from controler
            var result = await flightTest.Get(id);
            //check that we get someting
            Assert.IsNotNull(result); 


            //exercise between fake flight and expected flight
            Task<ActionResult<FlightPlan>> fp1 = flightTest.Get(id);
            var a = fp1.Result;
            var b = (OkObjectResult)fp1.Result.Result;
            var flightAsync = (FlightPlan)b.Value;
            Assert.IsTrue(fakeFlightPlane.Passengers == flightAsync.Passengers);
            Assert.IsTrue(fakeFlightPlane.Company_name == flightAsync.Company_name);
            Assert.IsTrue(fakeFlightPlane.InitialLocation.Latitude ==
                flightAsync.InitialLocation.Latitude);

        }
    }
}
