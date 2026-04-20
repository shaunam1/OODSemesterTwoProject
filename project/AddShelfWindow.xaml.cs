using System.IO.Packaging;
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
            MainWindow main = this.Owner as MainWindow;

            //Add shelf name
            string shelfName = tbxShelfName.Text;

            //Check that the name is not whitespace or null
            bool isWhitespace;
            isWhitespace = CheckIfWhitespace(shelfName);

           
            if (!isWhitespace) 
            {
                //Check that the name is not already taken
                bool nameTaken = false;
                nameTaken = CheckIfNameTaken(shelfName);

                if (nameTaken == false)
                {
                    //Add to ObservableCollection of Shelves
                    main.AllShelves.Add(new Shelf(shelfName));
                    //Close window when shelf added
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please choose a name that is not already taken");
                }
            }
            else
            {
                MessageBox.Show("Please enter a name for your shelf");
            }
        }

        private bool CheckIfNameTaken(string shelfName)
        {
            bool isTaken = false;

            MainWindow main = this.Owner as MainWindow;

            //Check if any existing shelf already has this name
            foreach (Shelf s in main.AllShelves)
            {
                if (s.ShelfName == shelfName)
                {
                    isTaken = true;
                }
            }

            return isTaken;
        }

        private bool CheckIfWhitespace(string shelfName)
        {
            bool isWhitespace;

            isWhitespace = string.IsNullOrWhiteSpace(shelfName);

            return isWhitespace;
        }
    }
}
