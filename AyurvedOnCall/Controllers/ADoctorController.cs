using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AyurvedOnCall.Controllers
{
    [CheckAdminAuthorization]
    public class ADoctorController : Controller
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

                    var doctors = _dbEntities.Doctors.Where(s => !s.IsDelete).Select(s => new DoctorVm
                    {
                        City = s.City,
                        CreatedDate = s.CreatedDate,
                        Degree = s.Degree,
                        DoctorId = s.DoctorId,
                        DoctoringStartDate = s.DoctoringStartDate,
                        Email = s.Email,
                        FullName = s.FullName,
                        GenderId = s.GenderId,
                        IsActive = s.IsActive,
                        IsDelete = s.IsDelete,
                        ProfileImg = s.ProfileImg,
                        Mobile = s.Mobile,
                        Speciality = s.Speciality.Name,
                        University = s.University
                    }).ToList();

                    return View(doctors);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult Add()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();
                    return View(new Doctor());
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
        public ActionResult Add(Doctor data)
        {
            try
            {
                using (_dbEntities)
                {

                    var isEmailExist = _dbEntities.Doctors.FirstOrDefault(s => s.Email == data.Email);
                    if (isEmailExist != null)
                    {
                        TempData["Error"] = "Email (" + data.Email + ") already exists in our system";
                        return View(data);
                    }

                    var rnd = new Random();
                    var pass = rnd.Next(0, 999999).ToString("D6");

                    data.RoleMasterId = (int)EnumList.Roles.Doctor;
                    data.Password = pass;
                    data.CreatedDate = DateTime.UtcNow;
                    data.IsActive = true;
                    data.IsDelete = false;
                    _dbEntities.Doctors.Add(data);
                    _dbEntities.SaveChanges();

                    //Send a mail to doctor
                    if (Request.Url != null)
                    {
                        var loginUrl = Url.Action("Index", "Admin", "", protocol: Request.Url.Scheme);

                        var replacements = new Dictionary<string, string>
                        {
                            { "#Password#", data.Password},
                            { "#login#", loginUrl},
                            { "#name#", data.FullName}
                        };

                        SendgridEmailHelpers.SendTempleteEmail((int)EnumList.EmailTemplete.DoctorRegistration, data.Email, data.FullName, replacements);
                    }

                    TempData["Success"] = "Doctor added successfully";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult Edit(long id)
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();
                    var data = _dbEntities.Doctors.Find(id);
                    return View(data);
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
        public ActionResult Edit(Doctor data)
        {
            try
            {
                using (_dbEntities)
                {
                    var doctor = _dbEntities.Doctors.Find(data.DoctorId);

                    if (doctor != null)
                    {
                        doctor.SpecialityId = data.SpecialityId;
                        doctor.FullName = data.FullName;
                        doctor.Mobile = data.Mobile;
                        doctor.City = data.City;
                        doctor.Address = data.Address;
                        doctor.Latitude = data.Latitude;
                        doctor.Longitude = data.Longitude;
                        doctor.DoctoringStartDate = data.DoctoringStartDate;
                        doctor.GenderId = data.GenderId;
                        doctor.Degree = data.Degree;
                        doctor.University = data.University;
                        doctor.UpdatedDate = DateTime.UtcNow;

                        _dbEntities.Entry(doctor).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();
                        TempData["Success"] = "Doctor details updated successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Doctor details not found";
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

        public ActionResult Delete(long id)
        {
            try
            {
                using (_dbEntities)
                {

                    var data = _dbEntities.Doctors.Find(id);

                    if (data != null)
                    {
                        data.IsDelete = true;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        TempData["Success"] = "Doctor deleted successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Doctor details not found";
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

        public ActionResult ActiveDeactive(long id)
        {
            try
            {
                using (_dbEntities)
                {
                    var data = _dbEntities.Doctors.Find(id);

                    if (data != null)
                    {

                        var message = data.IsActive ? "Doctor deactivated successfully" : "Doctor activated successfully";

                        data.IsActive = !data.IsActive;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        TempData["Success"] = message;
                    }
                    else
                    {
                        TempData["Error"] = "Doctor details not found";
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