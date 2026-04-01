using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace project
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Variables
        DataAccess dataAccess = new DataAccess();
        APIService apiService = new APIService();
        public string[] originalAuthors = { "All", "Madeline Miller", "Stephen King", "Suzanne Collins", "R.F. Kuang", "S.E. Hinton", "Donna Tartt" };
        List<Book> allBookRecords = new List<Book>();
        public List<Book> searchResults = new List<Book>();
        List<Book> homeBooksList = new List<Book>();
        public List<User> users = new List<User>();
        List<User> usersCompare = new List<User>();
        string selectedAuthor = "", description = "";
        public bool isSearchAuthors = false, isUserOne = true, isLoggedIn = false;
        public int userID, userNumber = 0;

        public MainWindow()
        {
            //this = main window
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public decimal total;
        public decimal Total
        {
            get { return total; }
            set
            {
                total = value;
                OnPropertyChanged();
            }
        }

        public int cartCount;
        public int CartCount
        {
            get { return cartCount; }
            set
            {
                cartCount = value;
                OnPropertyChanged();
            }
        }

        public Book selectedBook;

        public Book SelectedBook
        {
            get { return selectedBook;  }
            set
            {
                selectedBook = value;
                OnPropertyChanged();
            }
        }
        

        public User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set 
            { 
                currentUser = value;
                OnPropertyChanged();
            }
        }

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
            //if logged in display the books from the HomeBooks database and populate checkout
            if (isLoggedIn == true)
            {
                PopulateCheckout();
                ShowDatabaseBooks();

                //Set cart count
                CartCount = 0;

                //All Books shelf automatically created so that books can be shelved
                Shelf allBooks = new Shelf("All Books", ShelvedEntries);
                AllShelves.Add(allBooks);
            }
        }

        private void PackIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if the user clicks the cart icon they are brough to the checkout tab
            MyControl.SelectedItem = TabCheckout;
        }

        private void tbxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSearchBar(sender);
        }

        private void tbxShelfSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSearchBar(sender);
        }
        private void ClearSearchBar(object sender)
        {
            //sender is the object that called the method (either tbxSearch or tbxShelfSearch)
            TextBox searchBar = sender as TextBox;
            searchBar.Text = "";
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
            SelectedBook = book;

            //Change tab to TabBookInfo
            MyControl.SelectedItem = TabBookInfo;

            //Display information about the selected book
            imgCover.Source = new BitmapImage(new Uri($"{book.Cover_URL}", UriKind.Absolute));

            DetermineAuthor(book);
            DeterminePrice(book);
            DetermineLanguages(book);

            description = await apiService.CheckForDescription(book);
            tblkDescription.Text = description;

            tblkEditions.Text = "Current Editions: " + book.edition_count;  
        }


        private void btnAddtoCart_Click(object sender, RoutedEventArgs e)
        {   //When add to cart is clicked increase the cart count on all tabs
            if (SelectedBook.price != "Not for sale")
            {
                if (!Cart.Contains(SelectedBook))
                {
                    //Increase the cart count
                    CartCount++;
                    decimal cost = decimal.Parse(SelectedBook.price);
                    Total += cost;

                    //Add the selected book to the ObservableCollection Cart
                    Cart.Add(SelectedBook);
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
                if (!ShelvedEntries.Contains(SelectedBook))
                {
                    ShelvedEntries.Add(SelectedBook);
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
                searchTerm = searchTerm.Replace(" ", "+");
                //Display the search result or home books if there are no search results
                searchResults.Clear();
                allBookRecords = await apiService.GetBookSearchResults(searchTerm);
                if (allBookRecords.Count > 0)
                {
                    DisplaySearchResults();
                }
                else
                {
                    MessageBox.Show("No results found");
                    authorNames.Clear();
                    foreach (string s in originalAuthors)
                    {
                        authorNames.Add(s);
                    }
                    
                    DetermineEntries(homeBooksList);

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
            DetermineEntries(searchResults);
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
                if (ShelvedEntries.Contains(SelectedBook))  
                {
                    ShelvedEntries.Remove(SelectedBook);
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

            bookList = DetermineBookList();

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
                DetermineEntries(authorResults);
            }
            else
            {
                //If "All" or no author is selected
                DetermineEntries(bookList);
            }
        }

        private void ShowDatabaseBooks()
        {
            var query = dataAccess.GetHomeBooksFromDatabase();
            
            //Add each book in the db to HomeBooks
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

            homeBooksList = HomeBooks.ToList();
            DetermineEntries(homeBooksList);
        }

        private void btnRemoveCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Book book = btn.DataContext as Book;

            //Remove from Cart
            Cart.Remove(book);

            //Decrease cart count
            CartCount--;

            //Remove the cost of the removed book from the total and refresh counts
            decimal cost = decimal.Parse(book.price);
            Total -= cost;
        }

        private void PopulateCheckout()
        {
            //Display Checkout details for the currentUser
            //I'm not using Binding here for Date and FullName
            string monthYear = users[userNumber].CardDate.ToString("MM / yy");
            tblkFullName.Text = users[userNumber].FirstName + " " + users[userNumber].LastName;
            tbxDate.Text = monthYear;
        }

        private void btnBuyNow_Click(object sender, RoutedEventArgs e)
        {
            //If there's something in the cart
            if(Cart.Count > 0)
            {
                bool isCorrect = CheckUserDetailsCorrect();
                //If user details in checkout match the database
                if (isCorrect == true)
                {
                    UpdateOrdersDatabase();
                    Cart.Clear();
                    Total = 0;
                    CartCount = 0;
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
            int userOrderID = CurrentUser.UserID;
            dataAccess.UpdateOrders(cartBooks, Total, userOrderID);
        }
        
        private bool CheckUserDetailsCorrect()
        {
            bool isCorrect = false;
            var query = dataAccess.ValidateCheckout(userID);

            //Add users to list
            foreach (var user in query)
            {
                usersCompare.Add(user);
            }
            //If the details in the checkout match the details in the database
            if (tbxAddressLine1.Text == usersCompare[0].AddressLineOne && tbxAddressLine2.Text == usersCompare[0].AddressLineTwo && tbxEircode.Text == usersCompare[0].Eircode && tbxCardNumber.Text == usersCompare[0].CardNumber && tbxDate.Text == usersCompare[0].CardDate.ToString("MM / yy") && tbxCVV.Text.ToString() == usersCompare[0].CVV.ToString())
            {
                isCorrect = true;
            }
            return isCorrect;
        }

        private void tblkLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isLoggedIn == true)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to logout and close the app?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void DetermineAuthor(Book book)
        {
            if (book.author != null)
            {
                //If the book is from HomeBooksDatav7 the author is in author
                string author = "";
                author = book.author.ToString();
                tblkBookInfo.Text = author;
            }
            else
            {
                //if the book was searched for the author is in book.author_name
                tblkBookInfo.Text = book.author_name[0];
            }
        }

        private void DeterminePrice(Book book)
        {
            if (book.price is null)
            {
                book.price = "Not for sale";
            }
            tblkPrice.Text = $"€{book.price}";
        }

        private void DetermineLanguages(Book book)
        {
            string languages = "";
            if (book.LanguageData != null)
            {
                //if the book is coming from the HomeBooksv7 database the languages are in Language data
                languages = book.LanguageData.ToString();
                languages = languages.Replace(";", ", ");
                tblkLanguages.Text = $"Available in: {languages}";
            }
            else
            {
                //if the book was
                //searched for the languages are in book.language
                foreach (string l in book.language)
                {
                    languages += l + ", ";
                }

                languages = languages.Trim();
                tblkLanguages.Text += languages.TrimEnd(',');
            }
        }

        private List<Book> DetermineBookList()
        {
            //If this is for home page books
            if (isSearchAuthors == false)
            {
                return HomeBooks.ToList();
            }
            //If this is for search results
            //The list of books is the books returned in the search
            else
            {
               return searchResults;
            }
        }

        private void DetermineEntries(List<Book> listOfBooks)
        {
            Entries.Clear();
            foreach (Book b in listOfBooks)
            {
                Entries.Add(b);
            }
        }
    }
}
