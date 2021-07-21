using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.HttpClientExtensions;
using Newtonsoft.Json.HttpClientExtension.Tests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.HttpClientExtension.Tests
{
    [TestClass()]
    public class NewtonsoftHttpClientExtensionsTests
    {
        private const string URI = "http://localhost:12345/";

        [TestMethod()]
        public async Task GetTest_WhenHttpClientNull_ThrowsException()
        {
            //arrange
            HttpClient httpClient = null;

            //act and assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async() => await httpClient.GetFromJsonAsync<Person>(URI));

        }
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        [TestMethod()]
        public async Task GetTest_WhenUriEmptyString_ThrowsException(string emptyUri)
        {
            //arrange
            var httpClient = new HttpClient();

            //act and assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await httpClient.GetFromJsonAsync<Person>(emptyUri));

        }
    }
}