﻿using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;

namespace Newtonsoft.Json.HttpClientExtensions
{
    public static class NewtonsoftHttpClientExtensions
    {
        public static async Task<T> GetFromJsonAsync<T>(this HttpClient httpClient, string uri, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Can't be null or empty", nameof(uri));
            }

            var response = await httpClient.GetAsync(uri, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string uri, T value, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Can't be null or empty", nameof(uri));
            }

            throw new NotImplementedException();
        }
    }
}
