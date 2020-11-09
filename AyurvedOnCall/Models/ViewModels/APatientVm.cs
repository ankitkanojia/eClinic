using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class AppointmentVm
    {
        public long AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string GenderId { get; set; }
        public string Complaint { get; set; }
        public string Allergies { get; set; }
        public string Mobile { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsRejected { get; set; }
    }
}