using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AyurvedOnCall.Models;
using Exception = System.Exception;

namespace AyurvedOnCall.Helpers
{
    public static class CommonFunctions
    {
        public static List<SelectListItem> GetDiseasesDropdownData()
        {
            try
            {
                using (var db = new DBEntities())
                {
                    var response = new List<SelectListItem>
                    {
                        new SelectListItem{Text = "Select Diseases", Value = string.Empty}
                    };

                    response.AddRange(db.Diseases.Select(s => new SelectListItem
                    {
                        Value = s.DiseaseId.ToString(),
                        Text = s.Name
                    }).ToList());

                    return response;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<SelectListItem> GetSpecialityDropdownData()
        {
            try
            {
                using (var db = new DBEntities())
                {
                    var response = new List<SelectListItem>
                    {
                        new SelectListItem{Text = "Select Speciality", Value = string.Empty}
                    };

                    response.AddRange(db.Specialities.Select(s=> new SelectListItem
                    {
                        Value = s.SpecialityId.ToString(),
                        Text = s.Name
                    }).ToList());

                    return response;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<SelectListItem> GetDoctorDropdownData()
        {
            try
            {
                using (var db = new DBEntities())
                {
                    var response = new List<SelectListItem>
                    {
                        new SelectListItem{Text = "Select Doctor", Value = string.Empty}
                    };

                    response.AddRange(db.Doctors.Where(s=>s.IsActive && !s.IsDelete).Select(s => new SelectListItem
                    {
                        Value = s.DoctorId.ToString(),
                        Text = s.FullName
                    }).ToList());

                    return response;
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