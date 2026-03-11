using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using project;
using Newtonsoft.Json;


namespace UserDataManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserData db = new UserData();

            using (db)
            {
                User user1 = new User(1, "UserOne", "useronepassword", "John", "Doe", "Sligo Town", "Sligo", "S87T0P5", "1234 5678 9101 1213", new DateTime(2028, 01, 01), 123);
                User user2 = new User(1, "UserTwo", "usertwopassword", "Jane", "Doe", "Grange", "Sligo", "S85T0Q1", "1234 5678 5678 1234", new DateTime(2029, 07, 03), 567);

                db.Users.Add(user1);
                db.Users.Add(user2);

                Console.WriteLine("Users added to database");

                db.SaveChanges();

                Console.WriteLine("Changes saved to database");
            }
        }
    }
}
