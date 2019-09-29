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
    }
}