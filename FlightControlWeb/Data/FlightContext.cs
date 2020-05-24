using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Data
{
    public class FlightContext : DbContext
    {
        public FlightContext (DbContextOptions<FlightContext> options)
            : base(options)
        {
        }

        public DbSet<FlightControlWeb.Models.Flight> Flight { get; set; }
    }
}
