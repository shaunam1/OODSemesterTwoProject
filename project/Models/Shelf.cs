using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace project
{
    public class Shelf
    {
        //Properties
        [Key]
        public int ShelfID { get; set; }
        public string ShelfName { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        //Constructors
        public Shelf()
        {

        }//Default constructor

        public Shelf(string shelfName)
        {
            ShelfName = shelfName;
            Books = new ObservableCollection<Book>();
        }//Parameterised constructor

        public Shelf(string shelfName, ObservableCollection<Book> books)
        {
            ShelfName = shelfName;
            Books = books;

        }//Parameterised constructor

        public override string ToString()
        {
            return ShelfName;
        }
    }
}
