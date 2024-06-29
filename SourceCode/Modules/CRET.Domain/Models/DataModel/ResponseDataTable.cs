using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.DataModel
{
    public class ResponseDataTable<T> where T : class
    {
        public int total { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
        public int last_page { get; set; }
        public string next_page_url { get; set; }
        public string prev_page_url { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public List<T> data { get; set; }
        public string stringData { get; set; }
    }
}
