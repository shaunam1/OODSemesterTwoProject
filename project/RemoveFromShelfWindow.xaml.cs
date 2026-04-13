using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace project
{
    public partial class RemoveFromShelfWindow : Window
    {
        Shelf selectedShelf;
        
        public RemoveFromShelfWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            List<Shelf> potentialShelves = new List<Shelf>();
            Book selectedBook = main.selectedBook;
            foreach (Shelf s in main.AllShelves)
            {
                //If a shelf contains the selected book it is added to the listbox of shelves that
                //book can be removed from
                if (s.Books.Contains(selectedBook))
                {
                    potentialShelves.Add(s);
                }
            }
            cbxShelves.ItemsSource = potentialShelves;
        }

        private void cbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShelf = cbxShelves.SelectedItem as Shelf;
        }

        private void btnRemoveFrom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            Book selectedBook = main.selectedBook;
           
            //If no shelf has been selected
            if (selectedShelf == null)
            {
                MessageBox.Show("Please choose a shelf");
            }
            else
            {
                //If the "All Books" shelf which contains all shelved books is selected
                if (selectedShelf.ShelfName == "All Books")
                {
                    selectedShelf.Books.Remove(selectedBook);
                    //Remove the book from every shelf that it is contained in 
                    foreach (Shelf s in main.AllShelves)
                    {
                        if (s.Books.Contains(selectedBook)){
                            s.Books.Remove(selectedBook);
                        }
                    }
                }
                //If the selected shelf contains the selected book
                else
                {
                    //remove it from that shelf 
                    selectedShelf.Books.Remove(selectedBook);
                }
            }
            this.Close();
        }
    }
}
