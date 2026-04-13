using System.Collections.Generic;
using System.Windows;

namespace project
{
    public partial class Login : Window
    {
        DataAccess dataAccess = new DataAccess();
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            bool isCorrect = false;
            List<User> users = new List<User>();
            int userNo = 0, userId = 1;
            string incorrectMessage = "Username or password is incorrect.";

            //Get users from db
            var query = dataAccess.GetUserData();

            //add users to list of users
            foreach  (var user in query)
            {
                users.Add(user);
                main.users.Add(user);
            }

            //If the username matches either of the two users
            if (tbxUsername.Text == users[0].Username || tbxUsername.Text == users[1].Username)
            {
                //if user 2 assign the userNo as 1
                if(tbxUsername.Text == users[1].Username)
                {
                    userNo = 1;
                    userId = 2;
                    main.isUserOne = false;
                }

                //populate values in mainwindow.xaml.cs
                PopulateMainWindowValues(userNo, users, userId);

                //check if password matched encrypted password in db
                isCorrect = CheckPasswordCorrect(users, userNo);
               
                if (isCorrect == true)
                {
                   this.Close();
                   main.isLoggedIn = true;
                }
                else
                {
                    MessageBox.Show(incorrectMessage);
                    main.isLoggedIn = false;
                }
            }
            else
            {
                MessageBox.Show(incorrectMessage);
                main.isLoggedIn = false;
            }
        }

        private void PopulateMainWindowValues(int userNo, List<User>users, int userId)
        {
            MainWindow main = this.Owner as MainWindow;
            main.userNumber = userNo;
            main.CurrentUser = users[userNo];
            main.userID = userId;
        }

        private bool CheckPasswordCorrect(List<User>users, int userNo)
        {
            bool isCorrect = false;

            //Check if the decrypted password from the database matches the password entered
            if (BCrypt.Net.BCrypt.EnhancedVerify(tbxPassword.Text, users[userNo].Password) == true)
            {
                isCorrect = true;
            }
            return isCorrect;
        }
    }
}
