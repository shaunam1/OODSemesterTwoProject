using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    internal class DataAccess
    {
        public IQueryable<Book> GetHomeBooksFromDatabase()
        {
            BookData db = new BookData();

            var query = from b in db.HomeBooks
                        select b;

            return query;
        }

        public IQueryable<User> GetUserData()
        {
            UserData db = new UserData();

            var query = from u in db.Users
                        select u;

            return query;
        }

        public void SaveToDatabase(decimal total, int userID, HashSet<Book> booksInCart)
        {
            OrderBookDBContext db = new OrderBookDBContext();
            //add the order details to the Orders table
            Order order1 = new Order(total, userID, booksInCart);
            db.Orders.Add(order1);
            db.SaveChanges();
        }

        public void UpdateOrders(List<Book> cartBooks, decimal total, int userID)
        {
            OrderBookDBContext db = new OrderBookDBContext();
            //HashSet prevents duplicates
            HashSet<Book> booksInCart = new HashSet<Book>();
            foreach (Book b in cartBooks)
            {
                //track the book in the database
                Book book = db.Books.Find(b.key);
                if (book != null)
                {
                    booksInCart.Add(book);
                }
            }

            //add the order details to the Orders table
            Order order1 = new Order(total, userID, booksInCart);
            db.Orders.Add(order1);
            db.SaveChanges();
        }


        public IQueryable<User> ValidateCheckout(int userID)
        {
            UserData db = new UserData();

            var query = from u in db.Users
                        where u.UserID.Equals(userID)
                        select u;

            return query;
        }
    }
}
