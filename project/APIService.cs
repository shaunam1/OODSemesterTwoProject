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


        //A separate API call is needed to get the a book's description
        public async Task<string> CheckForDescription(Book book)
        {
            List<Entry> allWorks = new List<Entry>();
            string key = "", description = "";
            bool isDescription = false;

            //If a search result the author's key is in author_key
            if (book.author_key != null)
            {
                key = book.author_key[0].ToString();
            }
            //If from the database the author's key is in authorKey
            else
            {
                key = book.authorKey;
            }

            //Get works (books) by this author
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
                                //set this as the description for the selected book
                                description = allWorks[i].description.ToString();
                                isDescription = true;
                            }
                        }
                        i++;
                    }
                    //Stop when we get a description or we've gone through all works
                    while (i < allWorks.Count && isDescription == false);

                    //if there's no description
                    if (isDescription == false)
                    {
                        description = "Description unavailable";
                    }
                }
                //if there are no results returned
                else
                {
                    description = "Description unavailable";
                }
            }

            return description;
        }


    }
 }

