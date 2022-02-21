using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogAnalyzer.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using LogAnalyzer.Data;
using LogAnalyzer.Models;

namespace LogAnalyzer.Helpers.Tests
{
    [TestClass()]
    public class FileHelperTests
    {
        string HEALTHY_LINE = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574 \"-\" \"Mozilla/5.0 (X11; U; Linux x86_64; fr-FR) AppleWebKit/534.7 (KHTML, like Gecko) Epiphany/2.30.6 Safari/534.7\"";
       

        [TestMethod()]
        public void CheckFileLineFormatOkTestGoodDataReturnsTrue()
        {
            // Arrange
            var repositoryMock = new Mock<ILogRepository>();

            var helper = new FileHelper(repositoryMock.Object);

            //Act
            var goodLine = helper.CheckFileLineFormatOk(HEALTHY_LINE);

            //Assert

            Assert.AreEqual(true, goodLine);
        }
    }
}