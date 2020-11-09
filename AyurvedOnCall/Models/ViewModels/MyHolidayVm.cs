using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class MyHolidayVm
    {
        public DateTime OffDate { get; set; }
        public string Reason { get; set; }
        public long DoctorId { get; set; }
        public List<Offday> Offdays { get; set; }
    }
}