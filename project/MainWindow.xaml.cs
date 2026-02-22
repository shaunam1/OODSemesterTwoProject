using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
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
        List<string> authors = new List<string>();
        string[] originalAuthors = { "R. F. Kuang", "Madeline Miller", "Donna Tartt", "Suzanne Collins", "S. E. Hinton", "Stephen King", "All"  };
        string[] chosenBooks = { "babel+or+the+necessity+of+violence", "the+song+of+achilles", "the+secret+history", "ballad+of+songbirds+and+snakes", "the+outsiders", "mr.+mercedes" };
        public Book selectedBook;
        List<Book> allBookRecords = new List<Book>();
        Shelf allBooks;
        string bookSearch = "";
        string selectedAuthor = "";
        bool isSearchAuthors = false;
        List<Book> searchResults = new List<Book>();
        public MainWindow()
        {
            //this - main window
            DataContext = this;
            entries = new ObservableCollection<Book>();
            shelvedEntries= new ObservableCollection<Book>();
            cart = new ObservableCollection<Book>();
            allShelves = new ObservableCollection<Shelf>();
            authorNames = new ObservableCollection<string>();
            InitializeComponent();

            ;
        }

        //ADVICE FROM KEITH:
        //Instead of all the observable collections
        //Could Enums be used to filter?


        //Used for books being displayed on screen in Home tab
        private ObservableCollection<Book> entries;

        public ObservableCollection<Book> Entries
        {
            get { return entries; }
            set { entries = value; }
        }


        //For all the books that have been shelved
        private ObservableCollection<Book> shelvedEntries;

        public ObservableCollection<Book> ShelvedEntries
        {
            get { return shelvedEntries; }
            set { shelvedEntries = value; }
        }

        //For all the shelves that have been created
        private ObservableCollection<Shelf> allShelves;

        public ObservableCollection<Shelf> AllShelves
        {
            get { return allShelves; }
            set { allShelves = value; }
        }


        //For all books added to the cart
        private ObservableCollection<Book> cart;

        public ObservableCollection<Book> Cart
        {
            get { return cart; }
            set { cart = value; }
        }


        private ObservableCollection<string> authorNames;

        public ObservableCollection<string> AuthorNames
        {
            get { return authorNames; }
            set { authorNames = value; }
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            //display the selected books
            CreateBooks(chosenBooks);

            //Set cart counts on each tab
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

            //Set ItemsSource of listboxes
            lbxAuthor.ItemsSource = authorNames;
            Shelf allBooks = new Shelf("All Books", ShelvedEntries);
            AllShelves.Add(allBooks);
            lbxShelves.ItemsSource = AllShelves;
        }

        
        private void tbxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clear search bar when clicked
            tbxSearch.Text = "";
        }

        //Could I remove search bar from Bookshelf Tab
        private void tbxShelfSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clear search bar when clicked on BookShelf Tab
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

            //Add the selected book to the ObservableCollection Cart
            Cart.Add(selectedBook);

        }


        private async void CreateBooks(string[] chosenBooks)
        {
            authorNames.Clear();
            authorNames.Add("All");
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
                    if(allBookRecords.Count > 0)
                    {
                        
                        //add the first returned book to the selected observable collection
                        Entries.Add(bookResult.docs[0]);
                        authorNames.Add(bookResult.docs[0].author_name[0].ToString());
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
            StackPanel button = sender as StackPanel;
            //the datacontext of the button is the object in the data template
            Book book = button.DataContext as Book;
            ShowInfo(book);
        }



        private void btnShelve_Click(object sender, RoutedEventArgs e)
        {
            //When Add to Bookshelf is clicked 
            //The book is added to the observable collection ShevedEntries so that it will be displayed
            //on the Bookshelf tab
            


            if (allShelves.Count > 1)
            {
                ChooseShelfWindow thirdWindow = new ChooseShelfWindow();
                thirdWindow.Owner = this;
                thirdWindow.ShowDialog();
            }
            else
            {
                if (!ShelvedEntries.Contains(selectedBook))
                {
                    ShelvedEntries.Add(selectedBook);
                }
                else
                {
                    MessageBox.Show("This book is already shelved");
                }
            }
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
                string searchTerm = tbxSearch.Text;
                //Replace spaces with +
                searchTerm.Replace(" ", "+");
                bookSearch = searchTerm;
                //Display the search result 
                GetBookSearchResults(bookSearch);
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
            
            

            
           
        }

        private async void GetBookSearchResults(string bookSearch)
        {
            
            searchResults.Clear();
            isSearchAuthors = true;

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
                    if (allBookRecords.Count > 0)
                    {
                        authorNames.Clear();
                        authorNames.Add("All");

                    for (int j = 0; j < allBookRecords.Count; j++)
                            //I want to add a scroll bar so I can add more results
                    {
                        searchResults.Add(bookResult.docs[j]);
                        //if there is an author
                        if(bookResult.docs[j].author_name != null)
                        {
                            //Add the author to authorNames
                            if (!authorNames.Contains(bookResult.docs[j].author_name[0].ToString()))
                            {
                                authorNames.Add(bookResult.docs[j].author_name[0].ToString());
                            }
                        }

                    }
                    //Set authors in listbox as authors of these books
                    lbxAuthor.ItemsSource = authorNames;

                    //Display the new results
                    selectedBooks.ItemsSource = null;
                    selectedBooks.ItemsSource = searchResults;
                    }
                    else
                    {
                    //If no book found 
                    //Display Entries and originalAuthors
                        MessageBox.Show("No results found");
                        selectedBooks.ItemsSource = Entries;
                        isSearchAuthors = false;
                        lbxAuthor.ItemsSource = originalAuthors;

                    }
                }
        }

        private void btnRemoveShelf_Click(object sender, RoutedEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;
            //If All Books is not the only shelf
                if (selectedShelf.ShelfName != "All Books")
                {
                    MessageBox.Show("Please exit this shelf before removing a shelf");
                }
                else
                {
                    if (AllShelves.Count > 1)
                    {
                        //Open new window
                        DeleteShelfWindow thirdWindow = new DeleteShelfWindow();
                        thirdWindow.Owner = this;
                        thirdWindow.ShowDialog();
                    }
                    else
                    {
                        //Display message to user
                        MessageBox.Show("There are no shelves to delete");
                    }
                }
        }

       private void tbxShelfSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string searchTerm = tbxShelfSearch.Text.ToLower();
            List<Book> searchResults = new List<Book>();
            for (int i = 0; i < ShelvedEntries.Count; i++)
            {
                if (ShelvedEntries[i].title.ToLower().Contains(searchTerm))
                {
                    searchResults.Add(ShelvedEntries[i]);
                }

                
            }

            shelfBooks.ItemsSource = searchResults;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (allShelves.Count > 1)
            {
                RemoveFromShelfWindow fourthWindow = new RemoveFromShelfWindow();
                fourthWindow.Owner = this;
                fourthWindow.ShowDialog();
            }
            else
            {
                if (ShelvedEntries.Contains(selectedBook))
                {
                    ShelvedEntries.Remove(selectedBook);
                }
                else
                {
                    MessageBox.Show("This book hasn't been added to a shelf");
                }
            }

        }

        private void FilterAuthors(object sender, SelectionChangedEventArgs e)
        {
            List<Book> authorResults = new List<Book>();
            selectedAuthor = lbxAuthor.SelectedItem as string;

            //If filtering Entries
            if (isSearchAuthors == false)
            {
                if (selectedAuthor != null && selectedAuthor != "All")
                {
                    //if one author is selected
                    //Add books by that author to authorResults
                    foreach (Book b in Entries)
                    {
                        if (b.author_name[0] == selectedAuthor)
                        {
                            authorResults.Add(b);
                        }

                    }

                    //set this filtered list as the ItemsSource
                    selectedBooks.ItemsSource = authorResults;
                }
                else
                {
                    //If All or no author is selected Entries is the ItemsSource
                    selectedBooks.ItemsSource = Entries;
                }
            }
            else
            {
                //If filtering search results
                if (selectedAuthor != null && selectedAuthor != "All")
                {
                    //if one author is selected
                    //Add books by that author to authorResults
                    foreach (Book b in searchResults)
                    {
                        if(b.author_name != null)
                        {
                            if (b.author_name[0] == selectedAuthor)
                            {
                                authorResults.Add(b);
                            }
                        }
                        

                    }

                    //set this filtered list as the ItemsSource
                    selectedBooks.ItemsSource = authorResults;
                }
                else
                {
                    //If All or no author is selected Entries is the ItemsSource
                    selectedBooks.ItemsSource = searchResults;
                }
            }

        }


    }
}
