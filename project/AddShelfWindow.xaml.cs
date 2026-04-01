using System.Windows;

namespace project
{
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
        }
    }
}
