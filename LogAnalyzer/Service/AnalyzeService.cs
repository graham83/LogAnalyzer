using LogAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Service
{
    public class AnalyzeService
    {
        private LogRepository _logRepository;

        public AnalyzeService(LogRepository repostitory)
        {
            _logRepository = repostitory; 
        }

        public int GetNumberUniqueUrls()
        {
            return _logRepository.GetNumberUniqueIpAddresses();
        }

        public List<string> GetTop3Visited()
        {
            return _logRepository.MostVisitedUrls(3);
        }

        public List<string> GetAllUrlsWithTop3VisitFrequency()
        {
            // Get default Top 3
            var mostFrequent = _logRepository.MostVisitedUrlsAndCount(1);

            var mostFrequentCount = mostFrequent.FirstOrDefault().Count;

            var allTopFreqUrls = _logRepository.UrlsByCount(mostFrequentCount);

            var nextFrequent = _logRepository.UrlsByNextFrequencyOccurrence(mostFrequentCount);

            var allSecondFreqUrls = _logRepository.UrlsByCount(nextFrequent.Count);

            var thirdFrequent = _logRepository.UrlsByNextFrequencyOccurrence(nextFrequent.Count);

            var allThirdFreqUrls = _logRepository.UrlsByCount(thirdFrequent.Count);

            var combinedUrls = new List<string>();

            combinedUrls.AddRange(allTopFreqUrls);

            combinedUrls.AddRange(allSecondFreqUrls);

            combinedUrls.AddRange(allThirdFreqUrls);

            return combinedUrls;
        }

        public List<string> GetTop3ActiveIps()
        {
            return _logRepository.MostActiveIpAddresses(3);
        }
    }
}
