using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.HttpClientExtensions;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace NewtonsoftHttpClientExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();

            var stock = await httpClient.GetFromJsonAsync<Stock>("https://localhost:12345/stocks/VTSAX");
            Console.WriteLine($"Got stock {stock.Symbol}. Current price = {stock.Price}.");
            stock.Price += 0.10m;

            await httpClient.PostAsJsonAsync<Stock>("https://localhost:12345/stocks/", stock);
            Console.WriteLine("Added 10 cents to the price");
        }
    }
}
