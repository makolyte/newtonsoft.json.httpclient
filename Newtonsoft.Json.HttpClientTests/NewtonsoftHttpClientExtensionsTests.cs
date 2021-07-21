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

        private static HttpClient BuildForGet(string json, HttpStatusCode statusCode)
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

        private static (HttpClient, HttpResponseMessage) BuildForPost(string expectedJson, HttpStatusCode statusCode)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = statusCode;
            httpResponse.Content = new StringContent("Content");

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post && r.RequestUri.ToString().StartsWith(URI) &&
                    r.Content.ReadAsStringAsync().GetAwaiter().GetResult() == expectedJson),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHandler.Object);

            return (httpClient, httpResponse);
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
            HttpClient httpClient = BuildForGet("{}", HttpStatusCode.BadRequest);


            //act and assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await httpClient.GetFromJsonAsync<Person>(URI));

        }
        [TestMethod()]
        public async Task GetTest_WhenJsonInvalid_Throws()
        {
            //arrange
            var httpClient = BuildForGet("{", HttpStatusCode.OK);


            //act and assert
           await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await httpClient.GetFromJsonAsync<Person>(URI));

        }
        [TestMethod()]
        public async Task GetTest_VerifySerializerSettingsAreUsed()
        {
            //arrange
            var httpClient = BuildForGet("{\"NotAProp\":1}", HttpStatusCode.OK);

            var jsonSettings = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error };


            //act and assert
            await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await httpClient.GetFromJsonAsync<Person>(URI, jsonSettings));

        }
        [TestMethod()]
        public async Task GetTest_ReturnsDeserializedJsonFromHttpGet()
        {
            //arrange
            var json = JsonConvert.SerializeObject(PERSON);
            var httpClient = BuildForGet(json, HttpStatusCode.OK);


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
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        [TestMethod()]
        public async Task PostTest_WhenUriEmptyString_ThrowsException(string emptyUri)
        {
            //arrange
            var httpClient = new HttpClient();

            //act and assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await httpClient.PostAsJsonAsync<Person>(emptyUri, PERSON));

        }
        [TestMethod()]
        public async Task PostTest_ValueToSendIsNull_ThrowsException()
        {
            //arrange
            HttpClient httpClient = new HttpClient();

            //act and assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await httpClient.PostAsJsonAsync<Person>(URI, null));

        }
        [TestMethod()]
        public async Task PostTest_VerifyUsesSettingsForSerialization()
        {
            //arrange
            var httpClient = new HttpClient();
            var jsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Error };
            var circle1 = new Circular();
            var circle2 = new Circular()
            {
                Link = circle1
            };
            circle1.Link = circle2;


            //act and assert
            await Assert.ThrowsExceptionAsync<JsonSerializationException>(async () => await httpClient.PostAsJsonAsync<Circular>(URI, circle1));

        }

        [TestMethod()]
        public async Task PostTest_WhenStatusNotOk_ThrowsException()
        {
            //arrange
            var expectedJson = JsonConvert.SerializeObject(PERSON);

            (var httpClient, var httpResponse) = BuildForPost(expectedJson, HttpStatusCode.BadRequest);


            //act and assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await httpClient.PostAsJsonAsync<Person>(URI, PERSON));

        }
        [TestMethod()]
        public async Task PostTest_WhenStatusOk_ReturnsResponse()
        {
            //arrange
            var expectedJson = JsonConvert.SerializeObject(PERSON);

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.OK;
            httpResponse.Content = new StringContent("Content");

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post && r.RequestUri.ToString().StartsWith(URI) &&
                    r.Content.ReadAsStringAsync().GetAwaiter().GetResult() == expectedJson),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHandler.Object);


            //act
            var response = await httpClient.PostAsJsonAsync<Person>(URI, PERSON);

            //assert
            response.Should().BeSameAs(httpResponse);

        }
    }
}