using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using LogAnalyzer.Models;
using Microsoft.Data.Sqlite;

namespace LogAnalyzer.Data.Tests
{
    [TestClass()]
    public class LogRepositoryTests
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<LogDbContext> _contextOptions;

        public LogRepositoryTests()
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

        LogRepository CreateRepository() => new LogRepository(new LogDbContext(_contextOptions));

        public void Dispose() => _connection.Dispose();


        [TestMethod()]
        public void GetLogRecordsTestShouldReturnThirteen()
        {
            //Arrange
            using var repository = CreateRepository();

            //Act
            var noRecords = repository.GetLogRecords();

            //Assert
            Assert.AreEqual(13, noRecords.Count());
        }

        [TestMethod()]
        public void GetNumberUniqueIpAddressesTestShouldReturnFour()
        {
            //Arrange
            using var repository = CreateRepository();

            //Act
            var noUnique = repository.GetNumberUniqueIpAddresses();

            //Assert
            Assert.AreEqual(4, noUnique);
        }

        [TestMethod()]
        public void MostVisitedUrlsTestVerifyReturnValues()
        {
            //Arrange
            using var repository = CreateRepository();

            //Act
            var mostVisited = repository.MostVisitedUrls(3);

            //Assert
            Assert.AreEqual("/intranet-analytics/", mostVisited[0]);
            Assert.AreEqual("/faq/how-to-install/", mostVisited[1]);
            Assert.AreEqual("/blog/2018/08/survey-your-opinion-matters/", mostVisited[2]);
        }

        [TestMethod()]
        public void MostActiveIpAddressesTestVerifyReturnValues()
        {
            //Arrange
            using var repository = CreateRepository();

            //Act
            var mostActive = repository.MostActiveIpAddresses(3);

            //Assert
            Assert.AreEqual("177.71.128.21", mostActive[0]);
            Assert.AreEqual("72.44.32.11", mostActive[1]);
            Assert.AreEqual("168.41.191.9", mostActive[2]);

        }


    }
}