using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Models.ViewModels
{
    public class LoginVm
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class MapVm
    {
        public string Name { get; set; }
        public string Speciality { get; set; }
        public long DoctorId { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    public class HomeVm
    {
        public List<MapVm> MapCollection { get; set; }
        public string DoctorName { get; set; }
        public List<string> DiseasesIds { get; set; }
        public List<string> SpeacialityIds { get; set; }
        public bool IsFocus { get; set; }
    }
}