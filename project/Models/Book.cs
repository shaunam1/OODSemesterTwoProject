using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
//Added - code from Keith
using System.ComponentModel.DataAnnotations.Schema;


namespace project
{
    public class Book
    {
        //Properties
        public List<string> author_key { get; set; }
        public List<string> author_name { get; set; }
        public string author { get; set; }
        public string authorKey { get; set; }
        public int cover_i { get; set; }
        public string ebook_access { get; set; }
        public int edition_count { get; set; }
        public int first_publish_year { get; set; }
        public bool has_fulltext { get; set; }
        [Key]
        public string key { get; set; }
        public List<string> language { get; set; }
        public bool public_scan_b { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        
        //Not from API
        public decimal price { get; set; }
        public string Cover_URL
        {
            get
            {
                return $"https://covers.openlibrary.org/b/id/{cover_i}-M.jpg";
            }
        }

        public ICollection<Order> Orders { get; set; }

        //Added - code from Keith
        /*
        * This code was added because the list of authors and languages being returned from the API were
        * not being added to the database
        */

        public string LanguageData { get; set; }

        private List<string> _language;

        [NotMapped]
        public List<string> languages
        {
            get
            {
                if (_language == null)
                {
                    _language = string.IsNullOrEmpty(LanguageData)
                        ? new List<string>()
                        : LanguageData.Split(';').ToList();
                }
                return _language;
            }
            set
            {
                _language = value;
                LanguageData = (value == null || !value.Any())
                    ? null
                    : string.Join(";", value);
            }
        }

        //Added - code from Keith
        public void SyncListFields()
        {
            LanguageData = (language == null || !language.Any())
                ? null
                : string.Join(";", language);
        }

        // Constructors
        public Book()
        {
            //Added - code from Keith
            author_name = new List<string>();
            languages = new List<string>(); // ensures setter runs on first use

            this.Orders = new HashSet<Order>();
        }

    }

    public class BookRoot
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public bool numFoundExact { get; set; }
        public int num_found { get; set; }
        public string documentation_url { get; set; }
        public string q { get; set; }
        public object offset { get; set; }
        public List<Book> docs { get; set; }
    }


    public class BookData : DbContext
    {
        public BookData() : base("AllOrderDetailsv7") { }
        public DbSet<Book> HomeBooks { get; set; }
    }
}
