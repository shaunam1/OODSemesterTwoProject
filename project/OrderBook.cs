using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class OrderBook
    {
        [Key]
        public int OrderBookKey { get; set; }
        public string BookKey { get; set; }
        public Book Book { get; set; }
        public int OrderNumber { get; set; }
        public Order Order { get; set; }
    }

    public class OrderBookData : DbContext
    {
        public OrderBookData() : base("OrderDatav8") { }
        public DbSet<OrderBook> OrderBooks { get; set; }
    }
}
