using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AyurvedOnCall.Controllers
{
    [CheckAdminAuthorization]
    public class AEnquiryController : Controller
    {
        private readonly DBEntities _dbEntities = new DBEntities();
        public ActionResult Index()
        {
            try
            {
                using (_dbEntities)
                {
                    var data = _dbEntities.Enquiries.ToList();
                    return View(data);
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