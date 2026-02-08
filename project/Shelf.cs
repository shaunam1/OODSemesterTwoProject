using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class Shelf
    {
        //Properties
        public string ShelfName { get; set; }
        ObservableCollection<Book> Books { get; set; }

        //Constructors
        public Shelf()
        {

        }//Default constructor

        public Shelf(string shelfName, ObservableCollection<Book> books)
        {
            ShelfName = shelfName;
            Books = books;

        }//Default constructor
    }

}
