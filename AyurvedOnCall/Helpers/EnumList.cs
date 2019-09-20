using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Helpers
{
    public class EnumList
    {
        public enum Roles
        {
            Admin = 1,
            Doctor = 2,
            Patient = 3
        }

        public enum EmailTemplete
        {
            ForgotPassword = 1,
            DoctorRegistration = 2,
            AppointmentAccept = 3,
            AppointmentReject = 4
        }
    }
}