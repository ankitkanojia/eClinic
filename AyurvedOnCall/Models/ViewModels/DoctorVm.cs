using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class CalenderVm
    {
        public string title { get; set; }
        public string url { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public long id { get; set; }
    }
}