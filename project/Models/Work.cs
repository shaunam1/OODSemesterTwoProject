using System.Collections.Generic;

namespace project
{
    public class Entry
    {
        //There is a separate API needed to get a description for the book
        //I need to get all the entries for a particular author by their key
        //and find the title that matches the title of the relevant book

        //Properties
        public string title { get; set; }
        public string key { get; set; }
        public int latest_revision { get; set; }
        public int revision { get; set; }
        public List<string> subject_places { get; set; }
        public List<string> subjects { get; set; }
        public List<int> covers { get; set; }
        public object description { get; set; }
        public List<string> subject_people { get; set; }
        public List<string> subject_times { get; set; }
        public string first_publish_date { get; set; }
        public string location { get; set; }
    }

    

    public class WorkRoot
    {
        public List<Entry> entries { get; set; }
    }

}






