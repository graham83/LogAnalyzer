using LogAnalyzer.Data;
using LogAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerTests
{
    public static class DbInitialize
    {
        public static void Seed(LogDbContext context)
        {
            context.AddRange(
              new LogRecord() { SourceIpAddress = "177.71.128.21", HttpAction = "GET", Url = "/intranet-analytics/" },
               new LogRecord() { SourceIpAddress = "168.41.191.9", HttpAction = "GET", Url = "/intranet-analytics/" },
               new LogRecord() { SourceIpAddress = "72.44.32.11", HttpAction = "GET", Url = "/intranet-analytics/" },
               new LogRecord() { SourceIpAddress = "177.71.128.21", HttpAction = "GET", Url = "/blog/2018/08/survey-your-opinion-matters/" },
               new LogRecord() { SourceIpAddress = "168.41.191.9", HttpAction = "GET", Url = "/blog/2018/08/survey-your-opinion-matters/" },
               new LogRecord() { SourceIpAddress = "72.44.32.11", HttpAction = "GET", Url = "/faq/how-to-install/" },
               new LogRecord() { SourceIpAddress = "177.71.128.21", HttpAction = "GET", Url = "/faq/how-to-install/" },
               new LogRecord() { SourceIpAddress = "168.41.191.9", HttpAction = "GET", Url = "/intranet-analytics/" },
               new LogRecord() { SourceIpAddress = "72.44.32.11", HttpAction = "GET", Url = "/moved-permanently" },
               new LogRecord() { SourceIpAddress = "72.44.32.10", HttpAction = "GET", Url = "/moved-temporarily" },
               new LogRecord() { SourceIpAddress = "177.71.128.21", HttpAction = "GET", Url = "https://intranet/" },
               new LogRecord() { SourceIpAddress = "177.71.128.21", HttpAction = "GET", Url = "/faq/how-to-install/" },
               new LogRecord() { SourceIpAddress = "72.44.32.11", HttpAction = "GET", Url = "/home" });

            context.SaveChanges();
        }
    }
}
