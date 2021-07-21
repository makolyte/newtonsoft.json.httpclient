using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.HttpClientExtensions;
using Newtonsoft.Json.HttpClientExtension.Tests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using FluentAssertions;

namespace Newtonsoft.Json.HttpClientExtension.Tests
{
    [TestClass()]
    public class NewtonsoftHttpClientExtensionsTests
    {
        private const string URI = "http://localhost:12345/";
        private readonly Person PERSON = new Person()
        {
            FirstName = "Bob",
            LastName = "Test",
            IsProgrammer = true,
            BirthDate = new DateTime(year: 1977, month: 7, day: 7)
        };

        private static HttpClient Build(string json, HttpStatusCode statusCode)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = statusCode;
            httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().StartsWith(URI)),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
            
            return new HttpClient(mockHandler.Object);
        }

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

        [TestMethod()]
        public async Task GetTest_WhenStatusNotOK_ThrowsException()
        {
            //arrange
            HttpClient httpClient = Build("{}", HttpStatusCode.BadRequest);


            //act and assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await httpClient.GetFromJsonAsync<Person>(URI));

        }

        [TestMethod()]
        public async Task GetTest_WhenJsonInvalid_Throws()
        {
            //arrange
            var httpClient = Build("{", HttpStatusCode.OK);


            //act and assert
           await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await httpClient.GetFromJsonAsync<Person>(URI));

        }

        [TestMethod()]
        public async Task GetTest_VerifySerializerSettingsAreUsed()
        {
            //arrange
            var httpClient = Build("{\"NotAProp\":1}", HttpStatusCode.OK);

            var jsonSettings = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error };


            //act and assert
            await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await httpClient.GetFromJsonAsync<Person>(URI, jsonSettings));

        }

        [TestMethod()]
        public async Task GetTest_ReturnsDeserializedJsonFromHttpGet()
        {
            //arrange
            var json = JsonConvert.SerializeObject(PERSON);
            var httpClient = Build(json, HttpStatusCode.OK);


            //act
            var actualPerson = await httpClient.GetFromJsonAsync<Person>(URI);

            //assert
            actualPerson.Should().BeEquivalentTo(PERSON);

        }
        [TestMethod()]
        public async Task PostTest_WhenHttpClientNull_ThrowsException()
        {
            //arrange
            HttpClient httpClient = null;

            //act and assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => 
                await httpClient.PostAsJsonAsync<Person>(URI, PERSON));

        }


    }
}