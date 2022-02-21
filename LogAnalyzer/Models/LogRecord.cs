using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models
{
    public class LogRecord
    {
        public int ID { get; set; }
        public string SourceIpAddress { get; set; } = String.Empty;
        public string LastUpdated { get; set; } = String.Empty;
        public string HttpAction { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int HttpResponse { get; set; }
        public int ContentSize { get; set; }
        public string Info { get; set; } = string.Empty;
    }
}
