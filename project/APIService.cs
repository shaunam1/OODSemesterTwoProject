using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            
            //        authorNames.Clear();
            //        authorNames.Add("All");

            //        for (int j = 0; j < allBookRecords.Count; j++)
            //        {
            //            searchResults.Add(bookResult.docs[j]);
            //            //if there is an author
            //            if (bookResult.docs[j].author_name.Count > 0)
            //            {
            //                //Add the author to authorNames
            //                if (!authorNames.Contains(bookResult.docs[j].author_name[0].ToString()))
            //                {
            //                    authorNames.Add(bookResult.docs[j].author_name[0].ToString());
            //                }
            //            }
            //        }
            //        //Set authors in listbox as authors of these books
            //        lbxAuthor.ItemsSource = authorNames;

            //        //Display the new results
            //        selectedBooks.ItemsSource = null;
            //        selectedBooks.ItemsSource = searchResults;
            //        isSearchAuthors = true;
            //
            //}
        }
    }
//}
