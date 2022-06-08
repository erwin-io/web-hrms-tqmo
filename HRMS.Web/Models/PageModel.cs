using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace HRMS.Web.Models
{
    public class PageModel
    {
        public string Module { get; set; }
        public string MenuName { get; set; }
        public string Title { get; set; }
        public string ParentTitle { get; set; }
        public string ParentName { get; set; }
        public string ChildTitle { get; set; }
        public string ChildName { get; set; }
        public dynamic Data { get; set; } = new ExpandoObject();
    }
}