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
        [Key]
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }
        public User User { get; set; }
        public List<Book> Books { get; set; }

        public class OrderData : DbContext
        {
            public OrderData() : base("OrderData") { }
            public DbSet<Order> Orders { get; set; }
        }

    }
}
