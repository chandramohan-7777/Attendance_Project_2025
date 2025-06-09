using iva.Controllers;
using iva.Data;
using iva.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using RCC;
using System.Globalization;

namespace IvaAttandance.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }
        
        [Route("att/Test")]
        public IActionResult Test()
        {
            var cDate = DateTime.Now;
            var present = 0;
            var absent = 0;
            var getTotalEmp = _db.employee.Select(s => new { s.rfid, s.employeeId, s.employeeName, s.employeeCode, s.department }).ToList();
            var list = new List<LogListViewModel>();
            foreach (var d in getTotalEmp)
            {
                var obj = new LogListViewModel();
                var l = (from s in _db.log where s.card == d.rfid && s.ts.Date == cDate.Date select s).FirstOrDefault();
                obj.employeeName = d.employeeName;
                obj.employeeCode = d.employeeCode;
                obj.department = d.department;
                if (l != null)
                {
                    var o = (from s in _db.log where s.card == d.rfid && s.ts.Date == cDate.Date select s).OrderByDescending(o => o.id).FirstOrDefault();
                    present = present + 1;
                    obj.status = 1;
                    obj.empIn = l.ts;
                    obj.empOut = o.ts;
                }
                else
                {
                    obj.status = 0;
                }
                list.Add(obj);
            }
            ViewBag.totalEmp = getTotalEmp.Count;
            ViewBag.present = present;
            ViewBag.absent = getTotalEmp.Count - present;
            ViewBag.logList = list;
            return View();
        }






        //[Route("dashboard")]
        //public IActionResult List()
        //{
        //    var username = HttpContext.Session.GetString("username");
        //    ViewBag.Username = username;
        //    if (string.IsNullOrEmpty(username))
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    var officeId = HttpContext.Session.GetInt32("companyId");



        //    var cDate = DateTime.Now;
        //    var present = 0;
        //    var absent = 0;
        //    var getTotalEmp = _db.employee
        //        .Where(s => s.IsActive == true && s.companyId == officeId)
        //        .Select(s => new { s.rfid, s.employeeId, s.employeeName, s.employeeCode, s.department })
        //        .ToList();
        //    var activeInternCount = _db.intern
        //        .Where(w => w.IsActive == true && w.companyId == officeId)
        //        .Select(s => new { s.name, s.rfid, s.purpose, s.IsActive, s.department, s.internCode })
        //        .ToList();
        //    var list = new List<LogListViewModel>();
        //    foreach (var d in getTotalEmp)
        //    {
        //        var obj = new LogListViewModel();
        //        var l = (from s in _db.log
        //                 where s.card == d.rfid && s.ts.Date == cDate.Date
        //                 select s).FirstOrDefault();
        //        obj.employeeName = d.employeeName;
        //        obj.employeeCode = d.employeeCode;
        //        obj.department = d.department;
        //        if (l != null)
        //        {
        //            var o = (from s in _db.log where s.card == d.rfid && s.ts.Date == cDate.Date select s).OrderByDescending(o => o.id).FirstOrDefault();
        //            present = present + 1;
        //            obj.status = 1;
        //            obj.empIn = l.ts;
        //            obj.empOut = o.ts;
        //        }
        //        else
        //        {
        //            obj.status = 0;
        //        }
        //        list.Add(obj);
        //    }
        //    foreach (var d in activeInternCount)
        //    {
        //        var obj = new LogListViewModel();
        //        var l = (from s in _db.log
        //                 where s.card == d.rfid && s.ts.Date == cDate.Date

        //                 select s).FirstOrDefault();
        //        obj.employeeName = d.name;
        //        obj.employeeCode = d.internCode;
        //        obj.department = d.department;
        //        if (l != null)
        //        {
        //            var o = (from s in _db.log where s.card == d.rfid && s.ts.Date == cDate.Date select s).OrderByDescending(o => o.id).FirstOrDefault();
        //            present = present + 1;
        //            obj.status = 1;
        //            obj.empIn = l.ts;
        //            obj.empOut = o.ts;
        //        }
        //        else
        //        {
        //            obj.status = 0;
        //        }
        //        list.Add(obj);
        //    }
        //    ViewBag.totalIntern = activeInternCount.Count;
        //    ViewBag.totalEmp = getTotalEmp.Count;
        //    ViewBag.present = present;
        //    ViewBag.absent = (getTotalEmp.Count + activeInternCount.Count) - present;
        //    ViewBag.logList = list;
        //    return View();
        //}


        [Route("dashboard")]
        public IActionResult List()
        {
            // Authentication check
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var officeId = HttpContext.Session.GetInt32("companyId");
            if (officeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentDate = DateTime.Now.Date;

            // Get all active employees and interns
            var employees = _db.employee
                .Where(e => e.IsActive && e.companyId == officeId)
                .ToList();

            var interns = _db.intern
                .Where(i => i.IsActive && i.companyId == officeId)
                .ToList();

            // Get all logs for today
            var todayLogs = _db.log
                .Where(l => l.ts.Date == currentDate)
                .GroupBy(l => l.card)
                .ToDictionary(g => g.Key, g => new {
                    FirstLog = g.OrderBy(l => l.ts).FirstOrDefault(),
                    LastLog = g.OrderByDescending(l => l.ts).FirstOrDefault()
                });

            // Process data for ViewBag
            var logList = new List<object>();
            int presentCount = 0;

            // Process employees
            foreach (var emp in employees)
            {
                var logData = todayLogs.ContainsKey(emp.rfid) ? todayLogs[emp.rfid] : null;
                var isPresent = logData != null;

                logList.Add(new
                {
                    employeeCode = emp.employeeCode,
                    employeeName = emp.employeeName,
                    department = emp.department,
                    inTime = isPresent ? FormatTime(logData.FirstLog.ts) : "-",
                    outTime = isPresent ? (logData.FirstLog.ts == logData.LastLog.ts ? "-" : FormatTime(logData.LastLog.ts)) : "-",
                    status = isPresent ? "Present" : "Absent",
                    statusClass = isPresent ? "success" : "danger"
                });

                if (isPresent) presentCount++;
            }

            // Process interns
            foreach (var intern in interns)
            {
                var logData = todayLogs.ContainsKey(intern.rfid) ? todayLogs[intern.rfid] : null;
                var isPresent = logData != null;

                logList.Add(new
                {
                    employeeCode = intern.internCode,
                    employeeName = intern.name,
                    department = intern.department,
                    inTime = isPresent ? FormatTime(logData.FirstLog.ts) : "-",
                    outTime = isPresent ? (logData.FirstLog.ts == logData.LastLog.ts ? "-" : FormatTime(logData.LastLog.ts)) : "-",
                    status = isPresent ? "Present" : "Absent",
                    statusClass = isPresent ? "success" : "danger"
                });

                if (isPresent) presentCount++;
            }

            // Set ViewBag values
            ViewBag.totalEmp = employees.Count;
            ViewBag.totalIntern = interns.Count;
            ViewBag.present = presentCount;
            ViewBag.absent = (employees.Count + interns.Count) - presentCount;
            ViewBag.logList = logList;

            return View();
        }

        private string FormatTime(DateTime dateTime)
        {
            return dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

    }
}




