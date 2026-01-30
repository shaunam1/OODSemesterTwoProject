using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace project
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://openlibrary.org/search/authors.json?q=suzanne%20collins"),
                Headers =
                {

                }
                ,
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();


                var result = JsonConvert.DeserializeObject<Root>(body);
                List<Author> allAuthorRecords = result.docs;

                foreach (Author author in allAuthorRecords)
                {
                    Console.WriteLine(author.name);
                }


                Console.ReadLine();
            }
        }
    }
}

