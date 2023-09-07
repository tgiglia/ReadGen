using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class ListDetail
    {
        public string list_detail_id { get; set; }
        public string list_id { get; set; }
        public int list_type_id { get; set; }
        public int alarm_class_id { get; set; }
        public DateTimeOffset created_date { get; set; }
        public DateTimeOffset begin_date { get; set; }
        public DateTimeOffset end_date { get; set; }
        public string notes { get; set; }
        public string locale_id { get; set; }
        public int listDomainId { get; set; }
    }
}
