using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogAnalyzer.Models;

namespace LogAnalyzer.Data
{
    public class LogDbContext : DbContext
    {
        public DbSet<LogRecord> LogRecords { get; set; }

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {

        }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogRecord>().ToTable("LogRecords");
        }
    }
}
