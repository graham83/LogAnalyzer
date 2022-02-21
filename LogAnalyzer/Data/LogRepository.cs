using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models;

namespace LogAnalyzer.Data
{
    public class LogRepository : ILogRepository, IDisposable
    {
        private LogDbContext logDbContext;

        public LogRepository(LogDbContext context)
        {
            logDbContext = context;
        }

        public IEnumerable<LogRecord> GetLogRecords()
        {
            return logDbContext.LogRecords.ToList();
        }

        public void InsertLogRecord(LogRecord record)
        {
            logDbContext.LogRecords.Add(record);
        }

        public void Save()
        {
            logDbContext.SaveChanges();
        }

        public int GetNumberUniqueIpAddresses()
        {
            return logDbContext.LogRecords.Select(r => r.SourceIpAddress).Distinct().Count();
        }

        public List<string> MostVisitedUrls(int number)
        {
            var topList = logDbContext.LogRecords
              .GroupBy(i => i.Url)
              .OrderByDescending(g => g.Count())
              .Take(number)
              .Select(g => g.Key);

            return topList.ToList();
        }

        public List<string> MostActiveIpAddresses(int number)
        {

            var topList  = logDbContext.LogRecords
            .GroupBy(i => i.SourceIpAddress)
            .OrderByDescending(g => g.Count())
            .Take(number)
            .Select(g => g.Key);

            return topList.ToList();
           
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    logDbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
