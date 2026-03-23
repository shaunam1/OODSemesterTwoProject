using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project;
using BCrypt;

namespace UserDataManagement2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserData db = new UserData();

            using (db)
            {
                User user1 = new User(1, "UserOne", "John", "Doe", "Sligo Town", "Sligo", "S87T0P5", "1234 5678 9101 1213", new DateTime(2028, 01, 01), 123);
                User user2 = new User(2, "UserTwo", "Jane", "Doe", "Grange", "Sligo", "S85T0Q1", "1234 5678 5678 1234", new DateTime(2029, 07, 03), 567);

                string userOnePassword = "useronepassword";
                string userOnePasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userOnePassword, 11);

                string userTwoPassword = "usertwopassword";
                string userTwoPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userTwoPassword, 11);

                Console.WriteLine(userOnePasswordHash);
                Console.WriteLine(userTwoPasswordHash);

                user1.Password = userOnePasswordHash;
                user2.Password = userTwoPasswordHash;

                db.Users.Add(user1);
                db.Users.Add(user2);

                Console.WriteLine("Users added to database");

                db.SaveChanges();

                Console.WriteLine("Changes saved to database");
            }
        }
    }
}
