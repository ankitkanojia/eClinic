using AyurvedOnCall.Helpers;
using AyurvedOnCall.Models;
using System;
using System.Web.Mvc;

namespace AyurvedOnCall.Controllers
{
    [CheckAdminAuthorization]
    public class AChangePasswordController : Controller
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
            RegenerateTempData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Password)
        {
            try
            {
                using (_dbEntities)
                {
                    var adminId = CookieHelper.Get(StaticValues.SessionUserId);
                    var aId = Convert.ToInt64(adminId);
                    var data = _dbEntities.UserMasters.Find(aId);
                    if (data != null)
                    {
                        data.Password = Password;
                        _dbEntities.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _dbEntities.SaveChanges();

                        TempData["Success"] = "Password updated successfully.";
                    }
                    else
                    {
                        TempData["Error"] = "Admin details not found";
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