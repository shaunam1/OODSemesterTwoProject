using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace project
{
    public class User
    {
        //Properties
        [Key]
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

        }//Default constructor

        public User(int userId, string username, string firstName, string lastName, string addressOne, string addressTwo, string eircode, string cardNumber, DateTime date, int cvv)
        {
            UserID = userId;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            AddressLineOne = addressOne;
            AddressLineTwo = addressTwo;
            Eircode = eircode;
            CardNumber = cardNumber;
            CardDate = date;
            CVV = cvv;
        }//Parameterised constructor

    }

    public class UserData : DbContext
    {
        public UserData() : base("AllOrderDetailsv7") { }
        public DbSet<User> Users { get; set; }
    }
}
