using System;
using System.Collections.Generic;
using System.Linq;
using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;



namespace iva.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _db;
        public AttendanceController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        [Route("attendance")]
        public IActionResult List()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var dates = GenerateDateRange(DateTime.Now);
            ViewBag.list = GetAttendanceResults(dates);
            return View();
        }

        private List<dList> GenerateDateRange(DateTime referenceDate)
        {
            var list = new List<dList>();
            DateTime firstDayOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                list.Add(new dList
                {
                    date = date,
                    dayOfWeek = date.DayOfWeek.ToString(),
                    day = date.Day
                });
            }

            return list;
        }

        private List<attendanceList> GetAttendanceResults(List<dList> dateList)
        {
            var officeId = HttpContext.Session.GetInt32("companyId");

            // Fetch data upfront
            var employees = _db.employee
                               .Where(s => s.companyId == officeId)
                               .Select(s => new { s.rfid, s.employeeCode, s.employeeName }).ToList();
            var dateSet = dateList.Select(d => d.date).ToHashSet();
            var logs = _db.log
                .Where(l => dateSet.Contains(l.ts.Date))
                .ToList();

            // Group logs by employee RFID and date
            var groupedLogs = logs
                .GroupBy(l => new { l.card, Date = l.ts.Date })
                .ToDictionary(g => g.Key, g => g.ToList());

            var holidayDates = _db.official_holiday
                          .Where(s => s.companyId == officeId) // ✅ Filter first
                          .Select(s => s.holiday_date.Date)     // ✅ Then select dates
                          .ToHashSet();                        // ✅ Convert to HashSet for fast lookup


            var result = new List<attendanceList>();

            foreach (var emp in employees)
            {
                var attendance = new attendanceList
                {
                    rfid = emp.rfid,
                    employeeCode = emp.employeeCode,
                    employeeName = emp.employeeName,
                    dList = new List<dateList>()
                };

                foreach (var d in dateList)
                {
                    var key = new { card = emp.rfid, Date = d.date };
                    groupedLogs.TryGetValue(key, out var logsForDate);

                    var firstLog = logsForDate?.FirstOrDefault();
                    var lastLog = logsForDate?.LastOrDefault();

                    attendance.dList.Add(new dateList
                    {
                        date = d.date,
                        dType = IsWeekend(d.date) ? "weekend" : "weekday",
                        workingDayType = holidayDates.Contains(d.date) ? "Holiday" : "WorkingDay",
                        status = firstLog != null,
                        empIn = firstLog?.ts ?? DateTime.MinValue,
                        empOut = lastLog?.ts ?? DateTime.MinValue,

                       
                    });
                }

                result.Add(attendance);
            }

            return result;
        }


        private static bool IsWeekend(DateTime date) =>
            date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;



        [Route("att/getFilterDates")]
        public JsonResult GetFilterDates(filterDates obj)
        {
            try
            {
                DateTime sdate = Convert.ToDateTime(obj.fromDate);
                DateTime edate = Convert.ToDateTime(obj.toDate);
                var dates = getDatesBetween(sdate, edate);
                var res = GetAttendanceResults(dates);
                return Json(new { success = true, list = res });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private List<dList> getDatesBetween(DateTime startDate, DateTime endDate)
        {
            var dates = new List<dList>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dates.Add(new dList
                {
                    date = date,
                    dayOfWeek = date.DayOfWeek.ToString(),
                    day = date.Day
                });
            }

            return dates;
        }

        [Route("Report")]
        public IActionResult Report()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var dates = GenerateDateRange(DateTime.Now);
            ViewBag.list = getReportResult(dates);
            return View();
        }

        private List<reportList> getReportResult(List<dList> dateList)
        {
            var officeId = HttpContext.Session.GetInt32("companyId");

            var employees = _db.employee
                .Where(s => s.companyId == officeId && s.IsActive == true)
                .Select(s => new { s.rfid, s.employeeCode, s.employeeName }).ToList();
            var logs = _db.log
                .Where(l => dateList.Select(d => d.date).Contains(l.ts.Date))
                .ToList();
            var scanners = _db.scanner.ToDictionary(s => s.dev_code, s => s.type);

            var holidayDates = _db.official_holiday
                         .Where(s => s.companyId == officeId) // ✅ Filter first
                         .Select(s => s.holiday_date.Date)     // ✅ Then select dates
                         .ToHashSet();                        // ✅ Convert to HashSet for fast lookup


            var result = employees.AsParallel().Select(emp =>
            {
                var report = new reportList
                {
                    rfid = emp.rfid,
                    employeeCode = emp.employeeCode,
                    employeeName = emp.employeeName,
                    dList = new List<dateList>()
                };

                foreach (var d in dateList)
                {
                    var logsForDate = logs
                        .Where(l => l.card == emp.rfid && l.ts.Date == d.date)
                        .OrderBy(l => l.ts)
                        .ToList();

                    int totalBreakingMinutes = 0;
                    DateTime? inTime = null;
                    DateTime? outTime = null;

                    foreach (var entry in logsForDate)
                    {
                        if (scanners.TryGetValue(entry.dev, out var devType))
                        {
                            if (devType == 1) inTime = entry.ts;
                            if (devType == 2) outTime = entry.ts;
                        }

                        if (inTime.HasValue && outTime.HasValue && inTime.Value < outTime.Value)
                        {
                            totalBreakingMinutes += (int)(outTime.Value - inTime.Value).TotalMinutes;
                            inTime = null;
                            outTime = null;
                        }
                    }

                    string totalBreakingHours = $"{totalBreakingMinutes / 60} hrs {totalBreakingMinutes % 60} mins";

                    var firstLog = logsForDate.FirstOrDefault();
                    var lastLog = logsForDate.LastOrDefault();

                    report.dList.Add(new dateList
                    {
                        date = d.date,
                        dType = IsWeekend(d.date) ? "weekend" : "weekday",
                        workingDayType = holidayDates.Contains(d.date) ? "Holiday" : "WorkingDay",
                        dayOfWeek = d.dayOfWeek,
                        day = d.day,
                        status = firstLog != null,
                        empIn = firstLog?.ts ?? DateTime.MinValue, 
                        empOut = lastLog?.ts ?? DateTime.MinValue,
                        totalBreakingHours = totalBreakingHours
                    });
                }

                return report;
            }).OrderBy(r => r.employeeCode).ToList();

            return result;
        }



        [Route("api/att/GetMonthlyReport")]
        [HttpPost]
        public JsonResult GetMonthlyReport(int month, int year)
        {
            try
            {
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var dates = getDatesBetween(firstDayOfMonth, lastDayOfMonth);
                var result = getReportResult(dates);
                return Json(new { success = true, list = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string CalculateHours(DateTime empIn, DateTime empOut)
        {
            var timeDifference = empOut - empIn;
            int totalMinutes = (int)timeDifference.TotalMinutes;
            int totalHours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{totalHours} hrs {minutes} mins";
        }


        [Route("api/att/GetMonthlyReportExcel")]
        [HttpPost]
        public IActionResult GetMonthlyReportExcel(int month, int year)
        {
            try
            {
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var dates = getDatesBetween(firstDayOfMonth, lastDayOfMonth);
                var result = getReportResult(dates);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Attendance Report");
                    int startRow = 1;
                    var headerRange = worksheet.Range(startRow, 1, startRow, 16);
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRange.Style.Font.Bold = true;
                    foreach (var emp in result)
                    {
                        int present = 0, absent = 0, totDays = 0, wDays = 0;
                        foreach (var record in emp.dList)
                        {
                            if (record.dType == "weekend" && record.status)
                            {
                                wDays++;
                            }
                            else if (record.dType != "weekend")
                            {
                                totDays++;
                                if (record.status) wDays++;
                            }
                        }

                        present = emp.dList.Count(r => r.status);
                        absent = totDays - present;

                        // Add employee details
                        int startCol = 1;
                        worksheet.Cell(startRow, startCol).Value = "Employee Code:";
                        worksheet.Cell(startRow, startCol + 1).Value = emp.employeeCode;
                        worksheet.Cell(startRow, startCol + 3).Value = "Employee Name:";
                        worksheet.Cell(startRow, startCol + 4).Value = emp.employeeName;
                        worksheet.Cell(startRow, startCol + 6).Value = "Employee Total Present:";
                        worksheet.Cell(startRow, startCol + 7).Value = present;
                        worksheet.Cell(startRow, startCol + 9).Value = "Employee Total Absent:";
                        worksheet.Cell(startRow, startCol + 10).Value = absent;
                        worksheet.Cell(startRow, startCol + 12).Value = "Working Days:";
                        worksheet.Cell(startRow, startCol + 13).Value = $"{wDays}/ {totDays}";
                        worksheet.Cell(startRow, startCol + 15).Value = "Company Name:";
                        worksheet.Cell(startRow, startCol + 16).Value = "IVA TECHNOS";

                        // Header Styling
                        worksheet.Range(startRow, startCol, startRow, startCol + 16).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        worksheet.Range(startRow, startCol, startRow, startCol + 16).Style.Font.Bold = true;

                        startRow += 2;

                        // Add dates and attendance records
                        foreach (var date in emp.dList)
                        {
                            worksheet.Cell(startRow, startCol + date.day).Value = date.date.ToString("dd-MM-yyyy");
                            worksheet.Cell(startRow, startCol + date.day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        startRow++;

                        foreach (var date in emp.dList)
                        {
                            worksheet.Cell(startRow, startCol + date.day).Value = date.dayOfWeek.ToString();
                            worksheet.Cell(startRow, startCol + date.day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        startRow++;
                        string[] subHeaders = { "IN", "OUT", "Total Hours", "Status", "Holiday Status" };
                        foreach (var subHeader in subHeaders)
                        {
                            worksheet.Cell(startRow, startCol).Value = subHeader;

                            for (int day = 1; day <= emp.dList.Count; day++)
                            {
                                var date = emp.dList.FirstOrDefault(d => d.day == day);
                                if (date != null)
                                {
                                    switch (subHeader)
                                    {
                                        case "IN":
                                            worksheet.Cell(startRow, startCol + day).Value = date.empIn == DateTime.MinValue
                                                ? "--:--"
                                                : date.empIn.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "OUT":
                                            worksheet.Cell(startRow, startCol + day).Value = date.empOut == DateTime.MinValue
                                                ? "--:--"
                                                : date.empOut.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Total Hours":
                                            worksheet.Cell(startRow, startCol + day).Value = date.empIn != DateTime.MinValue && date.empOut != DateTime.MinValue
                                                ? CalculateHours(date.empIn, date.empOut)
                                                : "--";
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Status":
                                            worksheet.Cell(startRow, startCol + day).Value = date.status ? "P" : (date.dType == "weekend" ? "-" : "A");
                                            worksheet.Cell(startRow, startCol + day).Style.Fill.BackgroundColor = date.status
                                                ? XLColor.LightGreen
                                                : (date.dType == "weekend" ? XLColor.LightGray : XLColor.LightCoral);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Holiday Status":
                                            worksheet.Cell(startRow, startCol + day).Value = date.workingDayType == "Holiday"
                                                ? "Holiday"
                                                : "-";
                                            worksheet.Cell(startRow, startCol + day).Style.Fill.BackgroundColor = date.workingDayType == "Holiday"
                                                ? XLColor.LightGreen
                                                : XLColor.Wheat;

                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;
                                    }
                                }
                            }
                            startRow++;
                        }

                        startRow += 2;
                    }
                    worksheet.Columns().AdjustToContents();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AttendanceReport_{monthName}_{year}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        [Route("InternReport")]
        public IActionResult InternReport()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var dates = GenerateDateRange(DateTime.Now);
            ViewBag.list = getInternReportResult(dates);
            return View();
        }

        private List<InternreportList> getInternReportResult(List<dList> dateList)
        {
            var officeId = HttpContext.Session.GetInt32("companyId");
            var interns = _db.intern
                .Where(s => s.companyId == officeId && s.IsActive == true)
                .Select(s => new { s.rfid, s.internCode, s.name }).ToList();
            var logs = _db.log
                .Where(l => dateList.Select(d => d.date).Contains(l.ts.Date))
                .ToList();

            var scanners = _db.scanner.ToDictionary(s => s.dev_code, s => s.type);

            var holidayDates = _db.official_holiday
                       .Where(s => s.companyId == officeId) // ✅ Filter first
                       .Select(s => s.holiday_date.Date)     // ✅ Then select dates
                       .ToHashSet();                        // ✅ Convert to HashSet for fast lookup
            var result = interns.AsParallel().Select(emp =>
            {
                var report = new InternreportList
                {
                    internCode = emp.internCode,
                    internName = emp.name,
                    dList = new List<dateInternList>()
                };

                foreach (var d in dateList)
                {
                    var logsForDate = logs
                        .Where(l => l.card == emp.rfid && l.ts.Date == d.date)
                        .OrderBy(l => l.ts)
                        .ToList();

                    int totalBreakingMinutes = 0;
                    DateTime? inTime = null;
                    DateTime? outTime = null;

                    foreach (var entry in logsForDate)
                    {
                        if (scanners.TryGetValue(entry.dev, out var devType))
                        {
                            if (devType == 1) inTime = entry.ts;
                            if (devType == 2) outTime = entry.ts;
                        }

                        if (inTime.HasValue && outTime.HasValue && inTime.Value < outTime.Value)
                        {
                            totalBreakingMinutes += (int)(outTime.Value - inTime.Value).TotalMinutes;
                            inTime = null;
                            outTime = null;
                        }
                    }

                    string totalBreakingHours = $"{totalBreakingMinutes / 60} hrs {totalBreakingMinutes % 60} mins";

                    var firstLog = logsForDate.FirstOrDefault();
                    var lastLog = logsForDate.LastOrDefault();

                    report.dList.Add(new dateInternList
                    {
                        date = d.date,
                        dType = IsWeekend(d.date) ? "weekend" : "weekday",
                        workingDayType = holidayDates.Contains(d.date) ? "Holiday" : "WorkingDay",
                        dayOfWeek = d.dayOfWeek,
                        day = d.day,
                        status = firstLog != null,
                        internIn = firstLog?.ts ?? DateTime.MinValue,
                        internOut = lastLog?.ts ?? DateTime.MinValue,
                        totalBreakingHours = totalBreakingHours
                    });
                }

                return report;
            }).OrderBy(r => r.internCode).ToList();

            return result;
        }

        [Route("api/att/GetMonthlyInternReportExcel")]
        [HttpPost]
        public IActionResult GetMonthlyInternReportExcel(int month, int year)
        {
            try
            {
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var dates = getDatesBetween(firstDayOfMonth, lastDayOfMonth);
                var result = getInternReportResult(dates);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Attendance Report");
                    int startRow = 1;
                    var headerRange = worksheet.Range(startRow, 1, startRow, 16);
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRange.Style.Font.Bold = true;
                    foreach (var emp in result)
                    {
                        int present = 0, absent = 0, totDays = 0, wDays = 0;
                        foreach (var record in emp.dList)
                        {
                            if (record.dType == "weekend" && record.status)
                            {
                                wDays++;
                            }
                            else if (record.dType != "weekend")
                            {
                                totDays++;
                                if (record.status) wDays++;
                            }
                        }

                        present = emp.dList.Count(r => r.status);
                        absent = totDays - present;

                        // Add employee details
                        int startCol = 1;
                        worksheet.Cell(startRow, startCol).Value = "Intern Code:";
                        worksheet.Cell(startRow, startCol + 1).Value = emp.internCode;
                        worksheet.Cell(startRow, startCol + 3).Value = "Intern Name:";
                        worksheet.Cell(startRow, startCol + 4).Value = emp.internName;
                        worksheet.Cell(startRow, startCol + 6).Value = "Intern Total Present:";
                        worksheet.Cell(startRow, startCol + 7).Value = present;
                        worksheet.Cell(startRow, startCol + 9).Value = "Intern Total Absent:";
                        worksheet.Cell(startRow, startCol + 10).Value = absent;
                        worksheet.Cell(startRow, startCol + 12).Value = "Working Days:";
                        worksheet.Cell(startRow, startCol + 13).Value = $"{wDays}/ {totDays}";
                        worksheet.Cell(startRow, startCol + 15).Value = "Company Name:";
                        worksheet.Cell(startRow, startCol + 16).Value = "IVA TECHNOS";

                        // Header Styling
                        worksheet.Range(startRow, startCol, startRow, startCol + 16).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        worksheet.Range(startRow, startCol, startRow, startCol + 16).Style.Font.Bold = true;

                        startRow += 2;

                        // Add dates and attendance records
                        foreach (var date in emp.dList)
                        {
                            worksheet.Cell(startRow, startCol + date.day).Value = date.date.ToString("dd-MM-yyyy");
                            worksheet.Cell(startRow, startCol + date.day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        startRow++;

                        foreach (var date in emp.dList)
                        {
                            worksheet.Cell(startRow, startCol + date.day).Value = date.dayOfWeek.ToString();
                            worksheet.Cell(startRow, startCol + date.day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        startRow++;
                        string[] subHeaders = { "IN", "OUT", "Total Hours", "Status", "Holiday Status" };
                        foreach (var subHeader in subHeaders)
                        {
                            worksheet.Cell(startRow, startCol).Value = subHeader;

                            for (int day = 1; day <= emp.dList.Count; day++)
                            {
                                var date = emp.dList.FirstOrDefault(d => d.day == day);
                                if (date != null)
                                {
                                    switch (subHeader)
                                    {
                                        case "IN":
                                            worksheet.Cell(startRow, startCol + day).Value = date.internIn == DateTime.MinValue
                                                ? "--:--"
                                                : date.internIn.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "OUT":
                                            worksheet.Cell(startRow, startCol + day).Value = date.internOut == DateTime.MinValue
                                                ? "--:--"
                                                : date.internOut.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Total Hours":
                                            worksheet.Cell(startRow, startCol + day).Value = date.internIn != DateTime.MinValue && date.internOut != DateTime.MinValue
                                                ? CalculateHours(date.internIn, date.internOut)
                                                : "--";
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Status":
                                            worksheet.Cell(startRow, startCol + day).Value = date.status ? "P" : (date.dType == "weekend" ? "-" : "A");
                                            worksheet.Cell(startRow, startCol + day).Style.Fill.BackgroundColor = date.status
                                                ? XLColor.LightGreen
                                                : (date.dType == "weekend" ? XLColor.LightGray : XLColor.LightCoral);
                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                        case "Holiday Status":
                                            worksheet.Cell(startRow, startCol + day).Value = date.workingDayType == "Holiday"
                                                ? "Holiday"
                                                : "-";
                                            worksheet.Cell(startRow, startCol + day).Style.Fill.BackgroundColor = date.workingDayType == "Holiday"
                                                ? XLColor.LightGreen
                                                : XLColor.Wheat;

                                            worksheet.Cell(startRow, startCol + day).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            break;

                                    }
                                }
                            }
                            startRow++;
                        }
                        startRow += 2;
                    }
                    worksheet.Columns().AdjustToContents();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AttendanceReport_{monthName}_{year}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [Route("api/att/GetMonthlyInternReport")]
        [HttpPost]
        public JsonResult GetMonthlyInternReport(int month, int year)
        {
            try
            {
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var dates = getDatesBetween(firstDayOfMonth, lastDayOfMonth);
                var result = getInternReportResult(dates);
                return Json(new { success = true, list = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [Route("today")]
        public IActionResult Today()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var officeId = HttpContext.Session.GetInt32("companyId");

            var cDate = DateTime.Now;
            var present = 0;
            var absent = 0;
            var getTotalEmp = _db.employee
                .Where(s => s.IsActive == true && s.companyId == officeId)
                .Select(s => new { s.rfid, s.employeeId, s.employeeName, s.employeeCode, s.department })
                .ToList();
            var activeInternCount = _db.intern
                .Where(w => w.IsActive == true && w.companyId == officeId)
                .Select(s => new { s.name, s.rfid, s.purpose, s.IsActive, s.department, s.internCode })
                .ToList();
            var list = new List<LogListViewModel>();
            foreach (var d in getTotalEmp)
            {
                var obj = new LogListViewModel();
                var l = (from s in _db.log
                         where s.card == d.rfid && s.ts.Date == cDate.Date
                         select s).FirstOrDefault();
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
            foreach (var d in activeInternCount)
            {
                var obj = new LogListViewModel();
                var l = (from s in _db.log
                         where s.card == d.rfid && s.ts.Date == cDate.Date
                         select s).FirstOrDefault();
                obj.employeeName = d.name;
                obj.employeeCode = d.internCode;
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
            ViewBag.totalIntern = activeInternCount.Count;
            ViewBag.totalEmp = getTotalEmp.Count;
            ViewBag.present = present;
            ViewBag.absent = (getTotalEmp.Count + activeInternCount.Count) - present;
            ViewBag.logList = list;
            return View();
        }



        [Route("ManualUpdation")]
        public IActionResult ManualUpdation()
        {
            return View();
        }

        [Route("api/att/GetCompanyDevices")]

        [HttpGet]
        public JsonResult GetCompanyDevices()
        {
            var officeId = HttpContext.Session.GetInt32("companyId");


            var devices = _db.company_device
                               .Where(s => s.companyId == officeId)
                               .Select(s => new { s.device_code }).ToList();
            return Json(devices);
        }


        [Route("api/att/ManualEntryInTime")]
        [HttpPost]
        public IActionResult ManualEntryInTime([FromBody] ManualEntryInRequest request)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrEmpty(request.devCode) || string.IsNullOrEmpty(request.RFID) || request.InTime == null)
                {
                    return Json(new { success = false, message = "Missing required data: devCode, RFID, or InTime." });
                }
                bool checkEntry = _db.log.Any(s => s.card == request.RFID && s.ts == request.InTime);
                if (checkEntry)
                {
                    return Json(new { success = false, message = "InTime entry has already been recorded." });
                }

                int lastIdx = (_db.log.OrderByDescending(s => s.idx).Select(s => s.idx).FirstOrDefault()) + 1;

                // Create new log entry
                var entryIn = new log
                {
                    card = request.RFID,
                    ts = request.InTime,
                    dev = request.devCode,
                    idx = lastIdx
                };

                _db.log.Add(entryIn);
                _db.SaveChanges();

                return Json(new { success = true, message = "Manual entry In Time submitted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        [Route("api/att/ManualEntryOutTime")]
        [HttpPost]
        public IActionResult ManualEntryOutTime([FromBody] ManualEntryOutRequest request)

        {

            try
            {
                // Validate input data
                if (string.IsNullOrEmpty(request.devCode) || string.IsNullOrEmpty(request.RFID) || request.OutTime == null)
                {
                    return Json(new { success = false, message = "Missing required data: devCode, RFID, or InTime." });
                }
                bool checkEntry = _db.log.Any(s => s.card == request.RFID && s.ts.Date == request.OutTime.Date);
                if (checkEntry)
                {
                    int lastIdx = (_db.log.OrderByDescending(s => s.idx).Select(s => s.idx).FirstOrDefault()) + 1;
                    var entryOut = new log
                    {
                        card = request.RFID,
                        ts = request.OutTime,
                        dev = request.devCode,
                        idx = lastIdx
                    };

                    _db.log.Add(entryOut);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Manual entry Out Time submitted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "First In Time will provide an new entry after out time entered " });

                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }

        }



        [Route("api/att/ManualEntryInTimeOutTime")]
        [HttpPost]
        public IActionResult ManualEntryInTimeOutTime([FromBody] ManualEntryInOutRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            try
            {
                // Extracting values into separate variables
                string rfid = request.RFID;
                string devCode = request.devCode;
                string reason = request.reason;

                DateTime actualInTime = request.INTIME?.ActualInTime ?? DateTime.MinValue;
                DateTime inTime = request.INTIME?.InTime ?? DateTime.MinValue;

                DateTime actualOutTime = request.OUTTIME?.ActualOutTime ?? DateTime.MinValue;
                DateTime outTime = request.OUTTIME?.OuTime ?? DateTime.MinValue;

                var checkEntry = _db.log
                    .Where(s => s.card == rfid && (s.ts == inTime || s.ts == outTime))
                    .ToList();

                var retrieveEmployeeId = _db.employee
                    .FirstOrDefault(s => s.rfid == rfid);

                if (retrieveEmployeeId == null)
                {
                    return Json(new { success = false, message = "Employee not found for the provided RFID" });
                }

                var currentDateTime = DateTime.Now;
                var userId = HttpContext.Session.GetInt32("loginId");

                if (userId == null)
                {
                    return Json(new { success = false, message = "User session is invalid" });
                }

                if (checkEntry.Count == 0)
                {
                    // Add new log entries for InTime and OutTime
                    int lastInIdx = (_db.log.OrderByDescending(s => s.idx).Select(s => s.idx).FirstOrDefault()) + 1;

                    _db.log.Add(new log
                    {
                        card = rfid,
                        ts = inTime,
                        dev = devCode,
                        idx = lastInIdx
                    });

                    _db.log.Add(new log
                    {
                        card = rfid,
                        ts = outTime,
                        dev = devCode,
                        idx = lastInIdx + 1
                    });

                    _db.SaveChanges();

                    // Save manual entry details
                    SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualInTime, inTime, reason, "Added");
                    SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualOutTime, outTime, reason, "Added");

                    return Json(new { success = true, message = "InTime and OutTime added successfully" });
                }
                else
                {
                    var firstlog = _db.log.FirstOrDefault(s => s.ts == actualInTime);
                    var secondlog = _db.log.FirstOrDefault(s => s.ts == actualOutTime);

                    if (actualInTime != inTime && actualOutTime != outTime)
                    {
                        firstlog.ts = inTime;
                        secondlog.ts = outTime;
                        _db.SaveChanges();

                        SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualInTime, inTime, reason, "Updated");
                        SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualOutTime, outTime, reason, "Updated");

                        return Json(new { success = true, message = "InTime and OutTime updated successfully" });
                    }
                    else if (actualInTime != inTime && actualOutTime == outTime)
                    {
                        firstlog.ts = inTime;
                        _db.SaveChanges();

                        SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualInTime, inTime, reason, "Updated");

                        return Json(new { success = true, message = "InTime updated successfully" });
                    }
                    else if (actualInTime == inTime && actualOutTime != outTime)
                    {
                        secondlog.ts = outTime;
                        _db.SaveChanges();

                        SaveManualEntryDetails((int)userId, retrieveEmployeeId.employeeId, rfid, currentDateTime, actualOutTime, outTime, reason, "Updated");

                        return Json(new { success = true, message = "OutTime updated successfully" });
                    }

                }

                return Json(new { success = true, message = "No changes were made" });
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // Helper method to save manual entry details
        private void SaveManualEntryDetails(int userId, int employeeId, string rfid, DateTime currentDateTime, DateTime actualDateTime, DateTime updateDateTime, string reason, string status)
        {
            _db.manualEntryDetail.Add(new manualEntryDetail
            {
                user_id = userId,
                employee_Id = employeeId,
                employee_rfid = rfid,
                current_datetime = currentDateTime,
                actual_datetime = actualDateTime,
                update_datetime = updateDateTime,
                status = status,
                reason = reason
            });

            _db.SaveChanges();
        }











      

    }
}



