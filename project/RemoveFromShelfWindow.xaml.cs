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
    /// Interaction logic for RemoveFromShelfWindow.xaml
    /// </summary>
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
                //If the shelf contains the selected book
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
