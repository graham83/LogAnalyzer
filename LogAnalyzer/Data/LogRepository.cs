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

        public List<(string Url, int Count)> MostVisitedUrlsAndCount(int number)
        {
            var topList = logDbContext.LogRecords
              .GroupBy(i => i.Url)
              .OrderByDescending(g => g.Count())
              .Take(number)
              .Select(g => ValueTuple.Create(g.Key,g.Count()));
              
            return topList.ToList();
        }

        public List<string> UrlsByCount(int number)
        {
            var topList = logDbContext.LogRecords
              .GroupBy(i => i.Url)
              .Where(g => g.Count() == number)
               .OrderBy(o => o.Key)
              .Select(g => g.Key);
         

            return topList.ToList();
        }

        public (string Url, int Count) UrlsByNextFrequencyOccurrence(int number)
        {
            var topList = logDbContext.LogRecords
              .GroupBy(i => i.Url)
              .OrderByDescending(g => g.Count())
              .Where(g => g.Count() < number)
              .Select(g => ValueTuple.Create(g.Key, g.Count()))
              .FirstOrDefault();

            //var nextValue = topList.Select(x => ValueTuple.Create(topList.Key, topList.Count())).FirstOrDefault();
            return topList;
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
