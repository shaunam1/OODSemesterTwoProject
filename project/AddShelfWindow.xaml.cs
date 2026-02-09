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
            string shelfName = tbxShelfName.Text;

            MainWindow main = this.Owner as MainWindow;

            main.AllShelves.Add(new Shelf(shelfName));
        }
    }
}
