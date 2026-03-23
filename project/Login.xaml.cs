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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            bool isCorrect = false;

            UserData db = new UserData();

            var query = from u in db.Users
                        select u;

            List<User> users = new List<User>();
            foreach  (var user in query)
            {
                users.Add(user);
            }

            if (tbxUsername.Text == users[0].Username || tbxUsername.Text == users[1].Username)
            {
                int userNo = 0;

                if(tbxUsername.Text == users[1].Username)
                {
                    userNo = 1;
                    main.isUserOne = false;
                }
                if (BCrypt.Net.BCrypt.EnhancedVerify(tbxPassword.Text, users[userNo].Password) == true)
                {
                    isCorrect = true;
                }
                if (tbxUsername.Text == users[userNo].Username && isCorrect == true)
                {
                   this.Close();
                    main.isLoggedIn = true;
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect.");
                    main.isLoggedIn = false;
                }
            }
            else
            {
                MessageBox.Show("Username or password is incorrect.");
                main.isLoggedIn = false;
            }
        }
    }
}
