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

        [CheckDoctorAuthorization]
        public ActionResult Index()
        {
            RegenerateTempData();
            return View();
        }
    }
}