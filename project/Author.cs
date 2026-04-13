using System.Collections.Generic;

namespace project
{
    public class Author
    {
        //Properties
        //I only included relevant properties from the API
        //Each author has a unique key
        public string key { get; set; }
        public string name { get; set; }
       
    }

    public class Root
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public bool numFoundExact { get; set; }
        public List<Author> docs { get; set; }
    }


}
