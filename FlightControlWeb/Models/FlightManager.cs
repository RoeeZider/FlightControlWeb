using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlightControlWeb.Models
{
    public class FlightManager : IFlightManager
    {
        private static List<Flight> flights = new List<Flight>()
        {
            new Flight { Flight_id="1", Longitude=27.000, Latitude=30.000, CompanyName="ZidAir",Is_external=true,Passengers=200,Date_time="18:54"},
            new Flight{ Flight_id="2", Longitude=125.000, Latitude=10.000, CompanyName="HarelAir",Is_external=true,Passengers=400,Date_time="20:54"},
        };

        public static ConcurrentDictionary<string, FlightPlan> flightPlan = new ConcurrentDictionary<string, FlightPlan>() {};
        public static ConcurrentDictionary<string, string> servers = new ConcurrentDictionary<string, string>()
            ;

        public void AddFlightt(Flight f)
        {
            flights.Add(f);
        }

        public bool DeleteFlight(string id)
        {
            FlightPlan f = flightPlan[id];
            if (f == null)
                return false; ;
            flightPlan.TryRemove(id,out f);
            return true;
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            return flights;
        }


        public ConcurrentDictionary<string, FlightPlan>  GetDic()
        {
            return flightPlan;
        }

        public List<Server> GetServers()
        {
            List<Server> serversList = new List<Server>();
            foreach(KeyValuePair<string, string> s in servers)
            {
                Server _server = new Server(s.Key,s.Value);
                serversList.Add(_server);
            }
            return serversList;
        }

        public bool DeleteServer(string id)
        {
            string s = servers[id];
            if (s == null)
                return false;
            servers.TryRemove(id, out s);
            return true;
        }



        public bool AddFlighttPlan(FlightPlan f)
        {

            //cheak valid

            if(f.Company_name == null )
            {
                return false;
            }
            //|| f.Passengers.ToString() == null
            //and for any location / segments
            if (f.Segments==null || f.InitialLocation == null )
            {
                return false;
            }

            string id = Get_flight_id(f.Company_name);
            flightPlan[id] = f;
            return true;
        }

        public bool AddServer(Server s)
        {
            if (s.ServerId == null || s.ServerURL == null) return false;
            servers[s.ServerId] = s.ServerURL;
            return true;
        }

        /*
        public Flight GetFlightById(int id)
        {
            Flight f = flights.Where(x => x.Flight_id == id).FirstOrDefault();
            return f;
        }
        */

        //necessary?
        public void UpdateFlight(Flight f)
        {
            Flight newFlight = flights.Where(x => x.Flight_id == f.Flight_id).FirstOrDefault();
            newFlight.CompanyName = f.CompanyName;
           //and the rest

        }
        private string Get_flight_id(string name)
        {
            string id = "";
            Random rnd = new Random();
            if (name.Length >=3)
            {
                id = name.Substring(0, 3);
            }
            else
            {
                id = name.Substring(0);
                while(id.Length <3)
                {
                    id = id + GetLetter();
                }
            }

            while(id.Length<6)
            {
                id = id + rnd.Next(1, 9);
            }

            return id;
            
        }
        public static char GetLetter()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length - 1);
            return chars[num];
        }

        FlightPlan IFlightManager.GetFlightPlanById(string id)
        {
            FlightPlan fp;
            flightPlan.TryGetValue(id, out fp);
            return fp;
        }

        public async Task<FlightPlan> GetFlightPlanByServer(string id)
        {
            HttpClient client = new HttpClient();
            foreach (KeyValuePair<string, string> s  in servers)
            {
                HttpResponseMessage respone = await client.GetAsync(s.Value + "/api/FlightPlan/" + id);
                if (respone.IsSuccessStatusCode)
                {
                    var content = respone.Content;
                    string data = await content.ReadAsStringAsync();
                    FlightPlan fp = JsonConvert.DeserializeObject<FlightPlan>(data);
                    return fp;
                }
            }

            return null;

        }



        public async Task<List<Flight>>  GetAllFlights(string dateTime)
        {
            List<Flight> flights = new List<Flight>();
            DateTime time = ConvertToDateTime(dateTime);
            
            foreach(KeyValuePair<string,string> s in servers)
            {
                List<Flight> temp =  await GetExternalFlight(s.Value, dateTime);
                if(temp.Count != 0) flights.AddRange(temp);
            }

            return flights;




           
        }

        public async Task<List<Flight>> GetExternalFlight(string URL,string dateTime)
        {
            List<Flight> exteranlFlight = new List<Flight>();
            List<Flight> temp = new List<Flight>();
            string url = URL + "/api/Flights?relative_to=" + dateTime.ToString();
            HttpClient client = new HttpClient();
           
                var respone = await client.GetStringAsync(url);
          
            temp = JsonConvert.DeserializeObject<List<Flight>>(respone.ToString());
            foreach(Flight f in temp)
            {
                exteranlFlight.Add(f);
                f.Is_external = true;
            }
            return  exteranlFlight;
        }

         List<Flight> IFlightManager.GetInternalFlights(string dateTime)
        {
            List<Flight> flights = new List<Flight>();
            DateTime time = ConvertToDateTime(dateTime);
            List<Segment> flightSegments;

            DateTime flightTime;

            foreach(KeyValuePair<string,FlightPlan> fp in flightPlan)
            {
                flightSegments = fp.Value.Segments;
                flightTime = ConvertToDateTime(fp.Value.InitialLocation.Date_time);
                if (flightTime > time) continue;

                int num = flightSegments.Count;
                /*
                while(num>0)
                {
                    flightTime.AddSeconds(flightSegments[num].Timespan_seconds);
                    num--;
                }
                */


                //

                Flight f = new Flight();
                if (CheckSegments(flightSegments, f, flightTime, time,fp.Value.InitialLocation.Latitude, fp.Value.InitialLocation.Longitude))
                {
                    flights.Add(f);
                }
                
                //create the flight
                f.Flight_id = fp.Key;
                f.Passengers = fp.Value.Passengers;
                f.CompanyName = fp.Value.Company_name;
                f.Date_time = dateTime;
                f.Is_external = false;
            }




            return flights;
        }
        private bool CheckSegments(List<Segment> segments, Flight f, DateTime start, DateTime relative,double lat,double lon)
        {
            bool isRelevant = false;
            double startLat = lat;
            double startLong = lon; ;
            // Run over the segments.
            foreach (Segment s in segments)
            {
                DateTime saveStart = start;
                DateTime test = start.AddSeconds(s.Timespan_seconds);
                // The plan is in this segment at the time is given.
                if (DateTime.Compare(relative, start) >= 0 &&
                    DateTime.Compare(relative, test) <= 0)
                {
                    TimeSpan difference = relative - saveStart;
                    double k = difference.Seconds;
                    double l = s.Timespan_seconds - k;
                    f.Latitude = (startLat * l + s.Latitude * k) / (l + k);
                    f.Longitude = (startLong * l + s.Longitude * k) / (l + k);
                    isRelevant = true;
                    break;
                }
                else
                {
                    // Save the start location of the segment.
                    startLat = s.Latitude;
                    startLong = s.Longitude;
                }
            }
            return isRelevant;
        }

        public static DateTime ConvertToDateTime(string str)
        {
            string pattern = @"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})Z";
            if (Regex.IsMatch(str, pattern))
            {
                Match match = Regex.Match(str, pattern);
                int second = Convert.ToInt32(match.Groups[6].Value);
                int minute = Convert.ToInt32(match.Groups[5].Value);
                int hour = Convert.ToInt32(match.Groups[4].Value);
                int day = Convert.ToInt32(match.Groups[3].Value);
                int month = Convert.ToInt32(match.Groups[2].Value);
                int year = Convert.ToInt32(match.Groups[1].Value);
                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                throw new Exception("Unable to parse");
            }
        }

        public string GetID()
        {
            string id = "";
            foreach (KeyValuePair<string, FlightPlan> fp in flightPlan)
            {
                id= fp.Key;
            }
            return id;
        }
    }
    
}
