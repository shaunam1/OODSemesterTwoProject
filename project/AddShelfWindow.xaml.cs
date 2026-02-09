using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddShelfWindow.xaml
    /// </summary>
    public partial class AddShelfWindow : Window
    {
        public AddShelfWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Add shelf name
            string shelfName = tbxShelfName.Text;

            MainWindow main = this.Owner as MainWindow;
            //Add to ObservableCollection of Shelves
            main.AllShelves.Add(new Shelf(shelfName));

            //Close window when shelf added
            this.Close();
           
            //If no of shelves > 1
            //new window
            //Select shelf
            //added to selected shelf
            //Dropdown???

            
        }
    }
}
