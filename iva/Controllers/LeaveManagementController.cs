using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Diagnostics;
using System;
namespace iva.Controllers
{
    public class LeaveManagementController : Controller
    {
        private readonly AppDbContext _db;



        public LeaveManagementController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        [Route("HolidayCalender")]
        public IActionResult HolidayCalender()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var officeId = HttpContext.Session.GetInt32("companyId");


            var getList = _db.official_holiday
            .Where(s => s.companyId == officeId)
            .Select(s => new official_holidayList
            {
                holidayDayOfWeek = s.holiday_date.DayOfWeek.ToString(),
                holidayDate = s.holiday_date.Date,
                holidayName = s.holiday_name,
                id = s.id,

            }).ToList();
            ViewBag.holidayList = getList;
            return View();

        }

        [HttpPost]
        [Route("att/AddHolidayDetail")]
        public JsonResult AddHolidayDetail(official_holidayList obj)
        {
            try
            {
                if (obj == null)
                {
                    return Json(new { success = false, message = "Invalid data received." });
                }

                if (obj.id == 0)
                {
                    var checkDate = _db.official_holiday.FirstOrDefault(s => s.holiday_date == obj.holidayDate);

                    if (checkDate != null)
                    {
                        return Json(new { success = false, message = "Holiday date already declared." });
                    }

                    var officeId = HttpContext.Session.GetInt32("companyId");

                    if (officeId == null)
                    {
                        return Json(new { success = false, message = "Company ID not found in session." });
                    }
                    var newHoliday = new official_holiday
                    {
                        holiday_name = obj.holidayName,
                        holiday_date = obj.holidayDate,
                        companyId = (int)officeId
                    };

                    _db.official_holiday.Add(newHoliday);
                    _db.SaveChanges();

                    var getList = _db.official_holiday
                   .Where(s => s.companyId == officeId)
                   .Select(s => new official_holidayList
                   {
                       holidayDayOfWeek = s.holiday_date.DayOfWeek.ToString(),
                       holidayDate = s.holiday_date.Date,
                       holidayName = s.holiday_name,
                       id = s.id,

                   }).ToList();

                    return Json(new { success = true, message = "Holiday added successfully.", list = getList });
                }

                return Json(new { success = false, message = "Invalid holiday ID." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }




        [HttpPost]
        [Route("att/UpdateHolidayDetail")]
        public JsonResult UpdateHolidayDetail(official_holidayList obj)
        {
            try
            {
                if (obj == null || obj.id <= 0)
                {
                    return Json(new { success = false, message = "Invalid holiday ID." });
                }

                var existingHoliday = _db.official_holiday.FirstOrDefault(s => s.id == obj.id);

                if (existingHoliday == null)
                {
                    return Json(new { success = false, message = "Holiday not found." });
                }
                var officeId = HttpContext.Session.GetInt32("companyId");

                // Update existing holiday details
                existingHoliday.holiday_name = obj.holidayName;
                existingHoliday.holiday_date = obj.holidayDate;
                existingHoliday.companyId = (int)officeId;
                _db.SaveChanges();

                var getList = _db.official_holiday
                .Where(s => s.companyId == officeId)
                .Select(s => new official_holidayList
                {
                    holidayDayOfWeek = s.holiday_date.DayOfWeek.ToString(),
                    holidayDate = s.holiday_date.Date,
                    holidayName = s.holiday_name,
                    id = s.id,
                }).ToList();
                return Json(new { success = true, message = "Holiday updated successfully.", list = getList });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }



        [HttpPost]
        [Route("att/DeleteHoliday")]
        public JsonResult DeleteHoliday(int holidayId)
        {
            try
            {
                var HolidayDate = _db.official_holiday.FirstOrDefault(s => s.id == holidayId);

                if (HolidayDate == null)
                {
                    return Json(new { success = false, message = "Holiday Date not found" });
                }

                _db.official_holiday.Remove(HolidayDate);
                _db.SaveChanges();

                var officeId = HttpContext.Session.GetInt32("companyId");

                var getList = _db.official_holiday
                   .Where(s => s.companyId == officeId)
                   .Select(s => new official_holidayList
                   {
                       holidayDayOfWeek = s.holiday_date.DayOfWeek.ToString(),
                       holidayDate = s.holiday_date.Date,
                       holidayName = s.holiday_name,
                       id = s.id,

                   }).ToList();
                return Json(new { success = true, list = getList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("att/GetHolidayDetail")]
        public JsonResult GetHolidayDetail(int holidayId)
        {
            try
            {
                var checkVisitor = (from s in _db.official_holiday where s.id == holidayId select s).FirstOrDefault();
                if (checkVisitor != null)
                {
                    return Json(new { success = true, list = checkVisitor });
                }
                else
                {
                    return Json(new { success = true, list = "visitor not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }



    }
}
