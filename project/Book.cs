using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Book
    {
        public List<string> author_key { get; set; }
        public List<string> author_name { get; set; }
        public int cover_i { get; set; }
        public string ebook_access { get; set; }
        public int edition_count { get; set; }
        public int first_publish_year { get; set; }
        public bool has_fulltext { get; set; }
        public string key { get; set; }
        public List<string> language { get; set; }
        public bool public_scan_b { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string Cover_URL
        {
            get
            {
                return $"https://covers.openlibrary.org/b/id/{cover_i}-M.jpg";
            }
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


}
