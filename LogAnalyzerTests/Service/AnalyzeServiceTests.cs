using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogAnalyzer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using LogAnalyzer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using LogAnalyzerTests;
using LogAnalyzer.Models;

namespace LogAnalyzer.Service.Tests
{
    [TestClass()]
    public class AnalyzeServiceTests
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<LogDbContext> _contextOptions;

        public string MOST_VISITED_URL = "/intranet-analytics/";
        public string SECOND_MOST_VISITED_URL = "/faq/how-to-install/";
        public string THIRD_MOST_VISITED_URL = "/blog/2018/08/survey-your-opinion-matters/";
        public AnalyzeServiceTests()
        {

            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<LogDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new LogDbContext(_contextOptions);

            context.Database.EnsureCreated();

            DbInitialize.Seed(context);

        }

        LogRepository CreateRepository() => new LogRepository(new LogDbContext(_contextOptions));

        public void Dispose() => _connection.Dispose();

        [TestMethod()]
        public void GetAllUrlsWithTop3VisitFrequencyTest()
        {
            //Arrange
            using var repository = CreateRepository();

            var testRecord = new LogRecord
            {
                SourceIpAddress = "177.71.128.21",
                HttpAction = "GET",
                Url = "/home"
            };

            repository.InsertLogRecord(testRecord);
            repository.Save();

            //Act
            var analyzer = new AnalyzeService(repository);

            var top3FrequentUrls = analyzer.GetAllUrlsWithTop3VisitFrequency();

            //Assert
            Assert.AreEqual(MOST_VISITED_URL, top3FrequentUrls[0]);
            Assert.AreEqual(SECOND_MOST_VISITED_URL, top3FrequentUrls[1]);
            Assert.AreEqual(THIRD_MOST_VISITED_URL, top3FrequentUrls[2]);
            Assert.AreEqual(testRecord.Url, top3FrequentUrls[3]);
        }
    }
}