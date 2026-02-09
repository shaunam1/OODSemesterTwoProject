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
        string[] bookSearch = new string[1];
        public MainWindow()
        {
            //this - main window
            DataContext = this;
            entries = new ObservableCollection<Book>();
            shelvedEntries= new ObservableCollection<Book>();
            cart = new ObservableCollection<Book>();
            search = new ObservableCollection<Book>();
            allShelves = new ObservableCollection<Shelf>();
            InitializeComponent();

            ;
        }

        private ObservableCollection<Book> entries;

        public ObservableCollection<Book> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        private ObservableCollection<Book> shelvedEntries;

        public ObservableCollection<Book> ShelvedEntries
        {
            get { return shelvedEntries; }
            set { shelvedEntries = value; }
        }

        private ObservableCollection<Shelf> allShelves;

        public ObservableCollection<Shelf> AllShelves
        {
            get { return allShelves; }
            set { allShelves = value; }
        }

        private ObservableCollection<Book> cart;

        public ObservableCollection<Book> Cart
        {
            get { return cart; }
            set { cart = value; }
        }

        private ObservableCollection<Book> search;

        public ObservableCollection<Book> Search
        {
            get { return search; }
            set { search = value; }
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            //display the selected books
            CreateBooks(chosenBooks);
            //Set ItemSources of filter listboxes
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

            //Set ItemsSource of listboxes
            lbxAuthor.ItemsSource = authors;
            Shelf allBooks = new Shelf("All Books", ShelvedEntries);
            AllShelves.Add(allBooks);
            //Need to check if this works when clicked
            lbxShelves.ItemsSource = AllShelves;
            
            
            
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
            selectedBook = book;
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

            Cart.Add(selectedBook);

        }


        private async void CreateBooks(string[] array)
        {
            //for each book in the array of selected books
            for (int i = 0; i < array.Length; i++)
            {
                //Get API response
                var bookClient = new HttpClient();
                var bookRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://openlibrary.org/search.json?q={array[i]}"),
                    Headers =
                {

                }
                    ,
                };
                //getting error while making request
                using (var bookResponse = await bookClient.SendAsync(bookRequest))
                {
                    bookResponse.EnsureSuccessStatusCode();
                    var bookBody = await bookResponse.Content.ReadAsStringAsync();
                    var bookResult = JsonConvert.DeserializeObject<BookRoot>(bookBody);

                    //add books with this title to allBookRecords
                    allBookRecords = bookResult.docs;
                    if(allBookRecords.Count > 0)
                    {
                        //add the first returned book to the selected observable collection
                        Entries.Add(bookResult.docs[0]);
                    }
                    else
                    {
                        MessageBox.Show("No results found");
                    }


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


        private void btnShelve_Click(object sender, RoutedEventArgs e)
        {
            //When Add to Bookshelf is clicked 
            //The book is added to the observable collection ShevedEntries so that it will be displayed
            //on the Bookshelf tab
            ShelvedEntries.Add(selectedBook);
            

           // for (int i = 0; i < allShelves.Count; i++)
            //{
              //  lbxShelves.ItemsSource += allShelves[i].ShelfName;
            //}
        }

        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyControl.SelectedItem = TabCheckout;
        }
        

        //I want to display 6 results
        //I want the chosenBooks to be displayed if there are no results
        private void tbxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            //If the enter key is pressed
            if (e.Key == Key.Enter)
            {
                //clear the Observable collection of entries
                Entries.Clear();
                string searchTerm = tbxSearch.Text;
                //Replace spaces with +
                searchTerm.Replace(" ", "+");
                //Set this search term as the only string in bookSearch array
                Array.Clear(bookSearch, 0, 1);
                bookSearch[0] = searchTerm;
                //Display the search result 
                CreateBooks(bookSearch);
            }
        }

        private void lbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;
            //If the selected shelf has books in it
            //Show the books
            if (selectedShelf.Books != null)
            {
                shelfBooks.ItemsSource = selectedShelf.Books;
            }
            //If not display nothing
            else
            {
                shelfBooks.ItemsSource = null;
            }
            
        }


        //This is the same as CreateBook apart from the data type it takes in
        //Can I combine these???
        

        private void btnAddShelf_Click(object sender, RoutedEventArgs e)
        {
            //Open new window
            AddShelfWindow secondWindow = new AddShelfWindow();
            secondWindow.Owner = this;
            secondWindow.ShowDialog();
            //Add shelf name

            //Add to ObservableCollection of Shelves

            //When add to bookshelf is clicked
            //Added to all books shelf
            //If no of shelves > 1
            //new window
            //Select shelf
            //added to selected shelf
            //Dropdown???
            


        }
    }
}
