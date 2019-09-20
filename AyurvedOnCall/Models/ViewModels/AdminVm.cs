using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class AdminDashboardVm
    {
        public int TotalDoctors { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAppointments { get; set; }
    }
}