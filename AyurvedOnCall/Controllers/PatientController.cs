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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Appointment(Appointment data)
        {
            try
            {
                using (_dbEntities)
                {
                    data.CreatedDate = DateTime.Now;
                    data.IsActive = true;
                    data.IsDelete = false;
                    data.IsRejected = false;
                    data.IsConfirm = false;
                    _dbEntities.Appointments.Add(data);
                    _dbEntities.SaveChanges();

                    TempData["Success"] = "Appointment request added successfully. Wait for confirmation from doctor.";
                }

                return RedirectToAction("Appointment");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public JsonResult GetDoctorCalender()
        {
            try
            {
                using (_dbEntities)
                {
                    var response = new List<CalenderVm>();
                    var userid = CookieHelper.Get(StaticValues.SessionUserId);

                    if (!string.IsNullOrWhiteSpace(userid))
                    {
                        var uid = Convert.ToInt64(userid);
                        var user = _dbEntities.UserMasters.Find(uid);

                        if (user != null)
                        {
                            var appointment = _dbEntities.Appointments.Where(s => s.Email == user.Email).ToList();

                            if (appointment.Any())
                            {
                                foreach (var item in appointment)
                                {
                                    if (Request.Url != null)
                                    {
                                        var calData = new CalenderVm
                                        {
                                            id = item.AppointmentId,
                                            end = item.AppointmentDate.ToString("yyyy-MM-dd"),
                                            start = item.AppointmentDate.ToString("yyyy-MM-dd")
                                        };

                                        //if (Request.Url != null)
                                        //{
                                        //    calData.url = Url.Action("Appointment", "Patient", "", protocol: Request.Url.Scheme);
                                        //}

                                        if (item.IsRejected)
                                        {
                                            calData.title = item.Doctor.FullName + " Rejected Appointment ";
                                        }
                                        else
                                        {
                                            if (item.IsConfirm)
                                            {
                                                calData.title = item.Doctor.FullName + " Confirm Appointment ";
                                            }
                                            else
                                            {
                                                calData.title = item.Doctor.FullName + " Pending Appointment  ";
                                            }
                                        }

                                        response.Add(calData);
                                    }
                                }
                            }
                        }
                    }


                    return Json(new { status = true, data = response.ToArray() }, JsonRequestBehavior.AllowGet);
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