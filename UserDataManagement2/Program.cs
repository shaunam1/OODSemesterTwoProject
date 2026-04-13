using System;
using project;

namespace UserDataManagement2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserData db = new UserData();

            using (db)
            {
                //Create users
                User user1 = new User(1, "UserOne", "John", "Doe", "Sligo Town", "Sligo", "S87T0P5", "1234 5678 9101 1213", new DateTime(2028, 01, 01), 123);
                User user2 = new User(2, "UserTwo", "Jane", "Doe", "Grange", "Sligo", "S85T0Q1", "1234 5678 5678 1234", new DateTime(2029, 07, 03), 567);


                //Create and encrypt a password for each user
                string userOnePassword = "useronepassword";
                string userOnePasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userOnePassword, 11);

                string userTwoPassword = "usertwopassword";
                string userTwoPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userTwoPassword, 11);

                Console.WriteLine(userOnePasswordHash);
                Console.WriteLine(userTwoPasswordHash);

                user1.Password = userOnePasswordHash;
                user2.Password = userTwoPasswordHash;

                //Add users to database and save changes
                db.Users.Add(user1);
                db.Users.Add(user2);

                Console.WriteLine("Users added to database");

                db.SaveChanges();

                Console.WriteLine("Changes saved to database");
            }
        }
    }
}
