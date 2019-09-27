using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AyurvedOnCall.Controllers
{
    public class PatientController : Controller
    {
        private readonly DBEntities _dbEntities = new DBEntities();

        public void RegenerateTempData()
        {
            if (TempData["Success"] != null)
            {
                TempData["Success"] = TempData["Success"];
            }

            if (TempData["Error"] != null)
            {
                TempData["Error"] = TempData["Error"];
            }
        }

        public ActionResult Index()
        {
            RegenerateTempData();
            return View();
        }

        public ActionResult Appointment(long id = 0)
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();
                    var model = new Appointment();

                    var doctor = _dbEntities.Doctors.Find(id);
                    if (doctor != null)
                    {
                        model.DoctorId = doctor.DoctorId;
                    }

                    var userId = CookieHelper.Get(StaticValues.SessionUserId);

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        var uid = Convert.ToInt64(userId);
                        var user = _dbEntities.UserMasters.Find(uid);
                        if (user != null)
                        {
                            model.Email = user.Email;
                            model.Mobile = user.Mobile;
                            model.Fullname = user.FullName;
                        }
                    }


                    return View(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}