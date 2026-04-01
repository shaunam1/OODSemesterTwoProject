using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace project
{
    public partial class ChooseShelfWindow : Window
    {
        Shelf selectedShelf;
        public ChooseShelfWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            List<Shelf> potentialShelves = new List<Shelf>();
            Book selectedBook = main.SelectedBook;

            //if a shelf does not contain this book
            //it is added to the dropdown of shelves that the user can add the selectedBook to
            foreach (Shelf s in main.AllShelves)
            {
                if (!s.Books.Contains(selectedBook))
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

        private void btnAddTo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            Book selectedBook = main.SelectedBook;

            //If no shelf has been selected
            if (selectedShelf == null)
            {
                MessageBox.Show("Please choose a shelf");
            }
            else
            {
                //Add to the selected shelf 
                selectedShelf.Books.Add(selectedBook);
                if (selectedShelf.ShelfName != "All Books")
                {
                    //add to Observable collection if not already added
                    if (!main.ShelvedEntries.Contains(selectedBook))
                    {
                        main.ShelvedEntries.Add(selectedBook);
                    }
                }
                this.Close();
            }

        }
    }
}
