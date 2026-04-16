using System.Collections.Generic;
using System.Linq;

namespace project
{
    internal class DataAccess
    {
        public IQueryable<Book> GetHomeBooksFromDatabase()
        {
            //return homebooks in database as a query
            BookData db = new BookData();

            var query = from b in db.HomeBooks
                        select b;

            return query;
        }

        public IQueryable<User> GetUserData()
        {
            //return users in database as a query
            UserData db = new UserData();

            var query = from u in db.Users
                        select u;

            return query;
        }

        public void UpdateOrders(List<Book> cartBooks, decimal total, int userID)
        {
            OrderBookDBContext db = new OrderBookDBContext();

            //Need to add the books from cartBooks (a list made from Cart [ObservableCollection])
            //to a HashSet (for the Order constructor)
            HashSet<Book> booksInCart = new HashSet<Book>();

            foreach (Book b in cartBooks)
            {
                /*
                 * I had to ask ChatGPT for help with this method
                 * I did not orginally have Book book = db.Books.Find(b.key);
                 * But I kept getting an error that duplicate keys cannot be inserted into dbo.Books
                 * I was unable to find anything else online that worked
                 * I added this line so that the book with that primary key is found in dbo.Books and so there is no attempt to add
                 * the book to the table again
                 * This resolved the issue
                 */

                //track the book in the database
                Book book = db.Books.Find(b.key);
                if (book != null)
                {
                    booksInCart.Add(book);
                }
            }

            //add the order details to the Orders table and save
            Order order1 = new Order(total, userID, booksInCart);
            db.Orders.Add(order1);
            db.SaveChanges();
        }


        public IQueryable<User> ValidateCheckout(int userID)
        {
            //Return the user with the correct UserID from the database
            UserData db = new UserData();

            var query = from u in db.Users
                        where u.UserID.Equals(userID)
                        select u;

            return query;
        }
    }
}
