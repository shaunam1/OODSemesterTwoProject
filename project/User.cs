using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class User
    {
        
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string Eircode { get; set; }
        public string CardNumber { get; set; }
        public DateTime CardDate { get; set; }
        public int CVV { get; set; }
        public List<Order> Orders { get; set; }

        public User()
        {

        }

        public User(int userId, string username, string password, string firstName, string lastName, string addressOne, string addressTwo, string eircode, string cardNumber, DateTime date, int cvv)
        {
            UserID = userId;
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            AddressLineOne = addressOne;
            AddressLineTwo = addressTwo;
            Eircode = eircode;
            CardNumber = cardNumber;
            CardDate = date;
            CVV = cvv;
        }


        
    }

    public class UserData : DbContext
    {
        public UserData() : base("UserData") { }
        public DbSet<User> Users { get; set; }
    }
}
