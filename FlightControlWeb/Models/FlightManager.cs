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

namespace FlightControlWeb.Models
{
    public class FlightManager : IFlightManager
    {
        private static List<Flight> flights = new List<Flight>()
        {
            new Flight { Flight_id="1", Longitude=27.000, Latitude=30.000, CompanyName="ZidAir",Is_external=true,Passengers=200,Date_time="18:54"},
            new Flight{ Flight_id="2", Longitude=125.000, Latitude=10.000, CompanyName="HarelAir",Is_external=true,Passengers=400,Date_time="20:54"},
        };

        public static ConcurrentDictionary<string, FlightPlan> flightPlan = new ConcurrentDictionary<string, FlightPlan>();
        public static ConcurrentDictionary<string, string> servers = new ConcurrentDictionary<string, string>();

        public void AddFlightt(Flight f)
        {
            flights.Add(f);
        }

        public void DeleteFlight(string id)
        {
            FlightPlan f = flightPlan[id];
            if (f == null)
                throw new Exception("flight not found");
            flightPlan.TryRemove(id,out f);
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            return flights;
        }


        public ConcurrentDictionary<string, FlightPlan>  GetDic()
        {
            return flightPlan;
        }

        public ConcurrentDictionary<string, string> GetServers()
        {
            return servers;
        }

        public void DeleteServer(string id)
        {
            string s = servers[id];
            if (s == null)
                throw new Exception("flight not found");
            servers.TryRemove(id, out s);
        }



        public void AddFlightt2(FlightPlan f)
        {
            string id = Get_flight_id(f.Company_name);
            flightPlan[id] = f;
        }

        public void AddServer(Server s)
        {
            servers[s.ServerId] = s.ServerURL;
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
            return flightPlan[id];
        }



        List<Flight> IFlightManager.GetAllFlights(string dateTime)
        {
            List<Flight> flights = new List<Flight>();
            DateTime time = ConvertToDateTime(dateTime);
            
            foreach(KeyValuePair<string,string> s in servers)
            {
                List<Flight> temp = GetExternalFlight(s.Value, dateTime);
                flights.AddRange(temp);
            }

            return flights;




           
        }

        List<Flight> GetExternalFlight(string URL,string dateTime)
        {
            List<Flight> exteranlFlight = new List<Flight>();
            string url = "https://" + URL + "/api/Flights?relative_to=" + dateTime;
            HttpClient client = new HttpClient();
            var respone = client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject<List<Flight>>(respone.ToString());
            foreach(Flight f in json)
            {
                exteranlFlight.Add(f);
            }
            return exteranlFlight;
        }

         List<Flight> IFlightManager.GetInternalFlights(string dateTime)
        {
            List<Flight> flights = new List<Flight>();
            DateTime time = ConvertToDateTime(dateTime);
            List<Segment> flightSegments;

            DateTime flightTime;

            foreach(KeyValuePair<string,FlightPlan> fp in flightPlan)
            {
                flightSegments = fp.Value.segments;
                flightTime = ConvertToDateTime(fp.Value.initialLocation.Date_time);
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
                if (CheckSegments(flightSegments, f, flightTime, time,fp.Value.initialLocation.Latitude, fp.Value.initialLocation.Longitude))
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
    }
    
}
