using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class DoctorVm
    {
        public long DoctorId { get; set; }
        public string Speciality { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public DateTime DoctoringStartDate { get; set; }
        public string ProfileImg { get; set; }
        public string GenderId { get; set; }
        public string Degree { get; set; }
        public string University { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}