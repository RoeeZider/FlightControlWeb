using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        IEnumerable<Flight> GetAllFlights();
       // Flight GetFlightById(int id);
        void AddFlightt(Flight p);
        void UpdateFlight(Flight p);
        void DeleteFlight(string id);

        public void AddFlightt2(FlightPlan f);
        List<Flight> GetAllFlights(string dateTime);
        List<Flight> GetInternalFlights(string dateTime);
        ConcurrentDictionary<string, FlightPlan> GetDic();
        FlightPlan GetFlightPlanById(string id);
        void AddServer(Server s);
        ConcurrentDictionary<string, string> GetServers();
        void DeleteServer(string id);
    }
}
