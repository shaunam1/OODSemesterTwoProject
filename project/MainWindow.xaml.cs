using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace project
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Variables
        int cartCount = 0;
        public string[] originalAuthors = { "All", "Madeline Miller", "Stephen King", "Suzanne Collins", "R.F. Kuang", "S.E. Hinton", "Donna Tartt" };
        public Book selectedBook;
        List<Book> allBookRecords = new List<Book>();
        string selectedAuthor = "";
        public bool isSearchAuthors = false;
        public List<Book> searchResults = new List<Book>();
        string description = "";
        decimal total = 0;
        public bool isUserOne = true;
        User currentUser;
        DataAccess dataAccess = new DataAccess();
        APIService apiService = new APIService();

        public MainWindow()
        {

            //this - main window
            DataContext = this;
            homeBooks = new ObservableCollection<Book>();
            shelfFilter = new ObservableCollection<Book>();
            entries = new ObservableCollection<Book>();
            shelvedEntries = new ObservableCollection<Book>();
            cart = new ObservableCollection<Book>();
            allShelves = new ObservableCollection<Shelf>();
            authorNames = new ObservableCollection<string>();
            InitializeComponent();

            ;
        }

        #region
        //ADVICE FROM KEITH:
        //Instead of all the observable collections
        //Could Enums be used to filter?

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Book> shelfFilter;

        public ObservableCollection<Book> ShelfFilter
        {
            get { return shelfFilter; }
            set 
            { 
                shelfFilter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShelfFilter"));
            }
        }

        private ObservableCollection<Book> homeBooks;

        public ObservableCollection<Book> HomeBooks
        {
            get { return homeBooks; }
            set { homeBooks = value; }
        }

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
        public ObservableCollection<string> authorNames;

        

        public ObservableCollection<string> AuthorNames
        {
            get { return authorNames; }
            set { authorNames = value; }
        }

        #endregion 
        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Owner = this;
            loginWindow.ShowDialog();
            //display the books from HomeBooksDatav7 database
            PopulateCheckout();
            ShowDatabaseBooks();

            //Set cart counts on each tab
            RefreshCartCountsAndTotal();

            //All Books shelf automatically created so that books can be shelved
            Shelf allBooks = new Shelf("All Books", ShelvedEntries);
            AllShelves.Add(allBooks);
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

            description = await apiService.CheckForDescription(book);
            tblkDescription.Text = "Description: " + description;

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
                    decimal cost = decimal.Parse(selectedBook.price);
                    total += cost;
                    RefreshCartCountsAndTotal();

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

        private async void tbxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            //If the enter key is pressed
            if (e.Key == Key.Enter)
            {
                string searchTerm = tbxSearch.Text;
                //Replace spaces with +
                searchTerm = searchTerm.Replace(" ", "+");
                //Display the search result 
                searchResults.Clear();
                allBookRecords = await apiService.GetBookSearchResults(searchTerm);
                if (allBookRecords.Count > 0)
                {
                    DisplaySearchResults();
                }
                else
                {
                    MessageBox.Show("No results found");
                    Entries.Clear();
                    foreach (Book b in HomeBooks)
                    {
                        Entries.Add(b);
                    }
                    authorNames.Clear();
                    foreach(string s in originalAuthors)
                    {
                        authorNames.Add(s);
                    }
                    isSearchAuthors = false;
                }
            }
        }

        private void DisplaySearchResults()
        {
            authorNames.Clear();
            authorNames.Add("All");

            for (int j = 0; j < allBookRecords.Count; j++)
            {
                searchResults.Add(allBookRecords[j]);
                //if there is an author
                if (allBookRecords[j].author_name.Count > 0)
                {
                    //Add the author to authorNames
                    if (!authorNames.Contains(allBookRecords[j].author_name[0].ToString()))
                    {
                        authorNames.Add(allBookRecords[j].author_name[0].ToString());
                    }
                }
            }
            Entries.Clear();
            //Display the new results
            foreach(Book b in searchResults)
            {
                Entries.Add(b);
            }
            isSearchAuthors = true;
        }



        private void lbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Shelf selectedShelf = lbxShelves.SelectedItem as Shelf;
            //If the selected shelf has books in it show the books
            if (selectedShelf.Books != null)
            {
                ShelfFilter = selectedShelf.Books;
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
            List<Book> filteredList = new List<Book>();
            for (int i = 0; i < selectedShelf.Books.Count; i++)
            {
                //if a book contains the search term add it to search results
                if (selectedShelf.Books[i].title.ToLower().Contains(searchTerm))
                {
                    filteredList.Add(selectedShelf.Books[i]);
                }
            }
            ShelfFilter = new ObservableCollection<Book>(filteredList);
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
                //If there is only one shelf
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
            List<Book> bookList = new List<Book>();
            selectedAuthor = lbxAuthor.SelectedItem as string;

            //If this is for home page books
            if (isSearchAuthors == false)
            {
                bookList = Entries.ToList();
            }
            //If this is for search results
            //The list of books is the books returned in the search
            else
            {
                bookList = searchResults;
            }

            if (selectedAuthor != null && selectedAuthor != "All")
            {
                //If one author is selected
                //Add books by that author to authorResults
                foreach (Book b in bookList)
                {
                    //If the book has a list of authors and the first author is the selected author
                    //Or the book has one author 
                    if ((b.author_name.Count > 0 && b.author_name[0] == selectedAuthor) || b.author != null && b.author == selectedAuthor)
                    {
                        authorResults.Add(b);
                    }
                }
                Entries.Clear();
                //set this filtered list as the ItemsSource
                foreach(Book b in authorResults)
                {
                    Entries.Add(b);
                }
            }
            else
            {
                //If "All" or no author is selected Entries is the ItemsSource
                //Display all search results
                Entries.Clear();
                foreach (Book b in bookList)
                {
                    Entries.Add(b);
                }
            }
        }




        private void ShowDatabaseBooks()
        {
            var query = dataAccess.GetHomeBooksFromDatabase();
            
            //Add each book in the db to Entries
            foreach (var book in query)
            {
                HomeBooks.Add(book);
            }

            authorNames.Add("All");
            //Add each author to authorNames
            foreach (var book in HomeBooks)
            {
                authorNames.Add(book.author.ToString());
            }

            foreach(Book b in HomeBooks)
            {
                Entries.Add(b);
            }
        }

        private void btnRemoveCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Book book = btn.DataContext as Book;

            //Remove from Cart
            Cart.Remove(book);

            //Decrease cart count
            cartCount--;

            //Remove the cost of the removed book from the total and refresh counts
            decimal cost = decimal.Parse(book.price);
            total -= cost;
            RefreshCartCountsAndTotal();

        }

        private void PopulateCheckout()
        {
            int userNumber = 0;
            List<User> users = new List<User>();
            var query = dataAccess.GetUserData();

            //Add users to list
            foreach (var user in query)
            {
                users.Add(user);
            }

            //If current user = UserOne
            if (isUserOne == true)
            {
                currentUser = users[0];
                userNumber = 0;
            }
            //If current user = UserTwo
            else
            {
                currentUser = users[1];
                userNumber = 1;
            }


            //Display Checkout details for the currentUser
            //I'm not using Binding here because of monthYear and FullName
            string monthYear = users[userNumber].CardDate.ToString("MM / yy");

            tblkFullName.Text = users[userNumber].FirstName + " " + users[userNumber].LastName;
            tbxAddressLine1.Text = users[userNumber].AddressLineOne;
            tbxAddressLine2.Text = users[userNumber].AddressLineTwo;
            tbxEircode.Text = users[userNumber].Eircode;
            tbxCardNumber.Text = users[userNumber].CardNumber;
            tbxDate.Text = monthYear;
            tbxCVV.Text = users[userNumber].CVV.ToString();
        }

        private void btnBuyNow_Click(object sender, RoutedEventArgs e)
        {
            //If there's something in the cart
            if(Cart.Count > 0)
            {
                bool isCorrect;
                isCorrect = CheckUserDetailsCorrect();
                //If user details in checkout match the database
                if (isCorrect == true)
                {
                    UpdateOrdersDatabase();
                    Cart.Clear();
                    total = 0;
                    cartCount = 0;
                    RefreshCartCountsAndTotal();
                    MessageBox.Show("Thank you for your order! You will receive an email with delivery details shortly.");
                }
                else
                {
                    MessageBox.Show("Customer delivery details or card details are incorrect");
                }
            }
            else
            {
                MessageBox.Show("There are no books in your cart!");
            }
        }

        private void UpdateOrdersDatabase()
        {
            
            List<Book> cartBooks = new List<Book>();
            cartBooks = Cart.ToList();
            int userID = currentUser.UserID;
            dataAccess.UpdateOrders(cartBooks, total, userID);
        }

        private void RefreshCartCountsAndTotal()
        {
            tblkCartCount.Text = cartCount.ToString();
            tblkShelfCartCount.Text = cartCount.ToString();
            tblkCount.Text = cartCount.ToString();
            tblkTotalCost.Text = total.ToString();
        }

        private bool CheckUserDetailsCorrect()
        {
            bool isCorrect = false;
            //If the details in the checkout match the details in the database
            if(tbxAddressLine1.Text == currentUser.AddressLineOne && tbxAddressLine2.Text == currentUser.AddressLineTwo && tbxEircode.Text == currentUser.Eircode && tbxCardNumber.Text == currentUser.CardNumber && tbxDate.Text == currentUser.CardDate.ToString("MM / yy") && tbxCVV.Text == currentUser.CVV.ToString())
            {
                isCorrect = true;
            }

            return isCorrect;
        }
    }
}
