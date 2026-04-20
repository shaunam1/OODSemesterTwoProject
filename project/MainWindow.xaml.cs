using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace project
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Variables
        DataAccess dataAccess = new DataAccess();
        APIService apiService = new APIService();
        public string[] originalAuthors = { "All", "Madeline Miller", "Stephen King", "Suzanne Collins", "R.F. Kuang", "S.E. Hinton", "Donna Tartt" };
        List<Book> allBookRecords = new List<Book>();
        List<Book> homeBooksFromDatabase = new List<Book>();
        public List<User> users = new List<User>();
        List<User> usersCompare = new List<User>();
        string selectedAuthor = "", description = "";
        public bool isUserOne = true, isLoggedIn = false;
        public int userID, userNumber = 0;
        string searchTerm = "", filterShelfSearch = "";
        Shelf selectedShelf;
        bool isDatabaseBooks = false;

        /*
         * HELPFUL RESOURCES
         * I used the following resources to help me with this project
         * 1. How to Hash Passwords with BCrypt in C# - Claudio Bernasconi : https://youtu.be/UNLl4kCpwGo?si=naNR0iHKxX_0F3OA 
         * 2. Entity Framework Tutorial: https://www.entityframeworktutorial.net/code-first/what-is-code-first.aspx 
         * 3.C WPF Tutorial Playlist - Kampa Plays: https://youtube.com/playlist?list=PLih2KERbY1HHOOJ2C6FOrVXIwg4AZ-hk1&si=PZSSTrZszOdFLYjl 
         *   ->particularly tutorials 8, 13, 14, 17, and 19
         * 4. Task asynchronous programming model - Microsoft https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model 
         * 5. CollectionViewSource
         * 
         
        */

        public MainWindow()
        {
            //this = main window
            DataContext = this;
            entries = new ObservableCollection<Book>();
            shelvedEntries = new ObservableCollection<Book>();
            cart = new ObservableCollection<Book>();
            allShelves = new ObservableCollection<Shelf>();
            authorNames = new ObservableCollection<string>();
            InitializeComponent();

            var homeCollectionViewSource = new CollectionViewSource { Source = Entries };
            HomeCollectionView = homeCollectionViewSource.View;
            HomeCollectionView.Filter = FilterHomeBooks;
            //Does not work when I bind in the xaml file
            selectedBooks.ItemsSource = HomeCollectionView;

            var collectionViewSource = new CollectionViewSource { Source = shelvedEntries };
            ShelfCollectionView = collectionViewSource.View;
            ShelfCollectionView.Filter = FilterShelfBooks;
            //Does not work when I bind in the xaml file
            shelfBooks.ItemsSource = ShelfCollectionView;
            ;
        }

        #region "Observable Collections and CollectionViewSource"

        //PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //CollectionView
        //Used to filter books shown in the Bookshelf tab
        public ICollectionView ShelfCollectionView { get; set; }
        //Used to filter books shown on the Home tab
        public ICollectionView HomeCollectionView { get; set; }

        
        //ObservableCollections
        //Total value of books in the cart
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

        //Total number of books in the cart
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

        //The book that has been selected by the user
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

        //Either UserOne or UserTwo from the database
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

        //Used for books being displayed on screen in Home tab
        private ObservableCollection<Book> entries;
        public ObservableCollection<Book> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        //All the books that have been shelved
        private ObservableCollection<Book> shelvedEntries;
        public ObservableCollection<Book> ShelvedEntries
        {
            get { return shelvedEntries; }
            set { shelvedEntries = value; }
        }

        //All the shelves that have been created
        private ObservableCollection<Shelf> allShelves;
        public ObservableCollection<Shelf> AllShelves
        {
            get { return allShelves; }
            set { allShelves = value; }
        }

        //All the books that have been added to the cart
        private ObservableCollection<Book> cart;
        public ObservableCollection<Book> Cart
        {
            get { return cart; }
            set { cart = value; }
        }

        //For the names of the authors of the current books being displayed on the Home tab
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

        private void PopulateCheckout()
        {
            //Display Checkout details for the currentUser
            //I'm not using Binding here for Date and FullName
            string monthYear = users[userNumber].CardDate.ToString("MM / yy");
            tblkFullName.Text = $"{users[userNumber].FirstName} {users[userNumber].LastName}";
            tbxDate.Text = monthYear;
        }

        //Displays the books from the database
        private void ShowDatabaseBooks()
        {
            //This bool is true if the books from the database are being displayed
            isDatabaseBooks = true;

            var query = dataAccess.GetHomeBooksFromDatabase();

            //Add each book in the db to HomeBooks
            foreach (var book in query)
            {
                //Added to both list and ObservableCollection to help with filtering HomeCollectionView
                homeBooksFromDatabase.Add(book);
                Entries.Add(book);
            }

            //Add each author to authorNames
            authorNames.Add("All");
            foreach (var book in homeBooksFromDatabase)
            {
                authorNames.Add(book.author.ToString());
            }
            
            HomeCollectionView.Refresh();
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
            //Clear the search bar when it is clicked
            searchBar.Text = "";
        }

        private void spBook_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //When the StackPanel (book) is clicked the user sees information about the book
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
            DetermineAuthor(book);
            DeterminePrice(book);
            DetermineLanguages(book);

            description = await apiService.CheckForDescription(book);
            tblkDescription.Text = description;

            tblkEditions.Text = $"Current Editions: {book.edition_count}";  
        }

        private void DetermineAuthor(Book book)
        {
            if (book.author != null)
            {
                //If the book is from the database the author is in author
                string author = "";
                author = book.author.ToString();
                tblkBookInfo.Text = author;
            }
            else
            {
                //if the book was searched for the author is in author_name
                tblkBookInfo.Text = book.author_name[0];
            }
        }

        private void DeterminePrice(Book book)
        {
            //Books that are not in the database do not have a price
            if (book.price.ToString() == "" || book.price.ToString() == null)
            {
                tblkPrice.Text = "Not for sale";
            }
            else
            {
                tblkPrice.Text = $"€{book.price}";
            }
        }

        private void DetermineLanguages(Book book)
        {
            string languages = "";
            if (book.LanguageData != null)
            {
                //if the book is coming from the database the languages are in LanguageData
                languages = book.LanguageData.ToString();
                languages = languages.Replace(";", ", ");
                tblkLanguages.Text = $"Available in: {languages}";
            }
            else
            {
                //if the book was searched for the languages are in language
                foreach (string l in book.language)
                {
                    languages += l + ", ";
                }

                languages = languages.Trim();
                tblkLanguages.Text += languages.TrimEnd(',');
            }
        }

        private void btnAddtoCart_Click(object sender, RoutedEventArgs e)
        {   
            //The book can only be added to the cart if it is from the database
            if (SelectedBook.price.ToString() != "0")
            {
                if (!Cart.Contains(SelectedBook))
                {
                    //Increase the cart count
                    CartCount++;
                    Total += SelectedBook.price;

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
        {   
            //if the user has more than one shelf open a new window to select a shelf
            if (allShelves.Count > 1)
            {
                ChooseShelfWindow thirdWindow = new ChooseShelfWindow();
                thirdWindow.Owner = this;
                thirdWindow.ShowDialog();
            }
            else
            {   //If the book is not already shelved add it to the "All Books" shelf
                if (!ShelvedEntries.Contains(SelectedBook))
                {
                    ShelvedEntries.Add(SelectedBook);
                    MessageBox.Show("Book shelved");
                }
                else
                {
                    MessageBox.Show("This book is already shelved");
                }
            }
        }

        private async void tbxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            //If the enter key is pressed on the Home tab
            if (e.Key == Key.Enter)
            {
                //We are searching for books that are not in the database
                isDatabaseBooks = false;

                //Format searchTerm for the API 
                searchTerm = tbxSearch.Text.Replace(" ", "+");

                //Display the search result or home books if there are no search results
                allBookRecords = await apiService.GetBookSearchResults(searchTerm);
                if (allBookRecords.Count > 0)
                {
                    //Remove previously searched books from Entries
                    Entries.Clear();
                    //Add the new results from the API
                    AddToEntries(allBookRecords);
                    //Re-add the database books
                    AddToEntries(homeBooksFromDatabase);
                    //Update the ObservableCollection of authors
                    UpdateAuthorsListbox();
                }
                else
                {
                    isDatabaseBooks = true;
                    MessageBox.Show("No results found");
                    authorNames.Clear();
                    foreach (string s in originalAuthors)
                    {
                        authorNames.Add(s);
                    }
                }
                HomeCollectionView.Refresh();
            }
        }

        private void AddToEntries(List<Book> books)
        {   //Add each book in a list of books to the Entries ObservableCollection
            foreach (Book b in books)
            {
                Entries.Add(b);
            }
        }

        private void UpdateAuthorsListbox()
        {   //Populates the authors listbox when the books do not come from the database
            //Clear the ObservableCollection and add "All"
            authorNames.Clear();
            authorNames.Add("All");

            //For each book returned by t
            for (int j = 0; j < allBookRecords.Count; j++)
            {
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
        }

        private void lbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShelf = lbxShelves.SelectedItem as Shelf;
            //If the selected shelf has books in it show the books
            if (selectedShelf.Books != null)
            {
                //Refresh what is displayed by filtering
                ShelfCollectionView.Refresh();
            }
        }

        private void btnAddShelf_Click(object sender, RoutedEventArgs e)
        {
            //If the user wants to add a new shelf, open AddShelfWindow
            AddShelfWindow secondWindow = new AddShelfWindow();
            secondWindow.Owner = this;
            secondWindow.ShowDialog();
        }

        private void btnRemoveShelf_Click(object sender, RoutedEventArgs e)
        {   //If the user wants to remove a shelf
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
        {   //If the user is searching for a book in the Bookshelf tab

            selectedShelf = lbxShelves.SelectedItem as Shelf;
            //if no shelf is selected search "All Books" shelf
            if (selectedShelf is null)
            {
                selectedShelf = AllShelves[0];
            }

            //The search term
            filterShelfSearch = tbxShelfSearch.Text.ToLower();

            ShelfCollectionView.Refresh();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //If the user wants to remove a book from a shelf
            //If there is more than one shelf open RemoveFromShelfWindow to select a shelf
            if (ShelvedEntries.Contains(selectedBook))
            {
                if (allShelves.Count > 1)
                {
                    RemoveFromShelfWindow fourthWindow = new RemoveFromShelfWindow();
                    fourthWindow.Owner = this;
                    fourthWindow.ShowDialog();
                }
                else
                {
                    //If there is only one shelf and it contains the selectedBook
                    ShelvedEntries.Remove(SelectedBook);
                    MessageBox.Show("Book removed");
                }
            }
            else
            {
                MessageBox.Show("This book hasn't been added to a shelf");
            }

        }

        private void FilterAuthors(object sender, SelectionChangedEventArgs e)
        {   //When there is a selection change in lbxAuthor

            //selectedAuthor needed to filter books on the Home tab
            selectedAuthor = lbxAuthor.SelectedItem as string;
            HomeCollectionView.Refresh();
        }

        private void btnRemoveCart_Click(object sender, RoutedEventArgs e)
        {
            //To remove a book from the cart
            Button btn = sender as Button;
            Book book = btn.DataContext as Book;

            //Remove from Cart
            Cart.Remove(book);

            //Decrease cart count
            CartCount--;

            //Remove the cost of the removed book from the total and refresh counts
            Total -= book.price;
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

        private bool CheckUserDetailsCorrect()
        {
            bool isCorrect = false;
            //Get the user with the correct userID from the database
            var query = dataAccess.ValidateCheckout(userID);

            //Add users to usersCompare list which will be used to compare
            //what's in the database to whats on the screen
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

        private void UpdateOrdersDatabase()
        {   //Create a list containing the books in the cart
            List<Book> cartBooks = new List<Book>();
            cartBooks = Cart.ToList();
            //Update the database with the order
            dataAccess.UpdateOrders(cartBooks, Total, userID);
        }
        
        private void tblkLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {   //If the user clicks "Logout"
            //If the user is logged in
            if (isLoggedIn == true)
            {
                //Ask user to confirm that they wish to logout
                MessageBoxResult result = MessageBox.Show("Would you like to logout and close the app?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private bool FilterShelfBooks(object item)
        {
            //If the user searches for a book on the Bookshelf tab ShelfCollectionView needs to be filtered
            bool isShelfBook = false;

            //If a shelf has been selected
            if (item is Book book && selectedShelf != null)
            {
                //If there is a search term
                if (filterShelfSearch != "")
                {
                    //If a book appears on the selected shelf and its title contains the search term
                    //It remains displayed
                    if (selectedShelf.Books.Contains(book) && book.title.ToLower().Contains(filterShelfSearch))
                    {
                        isShelfBook = true;
                    }
                }
                else
                {
                    //If there is no search term and the shelf contains the book the book is displayed
                    if (selectedShelf.Books.Contains(book))
                    {
                        isShelfBook = true;
                    }
                }
            }
            return isShelfBook;
        }

        
        private bool FilterHomeBooks(object item)
        {
            //if the books on the Home tab are being filtered by author
            //HomeCollectionView needs to be filtered
            bool isHomeBook = false;

            if(item is Book book)
            {
                //If the books currently displayed are from the database
                if(isDatabaseBooks == true)
                {
                    //If there is a selected author that is not "All"
                    if (selectedAuthor != "" && selectedAuthor != null && selectedAuthor != "All")
                    {
                        //If a book is written by that author it is displayed
                        if (homeBooksFromDatabase.Contains(book) &&  book.author == selectedAuthor)
                        {
                            isHomeBook = true;
                        }
                    }
                    else
                    {
                        //If all or no authors are selected
                        if (homeBooksFromDatabase.Contains(book))
                        {
                            isHomeBook = true;
                        }
                            
                    }
                }
                else
                {
                    //If the books are from the API
                    //If there is a selected author that is not "All"
                    if (selectedAuthor != "" && selectedAuthor != null && selectedAuthor != "All")
                    {
                        //If a book is written by that author it is displayed
                        if (allBookRecords.Contains(book) && book.author_name.Count > 0 && book.author_name[0] == selectedAuthor)
                        {
                            isHomeBook = true;
                        }
                    }
                    else
                    {
                        //If all or no authors are selected
                        if (allBookRecords.Contains(book))
                        {
                            isHomeBook = true;
                        }
                    }
                }
            }
            return isHomeBook;
        }

    }
}

