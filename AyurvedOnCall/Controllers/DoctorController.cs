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

        #region --> Login & Registration
        public ActionResult Authentication()
        {
            RegenerateTempData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Doctor data)
        {
            try
            {
                using (_dbEntities)
                {
                    var user = _dbEntities.Doctors.FirstOrDefault(s => s.Email == data.Email && s.Password == data.Password && !s.IsDelete);

                    if (user != null)
                    {
                        if (!user.IsActive)
                        {
                            TempData["Error"] = "Your account is not activated";
                            return RedirectToAction("Authentication");
                        }

                        if (user.RoleMasterId == (int)EnumList.Roles.Doctor)
                        {
                            SignInRemember(data.Email, true);
                            CookieHelper.Set(StaticValues.SessionUserId, user.DoctorId.ToString(), true, 365);
                            CookieHelper.Set(StaticValues.SessionFullName, user.FullName, true, 365);
                            CookieHelper.Set(StaticValues.SessionProfileImg, user.ProfileImg, true, 365);
                            CookieHelper.Set(StaticValues.SessionRoleId, user.RoleMasterId.ToString(), true, 365);

                            TempData["Success"] = "Welcome to doctor panel";
                            return RedirectToAction("Index");
                        }

                        TempData["Error"] = "You are not authorize to access this panel";
                        return RedirectToAction("Authentication");
                    }

                    TempData["Error"] = "Wrong credentials found";
                    return RedirectToAction("Authentication");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public ActionResult Logout()
        {
            EnsureLoggedOut();
            return RedirectToAction("Index");
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
            {
                // First we clean the authentication ticket like always
                //required NameSpace: using System.Web.Security;
                FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                //required NameSpace: using System.Security.Principal;
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();
            }

            //Remove cookie
            CookieHelper.Delete(StaticValues.SessionFullName);
            CookieHelper.Delete(StaticValues.SessionProfileImg);
            CookieHelper.Delete(StaticValues.SessionRoleId);
            CookieHelper.Delete(StaticValues.SessionUserId);
        }

        private static void SignInRemember(string userName, bool isPersistent)
        {
            // Clear any lingering authencation data
            FormsAuthentication.SignOut();

            // Write the authentication cookie
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Doctor data)
        {
            try
            {
                using (_dbEntities)
                {
                    var isEmailExist = _dbEntities.Doctors.FirstOrDefault(s => s.Email == data.Email);
                    if (isEmailExist != null)
                    {
                        TempData["Error"] = "Email (" + data.Email + ") already exists in our system";
                        return RedirectToAction("Authentication");
                    }

                    data.RoleMasterId = (int)EnumList.Roles.Doctor;
                    data.CreatedDate = DateTime.UtcNow;
                    data.IsActive = false;
                    data.IsDelete = false;
                    _dbEntities.Doctors.Add(data);
                    _dbEntities.SaveChanges();

                    TempData["Success"] = "Doctor request added successfully. Please wait for confirmation from admin";

                    return RedirectToAction("Authentication");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        #endregion

        #region --> Appointment
        [CheckDoctorAuthorization]
        public ActionResult Appointments()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();
                    var data = _dbEntities.Appointments.Where(s => !s.IsDelete).ToList();
                    return View(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [CheckDoctorAuthorization]
        public ActionResult AcceptAppointment(long id)
        {
            try
            {
                using (_dbEntities)
                {
                    var data = _dbEntities.Appointments.Find(id);

                    if (data != null)
                    {
                        data.IsConfirm = true;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();


                        //Send a mail to patient
                        if (Request.Url != null)
                        {
                            var replacements = new Dictionary<string, string>
                            {
                                { "#name#", data.Fullname},
                                { "#date#", data.AppointmentDate.ToString("dd-MM-yyyy HH:mm tt")},
                                { "#complaint#", data.Complaint}
                            };
                            SendgridEmailHelpers.SendTempleteEmail((int)EnumList.EmailTemplete.AppointmentAccept, data.Email, data.Fullname, replacements);
                        }

                        TempData["Success"] = "Appointment accepted successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Appointment details not found";
                    }
                    return RedirectToAction("Appointments");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [CheckDoctorAuthorization]
        public ActionResult RejectAppointment(long id)
        {
            try
            {
                using (_dbEntities)
                {
                    var data = _dbEntities.Appointments.Find(id);

                    if (data != null)
                    {
                        data.IsRejected = true;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        //Send a mail to patient
                        if (Request.Url != null)
                        {
                            var replacements = new Dictionary<string, string>
                            {
                                { "#name#", data.Fullname},
                                { "#date#", data.AppointmentDate.ToString("dd-MM-yyyy HH:mm tt")},
                                { "#complaint#", data.Complaint}
                            };
                            SendgridEmailHelpers.SendTempleteEmail((int)EnumList.EmailTemplete.AppointmentReject, data.Email, data.Fullname, replacements);
                        }

                        TempData["Success"] = "Appointment rejected successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Appointment details not found";
                    }
                    return RedirectToAction("Appointments");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region --> Availablility Slab
        public ActionResult AvailableSlab()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();

                    long doctorId = 0;
                    var userid = CookieHelper.Get(StaticValues.SessionUserId);
                    if (!string.IsNullOrWhiteSpace(userid))
                    {
                        doctorId = Convert.ToInt64(userid);
                    }

                    var doctor = _dbEntities.Doctors.Find(doctorId);

                    if (doctor != null)
                    {
                        var availableSlabVm = new AvailableSlabVm
                        {
                            Slabs = new List<AvailableSlabListVm>(),
                            DoctorName = doctor.FullName,
                            DoctorId = doctor.DoctorId
                        };

                        var availableSlabList = doctor.AvailableSlabs.OrderByDescending(m => m.CreatedDate).Select(k => new AvailableSlabListVm
                        {
                            AvailableSlabId = k.AvailableSlabId,
                            SlabTime = k.IsAllDay ? "All Day" : k.StartTime + " To " + k.EndTime
                        }).ToList();

                        if (!availableSlabList.Any())
                        {
                            availableSlabList = new List<AvailableSlabListVm>();
                        }

                        availableSlabVm.Slabs = availableSlabList;

                        return View(availableSlabVm);
                    }

                    TempData["Error"] = "Your are logout. Please login again";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvailableSlab(AvailableSlabVm data)
        {
            try
            {
                using (_dbEntities)
                {
                    var availableSlab = new AvailableSlab
                    {
                        IsAllDay = data.IsAllDay,
                        CreatedDate = DateTime.Now
                    };
                    if (!data.IsAllDay)
                    {
                        availableSlab.StartTime = data.StartTime;
                        availableSlab.EndTime = data.EndTime;
                    }
                    else
                    {
                        availableSlab.StartTime = string.Empty;
                        availableSlab.EndTime = string.Empty;
                    }
                    availableSlab.DoctorId = data.DoctorId;

                    var exitingAvailableSlab = _dbEntities.AvailableSlabs.Where(k => k.DoctorId == data.DoctorId).ToList();
                    if (data.IsAllDay)
                    {
                        if (exitingAvailableSlab.Count > 0)
                        {
                            _dbEntities.AvailableSlabs.RemoveRange(exitingAvailableSlab);
                        }
                    }
                    else
                    {
                        if (exitingAvailableSlab.Count == 1)
                        {
                            var fod = exitingAvailableSlab.FirstOrDefault();

                            if (fod != null && fod.IsAllDay)
                            {
                                _dbEntities.AvailableSlabs.RemoveRange(exitingAvailableSlab);
                            }
                        }
                    }
                    _dbEntities.AvailableSlabs.Add(availableSlab);
                    _dbEntities.SaveChanges();
                    TempData["Success"] = "Slab added successfully.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return RedirectToAction("AvailableSlab", "Doctor");
        }

        [HttpGet]
        public ActionResult Delete(long id)
        {
            try
            {
                using (_dbEntities)
                {
                    var availableSlabs = _dbEntities.AvailableSlabs.Find(id);
                    if (availableSlabs != null)
                    {
                        _dbEntities.AvailableSlabs.Remove(availableSlabs);
                        _dbEntities.SaveChanges();
                        TempData["Success"] = "Slab deleted successfully.";
                    }
                    else
                    {
                        TempData["Success"] = "Slab not deleted successfully.";
                    }

                    return RedirectToAction("AvailableSlab", "Doctor");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        #endregion
    }
}