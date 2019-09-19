using AyurvedOnCall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AyurvedOnCall.Models.ViewModels;
using System.Web.Mvc;

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
    }
}