using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using project;
using Newtonsoft.Json;

namespace DataManagement
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BookData db = new BookData();


            using (db)
            {
                string[] chosenBooks = { "babel+or+the+necessity+of+violence", "the+song+of+achilles", "the+secret+history", "ballad+of+songbirds+and+snakes", "the+outsiders", "mr.+mercedes" };
                List<Book> books = await CreateBooks(chosenBooks);

                //Create books
                Book book1 = books[0];
                book1.price = 11.95m;
                book1.author =  "R.F. Kuang";
                book1.authorKey = "OL7486601A";
                Book book2 = books[1];
                book2.price = 12.00m;
                book2.author = "Madeline Miller";
                book2.authorKey = "OL1926056A";
                Book book3 = books[2];
                book3.price = 13.50m;
                book3.author = "Donna Tartt";
                book3.authorKey = "OL841112A";
                Book book4 = books[3];
                book4.price = 14.00m;
                book4.author = "Suzanne Collins";
                book4.authorKey = "OL1394359A";
                Book book5 = books[4];
                book5.price = 9.99m;
                book5.author = "S.E. Hinton";
                book5.authorKey = "OL397826A";
                Book book6 = books[5];
                book6.price = 11.90m;
                book6.author = "Stephen King";
                book6.authorKey = "OL19981A";
                

                //ADDED - code from Keith
                book1.SyncListFields();
                book2.SyncListFields();
                book3.SyncListFields();
                book4.SyncListFields();
                book5.SyncListFields();
                book6.SyncListFields();

                //Add books to the database and save the changes
                db.HomeBooks.Add(book1);
                db.HomeBooks.Add(book2);
                db.HomeBooks.Add(book3);
                db.HomeBooks.Add(book4);
                db.HomeBooks.Add(book5);
                db.HomeBooks.Add(book6);

                Console.WriteLine("Books added to database");

                db.SaveChanges();

                Console.WriteLine("Changes saved to database");
            }
        }

        private static async Task<List<Book>> CreateBooks(string[] chosenBooks)
        {
            List<Book> allBookRecords = new List<Book>();
            List<Book> books = new List<Book>();
            //for each book in the array of selected books
            for (int i = 0; i < chosenBooks.Length; i++)
            {
                //Get API response
                var bookClient = new HttpClient();
                var bookRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://openlibrary.org/search.json?q={chosenBooks[i]}"),
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
                    if (allBookRecords.Count > 0)
                    {
                        books.Add(bookResult.docs[0]);
                    }
                }
            }
            return books;
        }
    }

}
