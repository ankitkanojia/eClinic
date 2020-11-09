using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;

namespace AyurvedOnCall.Controllers
{
    public class HomeController : Controller
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

                    var doctors = _dbEntities.Doctors.Where(s => !s.IsDelete && s.IsActive).ToList();

                    var homeVm = new HomeVm
                    {
                        MapCollection = new List<MapVm>(),
                        IsFocus = false
                    };

                    //Filter the data for OFFDAY
                    var startDateTime = DateTime.Today; //Today at 00:00:00
                    var endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
                    var offdays = _dbEntities.Offdays.Where(s => s.OffDate >= startDateTime && s.OffDate <= endDateTime).ToList();
                    if (offdays.Any())
                    {
                        foreach (var item in offdays)
                        {
                            var removeDoctor = doctors.FirstOrDefault(s => s.DoctorId == item.DoctorId);
                            if (removeDoctor != null)
                            {
                                doctors.Remove(removeDoctor);
                            }
                        }
                    }

                    foreach (var item in doctors)
                    {
                        var latitude = Convert.ToDouble(item.Latitude);
                        var longitude = Convert.ToDouble(item.Longitude);

                        homeVm.MapCollection.Add(new MapVm
                        {
                            DoctorId = item.DoctorId,
                            Name = item.FullName,
                            Speciality = item.Speciality.Name,
                            Lat = latitude,
                            Long = longitude
                        });
                    }

