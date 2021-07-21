using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.HttpClientExtensions;
using Newtonsoft.Json.HttpClientExtension.Tests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Newtonsoft.Json.HttpClientExtension.Tests
{
    [TestClass()]
    public class NewtonsoftHttpClientExtensionsTests
    {
        [TestMethod()]
        public void Get()
        {
            //arrange
            var httpClient = new HttpClient();

            //act
            httpClient.GetFromJsonAsync<Person>()

            //assert
        }
    }
}