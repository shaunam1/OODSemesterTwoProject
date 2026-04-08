using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace project
{
    public class APIService
    {
        public async Task<List<Book>> GetBookSearchResults(string bookSearch)
        {
            List<Book> allBookRecords = new List<Book>();
            //Get API response
            var bookClient = new HttpClient();
            var bookRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://openlibrary.org/search.json?q={bookSearch}"),
                Headers =
                {

                }
                ,
            };
            using (var bookResponse = await bookClient.SendAsync(bookRequest))
            {
                bookResponse.EnsureSuccessStatusCode();
                var bookBody = await bookResponse.Content.ReadAsStringAsync();
                var bookResult = JsonConvert.DeserializeObject<BookRoot>(bookBody);

                //add books with this title to allBookRecords
                allBookRecords = bookResult.docs;

                return allBookRecords as List<Book>;
            }
        }

        public async Task<string> CheckForDescription(Book book)
        {
            List<Entry> allWorks = new List<Entry>();
            string key = "", description = "";
            bool isDescription = false;
            //If a search result
            if (book.author_key != null)
            {
                key = book.author_key[0].ToString();
            }
            //If a home page book
            else
            {
                key = book.authorKey;
            }

            //Get works by this author
            var worksClient = new HttpClient();
            var worksRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://openlibrary.org/authors/{key}/works.json"),
                Headers =
            {

            }
                ,
            };
            using (var worksResponse = await worksClient.SendAsync(worksRequest))
            {
                worksResponse.EnsureSuccessStatusCode();
                var workBody = await worksResponse.Content.ReadAsStringAsync();
                var workResult = JsonConvert.DeserializeObject<WorkRoot>(workBody);

                //add books with the selected title to allBookRecords
                allWorks = workResult.entries;
                if (allWorks.Count > 0)
                {
                    int i = 0;
                    do
                    {
                        if (allWorks[i].title == book.title)
                        {
                            //if this book has a description 
                            if (allWorks[i].description != null)
                            {
                                description = allWorks[i].description.ToString();
                                isDescription = true;
                            }
                        }
                        i++;
                    }
                    //Stop when we get a description or we've gone through all works
                    while (i < allWorks.Count && isDescription == false);
                    if (isDescription == false)
                    {
                        description = "Description unavailable";
                    }
                    isDescription = false;
                }
                else
                {
                    isDescription = false;
                    description = "unavailable";
                }
            }

            return description;
        }


    }
 }