                    return View(homeVm);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Index(HomeVm data)
        {
            try
            {
                var doctors = _dbEntities.Doctors.Where(s => !s.IsDelete && s.IsActive).ToList();

                //Filter all condition
                if (!string.IsNullOrWhiteSpace(data.DoctorName))
                {
                    doctors = doctors.Where(s => s.FullName.ToLower().Contains(data.DoctorName.ToLower())).ToList();
                }

                var speciality = new List<long>();

                if (data.SpeacialityIds != null && data.SpeacialityIds.Any())
                {
                    foreach (var item in data.SpeacialityIds)
                    {
                        speciality.Add(Convert.ToInt64(item));
                    }
                }

                if (data.DiseasesIds != null && data.DiseasesIds.Any())
                {
                    var diseases = _dbEntities.Diseases.ToList();

                    foreach (var item in data.DiseasesIds)
                    {
                        var fod = diseases.FirstOrDefault(s => s.DiseaseId == Convert.ToInt64(item));
                        if (fod != null && !speciality.Contains(fod.SpecialityId))
                        {
                            speciality.Add(Convert.ToInt64(fod.SpecialityId));
                        }
                    }
                }

                //Filter with Speciality
                if (speciality.Any())
                {
                    var filterDoctorData = new List<Doctor>();

                    foreach (var item in speciality)
                    {
                        var fod = doctors.FirstOrDefault(s => s.SpecialityId == item);
                        if (fod != null)
                        {
                            filterDoctorData.Add(fod);
                        }
                    }

                    doctors = filterDoctorData;
                }

                //Filter the data for OFFDAY
                var startDateTime = DateTime.Today; //Today at 00:00:00
                var endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
                var offdays = _dbEntities.Offdays.Where(s => s.OffDate >= startDateTime && s.OffDate <= endDateTime).ToList();
                if (offdays.Any())
                {
                    foreach (var item in offdays)
                    {
                        var removeDoctor = doctors.FirstOrDefault(s => s.DoctorId == item.DoctorId);
                        if (removeDoctor != null)
                        {
                            doctors.Remove(removeDoctor);
                        }
                    }
                }

                var homeVm = new HomeVm
                {
                    MapCollection = new List<MapVm>(),
                    IsFocus = true
                };

                if (data.SpeacialityIds != null && data.SpeacialityIds.Any())
                {
                    homeVm.SpeacialityIds = data.SpeacialityIds;
                }

                if (data.DiseasesIds != null && data.DiseasesIds.Any())
                {
                    homeVm.DiseasesIds = data.DiseasesIds;
                }

                foreach (var item in doctors)
                {
                    var latitude = Convert.ToDouble(item.Latitude);
                    var longitude = Convert.ToDouble(item.Longitude);

                    homeVm.MapCollection.Add(new MapVm
                    {
                        DoctorId = item.DoctorId,
                        Name = item.FullName,
                        Speciality = item.Speciality.Name,
                        Lat = latitude,
                        Long = longitude
                    });
                }

                return View(homeVm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult Authentication()
        {
            RegenerateTempData();
            return View(new UserMaster());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserMaster data)
        {
            try
            {
                var user = _dbEntities.UserMasters.FirstOrDefault(s => s.Email == data.Email && s.Password == data.Password && !s.IsDelete);

                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        TempData["Error"] = "Your account was blocked by admin";
                        return RedirectToAction("Authentication");
                    }

                    if (user.RoleMasterId == (int)EnumList.Roles.Patient)
                    {
                        SignInRemember(data.Email, true);
                        CookieHelper.Set(StaticValues.SessionUserId, user.UserMasterId.ToString(), true, 365);
                        CookieHelper.Set(StaticValues.SessionFullName, user.FullName, true, 365);
                        CookieHelper.Set(StaticValues.SessionProfileImg, user.ProfileImg, true, 365);
                        CookieHelper.Set(StaticValues.SessionRoleId, user.RoleMasterId.ToString(), true, 365);

                        TempData["Success"] = "Welcome to patient panel";
                        return RedirectToAction("Index", "Patient");
                    }

                    TempData["Error"] = "You are not authorize to access this panel";
                    return RedirectToAction("Authentication");
                }

                TempData["Error"] = "Wrong credentials found";
                return RedirectToAction("Authentication");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(UserMaster data)
        {
            try
            {
                using (_dbEntities)
                {
                    data.RoleMasterId = (int)EnumList.Roles.Patient;
                    data.CreatedDate = DateTime.Now;
                    data.IsActive = true;
                    data.IsDelete = false;
                    data.IsFacebookLogin = false;
                    _dbEntities.UserMasters.Add(data);
                    _dbEntities.SaveChanges();

                    TempData["Success"] = "Registration done successfully";
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
        public ActionResult ForgotPassword(LoginVm data, string userType)
        {
            try
            {
                using (_dbEntities)
                {
                    long roleId;
                    string password, name;

                    if (userType == "2")
                    {
                        var doctor = _dbEntities.Doctors.FirstOrDefault(s => s.Email == data.Email);
                        if (doctor != null)
                        {
                            roleId = doctor.RoleMasterId;
                            password = doctor.Password;
                            name = doctor.FullName;
                        }
                        else
                        {
                            TempData["Error"] = "This email is not found in our system";
                            return View(data);
                        }
                    }
                    else
                    {
                        var userMaster = _dbEntities.UserMasters.FirstOrDefault(s => s.Email == data.Email);
                        if (userMaster != null)
                        {
                            roleId = userMaster.RoleMasterId;
                            password = userMaster.Password;
                            name = userMaster.FullName;
                        }
                        else
                        {
                            TempData["Error"] = "This email is not found in our system";
                            return View(data);
                        }
                    }

                    if (Request.Url != null)
                    {
                        var loginUrl = Url.Action("Authentication", roleId == (int)EnumList.Roles.Doctor ? "Doctor" : "Home", "", Request.Url.Scheme);

                        var replacements = new Dictionary<string, string>
                            {
                                { "#Password#", password},
                                { "#login#", loginUrl},
                                { "#name#", name}
                            };

                        SendgridEmailHelpers.SendTempleteEmail((int)EnumList.EmailTemplete.ForgotPassword, data.Email, name, replacements);
                    }

                    TempData["Success"] = "We have send the login information in your mailbox";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult Contact()
        {
            try
            {
                RegenerateTempData();
                return View(new Enquiry());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(Enquiry data)
        {
            try
            {
                using (_dbEntities)
                {
                    data.CreatedDate = DateTime.Now;
                    _dbEntities.Enquiries.Add(data);
                    _dbEntities.SaveChanges();

                    TempData["Success"] = "Your enquiry information saved successfully! Thank you";

                    return RedirectToAction("Contact");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult ChangePassword()
        {
            RegenerateTempData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string Password)
        {
            try
            {
                using (_dbEntities)
                {
                    var userId = CookieHelper.Get(StaticValues.SessionUserId);
                    var userRoleId = Convert.ToInt16(CookieHelper.Get(StaticValues.SessionRoleId));
                    var uId = Convert.ToInt64(userId);

                    if (userRoleId == (int)EnumList.Roles.Doctor)
                    {
                        var data = _dbEntities.Doctors.Find(uId);
                        if (data != null)
                        {
                            data.Password = Password;
                            _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            _dbEntities.SaveChanges();

                            TempData["Success"] = "Password updated successfully.";
                        }
                        else
                        {
                            TempData["Error"] = "User details not found";
                        }
                    }
                    else
                    {
                        var data = _dbEntities.UserMasters.Find(uId);
                        if (data != null)
                        {
                            data.Password = Password;
                            _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            _dbEntities.SaveChanges();

                            TempData["Success"] = "Password updated successfully.";
                        }
                        else
                        {
                            TempData["Error"] = "User details not found";
                        }
                    }

                    return RedirectToAction("Index");
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