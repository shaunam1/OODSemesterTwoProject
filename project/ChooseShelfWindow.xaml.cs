using System;
using System.Collections.Generic;
using System.Linq;
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

namespace project
{
    /// <summary>
    /// Interaction logic for ChooseShelfWindow.xaml
    /// </summary>
    public partial class ChooseShelfWindow : Window
    {
        Shelf selectedShelf;
        public ChooseShelfWindow()
        {
            InitializeComponent();
        }

        private void btnAddTo_Click(object sender, RoutedEventArgs e)
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
                //If the shelf already contains the selected book
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            List<Shelf> potentialShelves = new List<Shelf>();
            Book selectedBook = main.selectedBook;
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
    }
}
