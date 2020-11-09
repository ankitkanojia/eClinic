using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class AvailableSlabVm
    {
        public long DoctorId { get; set; }
        public long AvailableSlabId { get; set; }
        public string DoctorName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public List<AvailableSlabListVm> Slabs { get; set; }
    }

    public class AvailableSlabListVm
    {
        public long AvailableSlabId { get; set; }
        public long DoctorId { get; set; }
        public string SlabTime { get; set; }
        public string DoctorName { get; set; }
    }
}