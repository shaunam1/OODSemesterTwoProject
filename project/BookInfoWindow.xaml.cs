using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace project
{
    /// <summary>
    /// Interaction logic for BookInfoWindow.xaml
    /// </summary>
    public partial class BookInfoWindow : Window
    {
        public BookInfoWindow()
        {
            InitializeComponent();

        }

        private async void wdwBookInfo_Loaded(object sender, RoutedEventArgs e)
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



                //tblkFilter.Text = allAuthorRecords[0].name;

                var bookClient = new HttpClient();
                var bookRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://openlibrary.org/search.json?q=babel+or+the+necessity+of+violence"),
                    Headers =
                {

                }
                    ,
                };
                using (var bookResponse = await client.SendAsync(bookRequest))
                {
                    bookResponse.EnsureSuccessStatusCode();
                    var bookBody = await bookResponse.Content.ReadAsStringAsync();


                    var bookResult = JsonConvert.DeserializeObject<BookRoot>(bookBody);
                    List<Book> allBookRecords = bookResult.docs;
                    tblkBookInfo.Text = allBookRecords[0].title;

                }




                //Get covers and set as the sources for the images
            }
        }
    }
}
