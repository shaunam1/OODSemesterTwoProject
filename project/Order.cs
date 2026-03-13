using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class Order
    {
        //Properties
        [Key]
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
        public List<Book> Books { get; set; }
        public List<Shelf> Shelves { get; set; }

        public ICollection<OrderBook> OrderBooks { get; set; }

        //Constructors
        public Order()
        {

        }

        public Order(decimal total, int userID, List<Book> books)
        {
            
            Total = total;
            UserID = userID;
            Books = books;

        }

    }

    public class OrderData : DbContext
    {
        public OrderData() : base("OrderDatav8") { }
        public DbSet<Order> Orders { get; set; }
    }
}
