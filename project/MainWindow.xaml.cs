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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace project
{
    public partial class MainWindow : Window
    {
        //Variables
        int cartCount = 0;
        string[] originalAuthors = { "All", "Madeline Miller", "Stephen King", "Suzanne Collins", "R.F. Kuang", "S.E. Hinton", "Donna Tartt" };
        public Book selectedBook;
        List<Book> allBookRecords = new List<Book>();
        List<Entry> allWorks = new List<Entry>();
        string bookSearch = "";
        string selectedAuthor = "";
        bool isSearchAuthors = false;
        List<Book> searchResults = new List<Book>();
        string description = "";
        bool isDescription = false;
        bool isReady = false;
        public MainWindow()
        {
            //this - main window
            DataContext = this;
            entries = new ObservableCollection<Book>();
            shelvedEntries = new ObservableCollection<Book>();
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

        //For the authors of the books of the current book results
        private ObservableCollection<string> authorNames;

        public ObservableCollection<string> AuthorNames
        {
            get { return authorNames; }
            set { authorNames = value; }
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            //display the books from HomeBooksDatav7 database
            ShowDatabaseBooks();

            //Set cart counts on each tab
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

            //Set ItemsSource of listbox
            lbxAuthor.ItemsSource = authorNames;

            //All Books shelf automatically created so that books can be shelved
            Shelf allBooks = new Shelf("All Books", ShelvedEntries);
            AllShelves.Add(allBooks);
            lbxShelves.ItemsSource = AllShelves;
        }
        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if the user clicks the cart icon they are brough to the checkout tab
            MyControl.SelectedItem = TabCheckout;
        }

        private void tbxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clear search bar when clicked
            tbxSearch.Text = "";
        }

        private void tbxShelfSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clear search bar when clicked on BookShelf Tab
            tbxShelfSearch.Text = "";
        }

        private void spBook_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel button = sender as StackPanel;
            //the datacontext of the button is the object in the data template
            Book book = button.DataContext as Book;
            ShowInfo(book);
        }

        private async void ShowInfo(Book book)
        {
            selectedBook = book;

            //Change tab to TabBookInfo
            MyControl.SelectedItem = TabBookInfo;

            //Display information about the selected book
            tblkTitle.Text = book.title;
            imgCover.Source = new BitmapImage(new Uri($"{book.Cover_URL}", UriKind.Absolute));

            if (book.author != null)
            {
                //If the book is from HomeBooksDatav7 the author is in author
                string author = "";
                tblkBookInfo.Text = "Author: ";
                author = book.author.ToString();
                tblkBookInfo.Text += author;
            }
            else
            {
                //if the book was searched for the author is in book.author_name
                tblkBookInfo.Text = "Author: " + book.author_name[0];

            }

            tblkPrice.Text = "Price: €";
            if (book.price is null)
            {
                book.price = "Not for sale";
                tblkPrice.Text = tblkPrice.Text.TrimEnd('€');
            }
            tblkPrice.Text += book.price;
            tblkPublished.Text = "First Published: " + book.first_publish_year;

            await CheckForDescription(book);
            if (isDescription == true)
            {
                description = description.Replace(".", "\n");
                tblkDescription.Text = "Description: " + description;
            }
            else
            {
                tblkDescription.Text = "Description: Unavailable";
            }

            tblkEditions.Text = "Current Editions: " + book.edition_count;

            tblkLanguages.Text = "Available in: ";
            string languages = "";
            if (book.LanguageData != null)
            {
                //if the book is coming from the HomeBooksv7 database the languages are in Language data
                languages = book.LanguageData.ToString();
                languages = languages.Replace(";", ", ");
                tblkLanguages.Text += languages;
            }
            else
            {
                //if the book was searched for the languages are in book.language
                foreach (string l in book.language)
                {
                    languages += l + ", ";
                }

                languages = languages.Trim();
                tblkLanguages.Text += languages.TrimEnd(',');
            }
        }


        private void btnAddtoCart_Click(object sender, RoutedEventArgs e)
        {   //When add to cart is clicked increase the cart count on all tabs
            if (selectedBook.price != "Not for sale")
            {
                if (!Cart.Contains(selectedBook))
                {
                    //Increase the cart count
                    cartCount++;
                    tblkCartCount.Text = cartCount.ToString();
                    tblkShelfCartCount.Text = cartCount.ToString();
                    tblkCount.Text = cartCount.ToString();

                    //add chosen books to JSON file of books in the cart
                    //SHOULD I REMOVE JSON???
                    string jsonString = JsonConvert.SerializeObject(selectedBook, Formatting.Indented);
                    System.IO.File.AppendAllText("cartItems.json", jsonString);
                    //Add the selected book to the ObservableCollection Cart
                    Cart.Add(selectedBook);
                }
                else
                {
                    MessageBox.Show("This book is already in your cart");
                }
            }
            else
            {
                MessageBox.Show("Sorry this book is not for sale");
            }
        }
        private void btnShelve_Click(object sender, RoutedEventArgs e)
        {   //When Add to Bookshelf is clicked 
            //The book is added to the observable collection ShevedEntries so that it will be displayed
            //on the Bookshelf tab

            //if the user has more than one shelf
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

        private async void GetBookSearchResults(string bookSearch)
        {
            searchResults.Clear();

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
                    {
                        searchResults.Add(bookResult.docs[j]);
                        //if there is an author
                        if (bookResult.docs[j].author_name.Count > 0)
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
                    isSearchAuthors = true;
                }
                else
                {
                    //If no book found display Entries and originalAuthors
                    MessageBox.Show("No results found");
                    selectedBooks.ItemsSource = Entries;
                    lbxAuthor.ItemsSource = originalAuthors;
                    isSearchAuthors = false;
                }
            }
        }

        private void lbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;
            //If the selected shelf has books in it show the books
            if (selectedShelf.Books != null)
            {
                shelfBooks.ItemsSource = selectedShelf.Books;
            }
        }

        private void btnAddShelf_Click(object sender, RoutedEventArgs e)
        {
            //Open AddShelfWindow
            AddShelfWindow secondWindow = new AddShelfWindow();
            secondWindow.Owner = this;
            secondWindow.ShowDialog();
        }

        private void btnRemoveShelf_Click(object sender, RoutedEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;

            //Can't delete a shelf if that shelf is currently selected
            if (selectedShelf != null && selectedShelf.ShelfName != "All Books")
            {
                MessageBox.Show("Please exit this shelf before removing a shelf");
            }
            else
            {
                //if All Books is not the only shelf
                if (AllShelves.Count > 1)
                {
                    //Open new window
                    DeleteShelfWindow thirdWindow = new DeleteShelfWindow();
                    thirdWindow.Owner = this;
                    thirdWindow.ShowDialog();
                }
                else
                {
                    //Display message to user as All Books cannot be deleted
                    MessageBox.Show("There are no shelves to delete");
                }
            }
        }

       private void tbxShelfSearch_KeyUp(object sender, KeyEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;
            //if no shelf is selected search All Books shelf
            if (selectedShelf is null)
            {
                selectedShelf = AllShelves[0];
            }
            string searchTerm = tbxShelfSearch.Text.ToLower();
            List<Book> searchResults = new List<Book>();
            for (int i = 0; i < selectedShelf.Books.Count; i++)
            {
                //if a book contains the search term add it to search results
                if (selectedShelf.Books[i].title.ToLower().Contains(searchTerm))
                {
                    searchResults.Add(selectedShelf.Books[i]);
                }
            }
            //display search results
            shelfBooks.ItemsSource = searchResults;
        }


        //FINISHED EDITING HERE
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
            List<Book> bookList = new List<Book>();

            if (isSearchAuthors == false)
            {
                bookList = Entries.ToList();
            }
            else
            {
                bookList = searchResults;
            }

            if (selectedAuthor != null && selectedAuthor != "All")
            {
                    //if one author is selected
                    //Add books by that author to authorResults
                foreach (Book b in bookList)
                {
                    if ((b.author_name.Count > 0 && b.author_name[0] == selectedAuthor) || b.author != null && b.author == selectedAuthor)
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
                selectedBooks.ItemsSource = bookList;
            }
        }

        

        private async Task CheckForDescription(Book book)
        {
            string key = "";
            if (book.author_key != null)
            {

                key = book.author_key[0].ToString();
            }
            else
            {
                key = book.authorKey;
            }
           
            
            var worksClient = new HttpClient();
            var worksRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                //how do I include the {} in the URL???
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

                //add books with this title to allBookRecords
                allWorks = workResult.entries;
                if (allWorks.Count > 0)
                {
                    for (int i = 0; i < allWorks.Count; i++)
                    {
                        if (allWorks[i].title == book.title)
                        {
                            if (allWorks[i].description != null)
                            {
                                description = allWorks[i].description.ToString();
                                isDescription = true;
                            }
                            else
                            {
                                isDescription = false;
                            }
                        }
                    }
                    //add the first returned book to the selected observable collection
                    isReady = true;
                }
                else
                {
                    //do i need this???
                    MessageBox.Show("No results found");
                }


                }

            

            
        }

        private void ShowDatabaseBooks()
        {
            BookData db = new BookData();

            var query = from b in db.HomeBooks
                        select b;

            foreach (var book in query)
            {
                Entries.Add(book);
                
            }

            authorNames.Add("All");

            foreach (var book in Entries)
            {
                authorNames.Add(book.author.ToString());
            }
            
        }

        private void btnRemoveCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Book book = btn.DataContext as Book;
            Cart.Remove(book);
            cartCount--;
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();

        }
    }
}
