using Microsoft.EntityFrameworkCore;
using ShedulerServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShedulerServices.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
         : base(options)
        {
        }

        public DbSet<TestSchedules> TestSchedules { get; set; }
      
    }
}

