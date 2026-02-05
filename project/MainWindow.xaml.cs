using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int cartCount = 0;
        string authorAPI = "https://openlibrary.org/search/authors.json?q=suzanne%20collins";
        string bookSearchAPI = "https://openlibrary.org/search.json?q=the+lord+of+the+rings";
        string[] genres = { "Horror", "Fantasy", "Thriller", "Romance" };
        string[] authors = { "Suzanne Collins", "R.F. Kuang", "Stephen King", "Donna Tartt", "Madelline Miller", "S.E. Hinton" };
        string[] chosenBooks = { "babel+or+the+necessity+of+violence", "the+song+of+achilles", "the+secret+history", "ballad+of+songbirds+and+snakes", "the+outsiders", "mr.+mercedes" };
        Book selectedBook;
        List<Book> allBookRecords = new List<Book>();
        List<Book> allBooks = new List<Book>();
        public MainWindow()
        {
            //this - main window
            DataContext = this;
            entries = new ObservableCollection<Book>();
            InitializeComponent();

            ;
        }

        private ObservableCollection<Book> entries;

        public ObservableCollection<Book> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        private async void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            //display the selected books
            CreateBooks();
            //Set ItemSources of filter listboxes
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

            //Set ItemsSource of listboxes
            lbxAuthor.ItemsSource = authors;
            lbxShelfAuthor.ItemsSource = authors;
        }

        //Clear search bar when clicked
        private void tbxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxSearch.Text = "";
        }

        private void tbxShelfSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxShelfSearch.Text = "";
        }


        private void ShowInfo(Book book)
        {
            //Change tab to TabBookInfo
            MyControl.SelectedItem = TabBookInfo;
            //Display information about the selected book
            tblkTitle.Text = book.title;
            imgCover.Source = new BitmapImage(new Uri($"{book.Cover_URL}", UriKind.Absolute));
            tblkBookInfo.Text = "Author: " + book.author_name[0];
        }


        private void btnAddtoCart_Click(object sender, RoutedEventArgs e)
        {
            //When add to cart is clicked
            //Increase the cart count on all tabs
            cartCount++;
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

            //add chosen books to JSON file of books in the cart
            string jsonString = JsonConvert.SerializeObject(selectedBook, Formatting.Indented);
            System.IO.File.AppendAllText("cartItems.json", jsonString);

        }


        private async void CreateBooks()
        {
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
                    //add the first returned book to the observable collection Entries
                    Entries.Add(bookResult.docs[0]);

                }
            }
        }

        private void spBook_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //how can I do this so that I don't need the button
            //  ShowInfo();
        }

        private void btnMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            //this just gets me the main window
            //this - refers to main window
            //Book obj = this.DataContext as Book;
            //MessageBox.Show(obj.title);

            //sender - is the button
            Button button = sender as Button;
            //the datacontext of the button is the object in the data template
            Book book = button.DataContext as Book;
            ShowInfo(book);
        }


    }
}
