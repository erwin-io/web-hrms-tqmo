using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.API.Models
{
    public class DataTableResponseModel<T>
    {
        public T data { get; set; }
        public long recordsFiltered { get; set; } = 0;
        public long recordsTotal { get; set; } = 0;
        public long draw { get; set; } = 0;

        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; }
        public string DeveloperMessage { get; set; }
        public T Data { get; set; }
        public bool IsWarning { get; set; } = false;

    }
}