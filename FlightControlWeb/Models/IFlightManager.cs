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
        bool DeleteFlight(string id);

        public bool AddFlighttPlan(FlightPlan f);
        Task<List<Flight>> GetAllFlights(string dateTime);
        Task<FlightPlan> GetFlightPlanByServer(string id);

        List<Flight> GetInternalFlights(string dateTime);
        ConcurrentDictionary<string, FlightPlan> GetDic();
        public FlightPlan GetFlightPlanById(string id);
        
        bool AddServer(Server s);
        List<Server> GetServers();
        bool DeleteServer(string id);

        string GetID();
    }
}
