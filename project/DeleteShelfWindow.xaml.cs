using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace project
{
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

            //Populate combobox with all shelves apart from All books
            //as All Books shelf cannot be deleted
            List<Shelf> deletableShelves = new List<Shelf>();
            for (int i = 1; i< main.AllShelves.Count; i++)
            {
                if (main.AllShelves[i].ShelfName != "AllBooks")
                {
                    deletableShelves.Add(main.AllShelves[i]);
                }
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
                //Remove shelf from observable collection and close window
                main.AllShelves.Remove(selectedShelf);
                this.Close();
            }
        }
    }
}
