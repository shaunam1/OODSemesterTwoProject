using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Windows.Input;

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
        public List<Shelf> Shelves { get; set; }

        

        //Constructors
        public Order()
        {
            this.Books = new HashSet<Book>();
        }

        public Order(decimal total, int userID, HashSet<Book> books)
        {
            Total = total;
            UserID = userID;
            this.Books = books;
        }

        public virtual ICollection<Book> Books { get; set; }
    }

    public class OrderData : DbContext
    {
        public OrderData() : base("AllOrderDetailsv7") { }
        public DbSet<Order> Orders { get; set; }
    }

    public class OrderBookDBContext : DbContext
    {
        public OrderBookDBContext() : base("AllOrderDetailsv7")
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Book> Books { get; set; }

        //Fluent API used to create a joining table name and column names
        //for many-to-many relationship between orders and books
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .HasMany<Book>(o => o.Books)
                .WithMany(b => b.Orders)
                .Map(bo =>
                {
                    bo.MapLeftKey("Order_OrderNumber");
                    bo.MapRightKey("Book_key");
                    bo.ToTable("OrderBooks");

                    /*
                     * If there is an error while trying to login change bo.ToTable("OrderBooks");
                     * to bo.ToTable("BookOrders"); and run DataManagement and UserDataManagement2 again
                     */
                });
        }
        
    }

}
