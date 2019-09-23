using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using AyurvedOnCall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AyurvedOnCall.Controllers
{
    public class APatientController : Controller
    {
        private readonly DBEntities _dbEntities = new DBEntities();

        public void RegenerateTempData()
        {
            if (TempData["Success"] != null)
                TempData["Success"] = TempData["Success"];

            if (TempData["Error"] != null)
                TempData["Error"] = TempData["Error"];
        }

        public ActionResult Index()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();

                    var patients = _dbEntities.UserMasters.Where(s => s.RoleMasterId == (int)EnumList.Roles.Patient && !s.IsDelete).ToList();


                    return View(patients);
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

                    var data = _dbEntities.UserMasters.Find(id);

                    if (data != null)
                    {
                        data.IsDelete = true;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        TempData["Success"] = "Patient deleted successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Patient details not found";
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
                    var data = _dbEntities.UserMasters.Find(id);

                    if (data != null)
                    {

                        var message = data.IsActive ? "Patient deactivated successfully" : "Patient activated successfully";

                        data.IsActive = !data.IsActive;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        TempData["Success"] = message;
                    }
                    else
                    {
                        TempData["Error"] = "Patient details not found";
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

        public ActionResult Appointments()
        {
            try
            {
                using (_dbEntities)
                {
                    RegenerateTempData();

                    var patients = _dbEntities.Appointments.Where(s => !s.IsDelete).Select(s => new AppointmentVm
                    {
                        Allergies = s.Allergies,
                        AppointmentDate = s.AppointmentDate,
                        AppointmentId = s.AppointmentId,
                        Complaint = s.Complaint,
                        CreatedDate = s.CreatedDate,
                        DateOfBirth = s.DateOfBirth,
                        DoctorName = s.Doctor.FullName,
                        Fullname = s.Fullname,
                        GenderId = s.GenderId,
                        IsActive = s.IsActive,
                        IsConfirm = s.IsConfirm,
                        IsRejected = s.IsRejected,
                        Mobile = s.Mobile
                    }).ToList();

                    return View(patients);
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