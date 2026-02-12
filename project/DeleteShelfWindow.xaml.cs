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
    /// Interaction logic for DeleteShelfWindow.xaml
    /// </summary>
    public partial class DeleteShelfWindow : Window
    {
        Shelf selectedShelf;
        public DeleteShelfWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            //How remove All Books shelf???
            //Populate combobox with shelves
            List<Shelf> deletableShelves = new List<Shelf>();
            for (int i = 1; i< main.AllShelves.Count; i++)
            {
                deletableShelves.Add(main.AllShelves[i]);
            }
            cbxDeleteShelves.ItemsSource = deletableShelves;

        }

        private void cbxDeleteShelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //shelf selected by the user is selectedShelf
            selectedShelf = cbxDeleteShelves.SelectedItem as Shelf;
        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            //If no shelf has been selected when button is pressed
            if (selectedShelf == null)
            {
                MessageBox.Show("Please choose a shelf");
            }
            else
            {
                //if the shelf selected is not "All Books" delete it
                if (selectedShelf.ShelfName != "All Books")
                {
                    //Remove shelf from observable collection and close window
                    main.AllShelves.Remove(selectedShelf);
                    this.Close();
                }
                else
                {
                    //Inform user this shelf cannot be deleted
                    MessageBox.Show("All Books shelf cannot be deleted");
                }

            }
        }
    }
}
