using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Web.Models
{
    public class ExceptionModel
    {
        public string Title { get; set; }
        public string HelpLink { get; set; }
        public int HResult { get; protected set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
    }
}