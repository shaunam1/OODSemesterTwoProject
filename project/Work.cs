using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace project
{
    public class Entry
    {
        
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
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);






