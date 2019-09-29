using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using Exception = System.Exception;

namespace AyurvedOnCall.Controllers
{
    public class DoctorController : Controller
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

        #region --> Dashboard
        [CheckDoctorAuthorization]
        public ActionResult Index()
        {
            RegenerateTempData();
            return View();
        }

        public JsonResult GetDoctorCalender()
        {
            try
            {
                using (_dbEntities)
                {
                    var userid = CookieHelper.Get(StaticValues.SessionUserId);
                    long doctorId = 0;
                    if (!string.IsNullOrWhiteSpace(userid))
                    {
                        doctorId = Convert.ToInt64(userid);
                    }
                    var response = new List<CalenderVm>();
                    var appointment = _dbEntities.Appointments.Where(s => s.DoctorId == doctorId).ToList();

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

                                if (Request.Url != null)
                                {
                                    calData.url = Url.Action("Appointments", "Doctor", "", protocol: Request.Url.Scheme);
                                }

                                if (item.IsRejected)
                                {
                                    calData.title = "Rejected Apmt: " + item.Fullname;
                                }
                                else
                                {
                                    if (item.IsConfirm)
                                    {
                                        calData.title = "Confirm Apmt: " + item.Fullname;
                                    }
                                    else
                                    {
                                        calData.title = "Pending Apmt: " + item.Fullname;
                                    }
                                }

                                response.Add(calData);
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
        #endregion
    }
}