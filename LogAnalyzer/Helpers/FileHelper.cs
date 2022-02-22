using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Data;
using LogAnalyzer.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Helpers
{
    public class FileHelper
    {
        private readonly string[] HTTP_ACTIONS = new string[] { "GET", "POST", "PUT", "DELETE" };
        private readonly int MAX_NO_LOG_ITEMS = 9;
        private readonly int MIN_NO_LOG_ITEMS = 5;

        private ILogRepository _logRepository;
   

        public List<string> BadLines { get; set; } = new List<string>();

        public FileHelper(ILogRepository repository)
        {
            _logRepository = repository;
        }

        public int GetNumberUniqueUrls()
        {
            return _logRepository.GetNumberUniqueIpAddresses();
        }

        public List<string> GetTop3Visited()
        {
            return _logRepository.MostVisitedUrls(3);
        }

        public List<string> GetTop3ActiveIps()
        {
            return _logRepository.MostActiveIpAddresses(3);
        }

        public bool CheckFileExists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }

        public bool CheckFileLineFormatOk(string fileLine)
        {

            var lineValues = GetLineComponents(fileLine);

            if (!ReadLineSanityCheck(lineValues)) return false;

            LogRecord ? logRecord = ParseLineRaw(lineValues);

            if (logRecord != null)
            {
                return true;
            }
            
            return false;
        }

        // Returns number of records written to DB
        // Read one line at a time via stream incase its a large log file
        public int WriteLogFileToRepository(string path)
        {
            var recordsWritten = 0;

            if (!CheckFileExists(path)) return 0;

            using StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    var lineValues = GetLineComponents(line);
                    if (ReadLineSanityCheck(lineValues))
                    {
                        LogRecord? logRecord = ParseLineRaw(lineValues);

                        if (logRecord != null)
                        {
                            _logRepository.InsertLogRecord(logRecord);
                            recordsWritten++;
                            continue;
                        }
                    }
                    BadLines.Add(line);
                }
            }

            _logRepository.Save();

            return recordsWritten;
        }

        private static List<string> GetLineComponents(string line)
        {
            int endStringIndex = 0, startIndex = 0;

            List<string> components = new List<string>();

            while (endStringIndex != -1)
            {
           
                if (line[startIndex] == '\"')     // Capture quoted inline strings
                {
                    endStringIndex = line.IndexOf('\"', startIndex + 1) + 1;
                }
                else if (line[startIndex] == '[') // Capture datetime included in brackets
                {
                    endStringIndex = line.IndexOf(']', startIndex + 1) + 1;
                }
                else  // Otherwise get all fields between the spaces
                {
                    endStringIndex = line.IndexOf(' ', startIndex + 1) == -1 ? line.Length - 1 :
                        line.IndexOf(' ', startIndex + 1);
                }

                components.Add(line.Substring(startIndex, endStringIndex - startIndex));

                startIndex = line.IndexOf(' ', endStringIndex);

                if (startIndex == -1) break;

                startIndex++;

            }

            return components;
        }

        private bool ReadLineSanityCheck(List<string> record)
        {
            // check length
            if (record.Count > MAX_NO_LOG_ITEMS ||
                record.Count < MIN_NO_LOG_ITEMS) return false;

            // check URL has real Action
            if (Array.IndexOf(HTTP_ACTIONS, GetUrlActionFromString(record[4])) == -1) return false; 

            // check URL path is legit
            if (Uri.IsWellFormedUriString(GetUrlPathFromString(record[4]), UriKind.Absolute) == false &&
                (Uri.IsWellFormedUriString(GetUrlPathFromString(record[4]), UriKind.Relative) == false)) return false;

            // check DateTime field expected format
            if (record[3].Contains('[') == false ||
                record[3].Contains(']') == false) return false;

            
            return true;
        }

        private static string GetUrlPathFromString(string component)
        {
            return component.Split(' ')[1].Replace("\"", string.Empty);
        }

        private static string GetUrlActionFromString(string component)
        {
            return component.Split(' ')[0].Replace("\"", string.Empty);
        }
        private LogRecord? ParseLineRaw(List<string> components)
        {

            try
            {
                return new LogRecord
                {
                    SourceIpAddress = components[0],
                    LastUpdated = components[3],
                    HttpAction = GetUrlActionFromString(components[4]),
                    Url = GetUrlPathFromString(components[4]),
                    HttpResponse = int.Parse(components[5]),
                    ContentSize = int.Parse(components[6]),
                    Info = components[8]
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return null;
        }

    }
}
