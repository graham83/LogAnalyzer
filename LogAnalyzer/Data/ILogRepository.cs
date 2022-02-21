using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models;

namespace LogAnalyzer.Data
{
    public interface ILogRepository : IDisposable
    {
        IEnumerable<LogRecord> GetLogRecords();
        int GetNumberUniqueIpAddresses();
        List<string> MostVisitedUrls(int number);
        List<string> MostActiveIpAddresses(int number);
        void InsertLogRecord(LogRecord record);
        void Save();


    }
}
