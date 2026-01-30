using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{


    public class Author
    {
        public List<string> alternate_names { get; set; }
        public string birth_date { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public List<string> top_subjects { get; set; }
        public string top_work { get; set; }
        public string type { get; set; }
        public int work_count { get; set; }
        public double ratings_average { get; set; }
        public double ratings_sortable { get; set; }
        public int ratings_count { get; set; }
        public int ratings_count_1 { get; set; }
        public int ratings_count_2 { get; set; }
        public int ratings_count_3 { get; set; }
        public int ratings_count_4 { get; set; }
        public int ratings_count_5 { get; set; }
        public int want_to_read_count { get; set; }
        public int already_read_count { get; set; }
        public int currently_reading_count { get; set; }
        public int readinglog_count { get; set; }
        public double _version_ { get; set; }
        public string death_date { get; set; }
    }

    public class Root
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public bool numFoundExact { get; set; }
        public List<Author> docs { get; set; }
    }


}
