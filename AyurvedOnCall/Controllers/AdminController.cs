using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AyurvedOnCall.Controllers
{
    public class AdminController : Controller
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
            try
            {
                using (_dbEntities)
                {

                    RegenerateTempData();
                    return View(new LoginVm());
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
        public ActionResult Index(LoginVm data)
        {
            try
            {
                using (_dbEntities)
                {
                    var user = _dbEntities.UserMasters.FirstOrDefault(s => s.Email == data.Email && s.Password == data.Password);

                    if (user != null)
                    {
                        if (user.RoleMasterId == (int)EnumList.Roles.Admin)
                        {
                            SignInRemember(data.Email, true);
                            CookieHelper.Set(StaticValues.SessionUserId, user.UserMasterId.ToString(), true, 365);
                            CookieHelper.Set(StaticValues.SessionFullName, user.FullName, true, 365);
                            CookieHelper.Set(StaticValues.SessionProfileImg, user.ProfileImg, true, 365);
                            CookieHelper.Set(StaticValues.SessionRoleId, user.RoleMasterId.ToString(), true, 365);

                            TempData["Success"] = "Welcome to admin panel";
                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            TempData["Error"] = "You are not authorize to access this panel";
                            return View(data);
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Wrong credentials found";
                        return View(data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult ForgotPassword()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();
                    return View(new LoginVm());
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
        public ActionResult ForgotPassword(LoginVm data)
        {
            try
            {
                using (_dbEntities)
                {
                    var user = _dbEntities.UserMasters.FirstOrDefault(s => s.Email == data.Email);

                    if (user != null)
                    {
                        if (Request.Url != null)
                        {
                            var loginUrl = Url.Action("Index", "Admin", "", protocol: Request.Url.Scheme);

                            var replacements = new Dictionary<string, string>
                            {
                                { "#Password#", user.Password},
                                { "#login#", loginUrl},
                                { "#name#", user.FullName}
                            };

                            SendgridEmailHelpers.SendTempleteEmail((int)EnumList.EmailTemplete.ForgotPassword, user.Email, user.FullName, replacements);
                        }

                        TempData["Success"] = "We have send the login information in your mailbox";
                        return RedirectToAction("Index");
                    }

                    TempData["Error"] = "This email is not found in our system";
                    return View(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult Dashboard()
        {
            try
            {
                RegenerateTempData();
                using (_dbEntities)
                {
                    var data = new AdminDashboardVm
                    {
                        TotalDoctors = _dbEntities.Doctors.Count(s => !s.IsDelete),
                        TotalAppointments = _dbEntities.Appointments.Count(),
                        TotalUsers = _dbEntities.UserMasters.Count(s => s.RoleMasterId == (int)EnumList.Roles.Patient && !s.IsDelete)
                    };
                    return View(data);
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
    }
}