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
        Book selectedBook;
        public ChooseShelfWindow()
        {
            InitializeComponent();
        }

        private void btnAddTo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            Book selectedBook = main.selectedBook;
            if (selectedShelf == null)
            {
                MessageBox.Show("Please choose a shelf");
            }
            else
            {
                if (selectedShelf.ShelfName != "All Books" && !selectedShelf.Books.Contains(selectedBook))
                {
                    selectedShelf.Books.Add(selectedBook);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("This shelf already contains this book");
                }
                    
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            cbxShelves.ItemsSource = main.AllShelves;

        }

        private void cbxShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShelf = cbxShelves.SelectedItem as Shelf;
        }
    }
}
