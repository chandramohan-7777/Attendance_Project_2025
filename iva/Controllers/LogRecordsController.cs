using ClosedXML.Excel;
using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static iva.Models.employeelogs;

namespace iva.Controllers
{
    public class LogRecordsController : Controller
    {
        private readonly AppDbContext _db;
        public LogRecordsController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }
        [Route("EmployeeLogRecords")]
        public IActionResult EmployeeLogRecords()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }

        [Route("InternLogRecords")]
        public IActionResult InternLogRecords()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [Route("GenerateEmployeeLogRecords")]
        public IActionResult GenerateEmployeeLogRecords()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }


            var officeId = HttpContext.Session.GetInt32("companyId");
            var companyName = _db.company
                .Where(x => x.id == officeId)
                .Select(x => x.companyName)
                .FirstOrDefault();
            ViewBag.CompanyName = companyName;
            return View();
        }
        [Route("GenerateInternLogRecords")]
        public IActionResult GenerateInternLogRecords()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var officeId = HttpContext.Session.GetInt32("companyId");
            var companyName = _db.company
                .Where(x => x.id == officeId)
                .Select(x => x.companyName)
                .FirstOrDefault();
            ViewBag.CompanyName = companyName;
            return View();
        }



        [Route("api/GetEmployeeLogRecords")]
        [HttpPost]
        public ActionResult<List<EmployeeLogRecodsList>> GetEmployeeLogRecords([FromBody] RetriveAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (string.IsNullOrWhiteSpace(retriveInfo.employeeRfid))
                {
                    return BadRequest("Employee RFID is required");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Preload all scanner devices into memory (single database query)
                var allScanners = _db.scanner.ToDictionary(x => x.dev_code, x => x.type);

                // Get date range
                var storedDateList = InBetweenDates(retriveInfo.startDate, retriveInfo.endDate);

                // Get employee info
                var employeeInfo = _db.employee
                    .FirstOrDefault(x => x.rfid == retriveInfo.employeeRfid);

                if (employeeInfo == null)
                {
                    return NotFound($"Employee with RFID {retriveInfo.employeeRfid} not found");
                }

                // Get all logs in one query
                var employeeLogs = _db.log
                    .Where(x => x.card == retriveInfo.employeeRfid &&
                               x.ts >= retriveInfo.startDate &&
                               x.ts <= retriveInfo.endDate.AddDays(1))
                    .OrderBy(x => x.ts)
                    .ToList();

                // Process logs with cached scanner data
                var processedLogs = employeeLogs
                    .Select(log => new employeelogs
                    {
                        logs = log.ts,
                        logType = GetDeviceTypeCached(log.dev, allScanners) // Uses cached data
                    })
                    .ToList();

                // Group by date
                var employeeLogRecordsLists = new List<EmployeeLogRecodsList>();
                foreach (var dateItem in storedDateList)
                {
                    var logsForDate = processedLogs
                        .Where(x => x.logs.Date == dateItem.date.Date)
                        .ToList();

                    employeeLogRecordsLists.Add(new EmployeeLogRecodsList
                    {
                        rfid = retriveInfo.employeeRfid,
                        date = dateItem.date,
                        day = dateItem.day.ToString(),
                        employeeCode = employeeInfo.employeeCode,
                        employeeName = employeeInfo.employeeName,
                        dayOfWeek = dateItem.dayOfWeek,
                        records = logsForDate
                    });
                }

                return Ok(employeeLogRecordsLists);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }


        [Route("api/GetInternLogRecords")]
        [HttpPost]
        public ActionResult<List<EmployeeLogRecodsList>> GetInternLogRecords([FromBody] RetriveInternAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (string.IsNullOrWhiteSpace(retriveInfo.internRfid))
                {
                    return BadRequest("Intern RFID is required");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Preload all scanner devices into memory (single database query)
                var allScanners = _db.scanner.ToDictionary(x => x.dev_code, x => x.type);

                // Get date range
                var storedDateList = InBetweenDates(retriveInfo.startDate, retriveInfo.endDate);

                // Get employee info
                var internInfo = _db.intern
                    .FirstOrDefault(x => x.rfid == retriveInfo.internRfid);

                if (internInfo == null)
                {
                    return NotFound($"Intern with RFID {retriveInfo.internRfid} not found");
                }

                // Get all logs in one query
                var internLogs = _db.log
                    .Where(x => x.card == retriveInfo.internRfid &&
                               x.ts >= retriveInfo.startDate &&
                               x.ts <= retriveInfo.endDate.AddDays(1))
                    .OrderBy(x => x.ts)
                    .ToList();

                // Process logs with cached scanner data
                var processedLogs = internLogs
                    .Select(log => new internlogs
                    {
                        logs = log.ts,
                        logType = GetDeviceTypeCached(log.dev, allScanners) // Uses cached data
                    })
                    .ToList();

                // Group by date
                var internLogRecordsLists = new List<InternLogRecodsList>();
                foreach (var dateItem in storedDateList)
                {
                    var logsForDate = processedLogs
                        .Where(x => x.logs.Date == dateItem.date.Date)
                        .ToList();

                    internLogRecordsLists.Add(new InternLogRecodsList
                    {
                        rfid = retriveInfo.internRfid,
                        date = dateItem.date,
                        day = dateItem.day.ToString(),
                        internCode = internInfo.internCode,
                        internName = internInfo.name,
                        dayOfWeek = dateItem.dayOfWeek,
                        records = logsForDate
                    });
                }

                return Ok(internLogRecordsLists);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }

        private string GetDeviceTypeCached(string dev, Dictionary<string, int> scannerLookup)
        {
            if (string.IsNullOrEmpty(dev))
                return "N/A";

            if (scannerLookup.TryGetValue(dev, out var roomType))
            {
                if (roomType == 1)
                    return "In";
                if (roomType == 2)
                    return "Out";
                if (roomType == 3)
                    return "In/Out";
               
            }

            return "N/A";
        }

        // Helper method remains the same
        public List<LogRecordsDateList> InBetweenDates(DateTime startDate, DateTime endDate)
        {
            List<LogRecordsDateList> records = new List<LogRecordsDateList>();

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be earlier than or equal to end date.");
            }

            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                records.Add(new LogRecordsDateList
                {
                    date = currentDate,
                    dayOfWeek = currentDate.DayOfWeek.ToString(),
                    day = currentDate.Day
                });

                currentDate = currentDate.AddDays(1);
            }
            return records;
        }



        //public List<LogRecordsDateList> LogRecordsDates()
        //{

        //    var list = new List<dList>();

        //    return list;
        //}


        [Route("api/SearchEmployees")]
        [HttpGet]
        public async Task<IActionResult> SearchEmployees([FromQuery] string field, [FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return BadRequest("Search term must be at least 2 characters");
                }

                term = term.ToLower();
                IQueryable<employee> query = _db.employee;

                switch (field.ToLower())
                {
                    case "rfid":
                        query = query.Where(e => e.rfid.ToLower().Contains(term));
                        break;
                    case "code":
                        query = query.Where(e => e.employeeCode.ToLower().Contains(term));
                        break;
                    case "name":
                        query = query.Where(e => e.employeeName.ToLower().Contains(term));
                        break;
                    default:
                        return BadRequest("Invalid search field");
                }

                var employees = await query
                    .OrderBy(e => e.employeeName)
                    .Take(10)
                    .Select(e => new
                    {
                        rfid = e.rfid,
                        code = e.employeeCode,
                        name = e.employeeName
                    })
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching employees");
            }
        }
       
    

        [Route("api/SearchInterns")]
        [HttpGet]
        public async Task<IActionResult> SearchInterns([FromQuery] string field, [FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return BadRequest("Search term must be at least 2 characters");
                }

                term = term.ToLower();
                IQueryable<intern> query = _db.intern;

                switch (field.ToLower())
                {
                    case "rfid":
                        query = query.Where(e => e.rfid.ToLower().Contains(term));
                        break;
                    case "code":
                        query = query.Where(e => e.internCode.ToLower().Contains(term));
                        break;
                    case "name":
                        query = query.Where(e => e.name.ToLower().Contains(term));
                        break;
                    default:
                        return BadRequest("Invalid search field");
                }

                var interns = await query
                    .OrderBy(e => e.name)
                    .Take(10)
                    .Select(e => new
                    {
                        rfid = e.rfid,
                        code = e.internCode,
                        name = e.name
                    })
                    .ToListAsync();

                return Ok(interns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching interns");
            }
        }

        [HttpGet("by-rfid/{rfid}")]
        public async Task<IActionResult> GetEmployeeByRfid(string rfid)
        {
            var employee = await _db.employee
                .Where(e => e.rfid == rfid)
                .Select(e => new
                {
                    rfid = e.rfid,
                    code = e.employeeCode,
                    name = e.employeeName
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        [HttpGet("by-rfid/{rfid}")]
        public async Task<IActionResult> GetInternByRfid(string rfid)
        {
            var intern = await _db.intern
                .Where(e => e.rfid == rfid)
                .Select(e => new
                {
                    rfid = e.rfid,
                    code = e.internCode,
                    name = e.name
                })
                .FirstOrDefaultAsync();

            if (intern == null)
            {
                return NotFound();
            }

            return Ok(intern);
        }


        ////Generate Excel sheet


      
        public ActionResult<List<EmployeeLogRecodsList>> GenerateAllEmployeeLogRecords([FromBody] RetriveEmployeeAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Preload all scanner devices into memory (single database query)
                var allScanners = _db.scanner.ToDictionary(x => x.dev_code, x => x.type);

                // Get date range
                var storedDateList = InBetweenDates(retriveInfo.startDate, retriveInfo.endDate);


                var officeId = HttpContext.Session.GetInt32("companyId");

                // Get all employees in the company
                var employees = _db.employee
                    .Where(e => e.companyId == officeId)
                    .ToList();

                if (!employees.Any())
                {
                    return NotFound("No employees found for this company");
                }

                // Get all logs in one query (for all employees)
                var allLogs = (from l in _db.log
                               join e in _db.employee on l.card equals e.rfid
                               where l.ts >= retriveInfo.startDate &&
                                     l.ts <= retriveInfo.endDate.AddDays(1) &&
                                     e.companyId == officeId
                               orderby l.ts
                               select new { Log = l, Employee = e })
                           .ToList();

                // Process logs for ALL employees
                var result = new List<EmployeeLogRecodsList>();

                foreach (var employee in employees)
                {
                    var employeeLogs = allLogs
                        .Where(x => x.Employee.rfid == employee.rfid)
                        .Select(x => new employeelogs
                        {
                            logs = x.Log.ts,
                            logType = GetDeviceTypeCached(x.Log.dev, allScanners)
                        })
                        .ToList();

                    // Group by date for this employee
                    foreach (var dateItem in storedDateList)
                    {
                        var logsForDate = employeeLogs
                            .Where(x => x.logs.Date == dateItem.date.Date)
                            .ToList();

                        if (logsForDate.Any()) // Only add dates with logs
                        {
                            result.Add(new EmployeeLogRecodsList
                            {
                                rfid = employee.rfid,
                                date = dateItem.date,
                                day = dateItem.day.ToString(),
                                employeeCode = employee.employeeCode,
                                employeeName = employee.employeeName,
                                dayOfWeek = dateItem.dayOfWeek,
                                records = logsForDate
                            });
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }

        public ActionResult<List<EmployeeLogRecodsList>> GenerateAllInternLogRecords([FromBody] RetriveAllInternRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Preload all scanner devices into memory (single database query)
                var allScanners = _db.scanner.ToDictionary(x => x.dev_code, x => x.type);

                // Get date range
                var storedDateList = InBetweenDates(retriveInfo.startDate, retriveInfo.endDate);


                var officeId = HttpContext.Session.GetInt32("companyId");

                // Get all employees in the company
                var interns = _db.intern
                    .Where(e => e.companyId == officeId)
                    .ToList();

                if (!interns.Any())
                {
                    return NotFound("No Interns found for this company");
                }

                // Get all logs in one query (for all employees)
                var allLogs = (from l in _db.log
                               join e in _db.intern on l.card equals e.rfid
                               where l.ts >= retriveInfo.startDate &&
                                     l.ts <= retriveInfo.endDate.AddDays(1) &&
                                     e.companyId == officeId
                               orderby l.ts
                               select new { Log = l, Intern = e })
                           .ToList();

                // Process logs for ALL interns
                var result = new List<InternLogRecodsList>();

                foreach (var intern in interns)
                {
                    var internLogs = allLogs
                        .Where(x => x.Intern.rfid == intern.rfid)
                        .Select(x => new internlogs
                        {
                            logs = x.Log.ts,
                            logType = GetDeviceTypeCached(x.Log.dev, allScanners)
                        })
                        .ToList();

                    // Group by date for this employee
                    foreach (var dateItem in storedDateList)
                    {
                        var logsForDate = internLogs
                            .Where(x => x.logs.Date == dateItem.date.Date)
                            .ToList();

                        if (logsForDate.Any()) // Only add dates with logs
                        {
                            result.Add(new InternLogRecodsList
                            {
                                rfid = intern.rfid,
                                date = dateItem.date,
                                day = dateItem.day.ToString(),
                                internCode = intern.internCode,
                                internName = intern.name,
                                dayOfWeek = dateItem.dayOfWeek,
                                records = logsForDate
                            });
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }



        [Route("api/DownloadEmployeeLogRecordsExcel")]
        [HttpPost]
        public IActionResult DownloadEmployeeLogRecordsExcel([FromBody] RetriveEmployeeAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Get the data using your existing method
                var actionResult = GenerateAllEmployeeLogRecords(retriveInfo);
                if (actionResult.Result is not OkObjectResult okResult)
                {
                    return actionResult.Result;
                }

                var employeeLogRecords = okResult.Value as List<EmployeeLogRecodsList>;
                if (employeeLogRecords == null)
                {
                    return NotFound("No log records found for the specified criteria");
                }

                // Create complete date range with all dates
                var allDates = new List<DateTime>();
                for (var date = retriveInfo.startDate; date <= retriveInfo.endDate; date = date.AddDays(1))
                {
                    allDates.Add(date);
                }

                // Get distinct employee names
                var employeeNames = employeeLogRecords.Select(x => x.employeeName).Distinct().ToList();

                // Create complete records for all dates and all employees
                var completeRecords = new List<EmployeeLogRecodsList>();
                foreach (var employee in employeeNames)
                {
                    foreach (var date in allDates)
                    {
                        var existingRecord = employeeLogRecords.FirstOrDefault(r =>
                            r.employeeName == employee &&
                            r.date.Date == date.Date);

                        completeRecords.Add(existingRecord ?? new EmployeeLogRecodsList
                        {
                            date = date,
                            dayOfWeek = date.DayOfWeek.ToString(),
                            employeeName = employee,
                            records = null
                        });
                    }
                }

                // Determine the maximum number of logs any employee has in a single day
                int maxLogs = completeRecords
                    .Max(record => record.records?.Count ?? 0);

                // Create Excel workbook with a single worksheet
                using (var workbook = new XLWorkbook())
                {
                    // Create one worksheet for all employees
                    var worksheet = workbook.Worksheets.Add("Employee Logs");

                    // Add basic headers
                    worksheet.Cell(1, 1).Value = "S.I.No";
                    worksheet.Cell(1, 2).Value = "Date";
                    //worksheet.Cell(1, 3).Value = "Day";
                    //worksheet.Cell(1, 4).Value = "Day of Week";
                    worksheet.Cell(1, 3).Value = "Employee";

                    // Add dynamic log headers (Log 1, Log 2, etc.)
                    for (int i = 1; i <= maxLogs; i++)
                    {
                        worksheet.Cell(1, 3 + i).Value = $"Log {i}";
                    }

                    // Format headers
                    var headerRange = worksheet.Range(1, 1, 1, 3 + maxLogs);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add data for all employees in one sheet
                    int row = 2;
                    foreach (var record in completeRecords.OrderBy(x => x.employeeName).ThenBy(x => x.date))
                    {
                        worksheet.Cell(row, 1).Value = row - 1; // S.I.No
                        worksheet.Cell(row, 2).Value = record.date.ToString("yyyy-MM-dd");
                        //worksheet.Cell(row, 3).Value = record.date.Day;
                        //worksheet.Cell(row, 4).Value = record.dayOfWeek;
                        worksheet.Cell(row, 3).Value = record.employeeName;

                        // Add each log to a separate cell
                        if (record.records != null && record.records.Any())
                        {
                            var orderedLogs = record.records
                                .OrderBy(x => x.logs)
                                .Select(log => $"{log.logs:hh:mm tt} {(log.logType)}")
                                .ToList();

                            for (int i = 0; i < orderedLogs.Count; i++)
                            {
                                worksheet.Cell(row, 4 + i).Value = orderedLogs[i];
                            }
                        }
                        else
                        {
                            worksheet.Cell(row, 4).Value = "No records";
                        }

                        row++;
                    }

                    // Adjust column widths
                    worksheet.Columns().AdjustToContents();

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Return Excel file
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"All_Employee_LogRecords_{retriveInfo.startDate:yyyy/MM/dd}_to_{retriveInfo.endDate:yyyy/MM/dd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while generating the Excel file: {ex.Message}");
            }
        }



        [Route("api/DownloadInternLogRecordsExcel")]
        [HttpPost]
        public IActionResult DownloadInternLogRecordsExcel([FromBody] RetriveAllInternRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Get the data using your existing method
                var actionResult = GenerateAllInternLogRecords(retriveInfo);
                if (actionResult.Result is not OkObjectResult okResult)
                {
                    return actionResult.Result;
                }

                var internLogRecords = okResult.Value as List<InternLogRecodsList>;
                if (internLogRecords == null)
                {
                    return NotFound("No log records found for the specified criteria");
                }

                // Create complete date range with all dates
                var allDates = new List<DateTime>();
                for (var date = retriveInfo.startDate; date <= retriveInfo.endDate; date = date.AddDays(1))
                {
                    allDates.Add(date);
                }

                // Get distinct employee names
                var internNames = internLogRecords.Select(x => x.internName).Distinct().ToList();

                // Create complete records for all dates and all employees
                var completeRecords = new List<InternLogRecodsList>();
                foreach (var intern in internNames)
                {
                    foreach (var date in allDates)
                    {
                        var existingRecord = internLogRecords.FirstOrDefault(r =>
                            r.internName == intern &&
                            r.date.Date == date.Date);

                        completeRecords.Add(existingRecord ?? new InternLogRecodsList
                        {
                            date = date,
                            dayOfWeek = date.DayOfWeek.ToString(),
                            internName = intern,
                            records = null
                        });
                    }
                }

                
                int maxLogs = completeRecords
                    .Max(record => record.records?.Count ?? 0);

                // Create Excel workbook with a single worksheet
                using (var workbook = new XLWorkbook())
                {
                    // Create one worksheet for all employees
                    var worksheet = workbook.Worksheets.Add("Intern Logs");

                    // Add basic headers
                    worksheet.Cell(1, 1).Value = "S.I.No";
                    worksheet.Cell(1, 2).Value = "Date";
                    //worksheet.Cell(1, 3).Value = "Day";
                    //worksheet.Cell(1, 4).Value = "Day of Week";
                    worksheet.Cell(1, 3).Value = "Intern";

                    // Add dynamic log headers (Log 1, Log 2, etc.)
                    for (int i = 1; i <= maxLogs; i++)
                    {
                        worksheet.Cell(1, 3 + i).Value = $"Log {i}";
                    }

                    // Format headers
                    var headerRange = worksheet.Range(1, 1, 1, 3 + maxLogs);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add data for all employees in one sheet
                    int row = 2;
                    foreach (var record in completeRecords.OrderBy(x => x.internName).ThenBy(x => x.date))
                    {
                        worksheet.Cell(row, 1).Value = row - 1; // S.I.No
                        worksheet.Cell(row, 2).Value = record.date.ToString("yyyy-MM-dd");
                        //worksheet.Cell(row, 3).Value = record.date.Day;
                        //worksheet.Cell(row, 4).Value = record.dayOfWeek;
                        worksheet.Cell(row, 3).Value = record.internName;

                        // Add each log to a separate cell
                        if (record.records != null && record.records.Any())
                        {
                            var orderedLogs = record.records
                                .OrderBy(x => x.logs)
                                .Select(log => $"{log.logs:hh:mm tt} {(log.logType)}")
                                .ToList();

                            for (int i = 0; i < orderedLogs.Count; i++)
                            {
                                worksheet.Cell(row, 4 + i).Value = orderedLogs[i];
                            }
                        }
                        else
                        {
                            worksheet.Cell(row, 4).Value = "No records";
                        }

                        row++;
                    }

                    // Adjust column widths
                    worksheet.Columns().AdjustToContents();

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();


                        // Return Excel file
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"All_Intern_LogRecords_{retriveInfo.startDate:yyyy/MM/dd}_to_{retriveInfo.endDate:yyyy/MM/dd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while generating the Excel file: {ex.Message}");
            }
        }



        [Route("api/DownloadEmployeeParticularLogRecordsExcel")]
        [HttpPost]
        public IActionResult DownloadEmployeeParticularLogRecordsExcel([FromBody] RetriveAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Get the data using your existing method
                var actionResult = GetEmployeeLogRecords(retriveInfo);
                if (actionResult.Result is not OkObjectResult okResult)
                {
                    return actionResult.Result;
                }

                var employeeLogRecords = okResult.Value as List<EmployeeLogRecodsList>;
                if (employeeLogRecords == null)
                {
                    return NotFound("No log records found for the specified criteria");
                }

                // Create complete date range with all dates
                var allDates = new List<DateTime>();
                for (var date = retriveInfo.startDate; date <= retriveInfo.endDate; date = date.AddDays(1))
                {
                    allDates.Add(date);
                }

                // Get distinct employee names
                var employeeNames = employeeLogRecords.Select(x => x.employeeName).Distinct().ToList();

                // Create complete records for all dates and all employees
                var completeRecords = new List<EmployeeLogRecodsList>();
                foreach (var employee in employeeNames)
                {
                    foreach (var date in allDates)
                    {
                        var existingRecord = employeeLogRecords.FirstOrDefault(r =>
                            r.employeeName == employee &&
                            r.date.Date == date.Date);

                        completeRecords.Add(existingRecord ?? new EmployeeLogRecodsList
                        {
                            date = date,
                            dayOfWeek = date.DayOfWeek.ToString(),
                            employeeName = employee,
                            records = null
                        });
                    }
                }

                // Determine the maximum number of logs any employee has in a single day
                int maxLogs = completeRecords
                    .Max(record => record.records?.Count ?? 0);

                // Create Excel workbook with a single worksheet
                using (var workbook = new XLWorkbook())
                {
                    // Create one worksheet for all employees
                    var worksheet = workbook.Worksheets.Add("Employee Logs");

                    // Add basic headers
                    worksheet.Cell(1, 1).Value = "S.I.No";
                    worksheet.Cell(1, 2).Value = "Date";
                    //worksheet.Cell(1, 3).Value = "Day";
                    //worksheet.Cell(1, 4).Value = "Day of Week";
                    worksheet.Cell(1, 3).Value = "Employee";

                    // Add dynamic log headers (Log 1, Log 2, etc.)
                    for (int i = 1; i <= maxLogs; i++)
                    {
                        worksheet.Cell(1, 3 + i).Value = $"Log {i}";
                    }

                    // Format headers
                    var headerRange = worksheet.Range(1, 1, 1, 3 + maxLogs);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add data for all employees in one sheet
                    int row = 2;
                    foreach (var record in completeRecords.OrderBy(x => x.employeeName).ThenBy(x => x.date))
                    {
                        worksheet.Cell(row, 1).Value = row - 1; // S.I.No
                        worksheet.Cell(row, 2).Value = record.date.ToString("yyyy-MM-dd");
                        //worksheet.Cell(row, 3).Value = record.date.Day;
                        //worksheet.Cell(row, 4).Value = record.dayOfWeek;
                        worksheet.Cell(row, 3).Value = record.employeeName;

                        // Add each log to a separate cell
                        if (record.records != null && record.records.Any())
                        {
                            var orderedLogs = record.records
                                .OrderBy(x => x.logs)
                                .Select(log => $"{log.logs:hh:mm tt} {(log.logType)}")
                                .ToList();

                            for (int i = 0; i < orderedLogs.Count; i++)
                            {
                                worksheet.Cell(row, 4 + i).Value = orderedLogs[i];
                            }
                        }
                        else
                        {
                            worksheet.Cell(row, 4).Value = "No records";
                        }

                        row++;
                    }

                    // Adjust column widths
                    worksheet.Columns().AdjustToContents();

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Return Excel file
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"EmployeeLogRecords_{retriveInfo.startDate:yyyy/MM/dd}_to_{retriveInfo.endDate:yyyy/MM/dd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while generating the Excel file: {ex.Message}");
            }
        }


        [Route("api/DownloadInternPeopleParticularLogRecordsExcel")]
        [HttpPost]
        public IActionResult DownloadInternPeopleParticularLogRecordsExcel([FromBody] RetriveInternAllRecords retriveInfo)
        {
            try
            {
                // Input validation
                if (retriveInfo == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (retriveInfo.startDate > retriveInfo.endDate)
                {
                    return BadRequest("Start date must be earlier than or equal to end date");
                }

                // Get the data using your existing method
                var actionResult = GetInternLogRecords(retriveInfo);
                if (actionResult.Result is not OkObjectResult okResult)
                {
                    return actionResult.Result;
                }

                var internLogRecords = okResult.Value as List<InternLogRecodsList>;
                if (internLogRecords == null)
                {
                    return NotFound("No log records found for the specified criteria");
                }

                // Create complete date range with all dates
                var allDates = new List<DateTime>();
                for (var date = retriveInfo.startDate; date <= retriveInfo.endDate; date = date.AddDays(1))
                {
                    allDates.Add(date);
                }

                // Get distinct employee names
                var internNames = internLogRecords.Select(x => x.internName).Distinct().ToList();

                // Create complete records for all dates and all employees
                var completeRecords = new List<InternLogRecodsList>();
                foreach (var intern in internNames)
                {
                    foreach (var date in allDates)
                    {
                        var existingRecord = internLogRecords.FirstOrDefault(r =>
                            r.internName == intern &&
                            r.date.Date == date.Date);

                        completeRecords.Add(existingRecord ?? new InternLogRecodsList
                        {
                            date = date,
                            dayOfWeek = date.DayOfWeek.ToString(),
                            internName = intern,
                            records = null
                        });
                    }
                }

                // Determine the maximum number of logs any employee has in a single day
                int maxLogs = completeRecords
                    .Max(record => record.records?.Count ?? 0);

                // Create Excel workbook with a single worksheet
                using (var workbook = new XLWorkbook())
                {
                    // Create one worksheet for all employees
                    var worksheet = workbook.Worksheets.Add("Intern Logs");

                    // Add basic headers
                    worksheet.Cell(1, 1).Value = "S.I.No";
                    worksheet.Cell(1, 2).Value = "Date";
                    //worksheet.Cell(1, 3).Value = "Day";
                    //worksheet.Cell(1, 4).Value = "Day of Week";
                    worksheet.Cell(1, 3).Value = "Employee";

                    // Add dynamic log headers (Log 1, Log 2, etc.)
                    for (int i = 1; i <= maxLogs; i++)
                    {
                        worksheet.Cell(1, 3 + i).Value = $"Log {i}";
                    }

                    // Format headers
                    var headerRange = worksheet.Range(1, 1, 1, 3 + maxLogs);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add data for all employees in one sheet
                    int row = 2;
                    foreach (var record in completeRecords.OrderBy(x => x.internName).ThenBy(x => x.date))
                    {
                        worksheet.Cell(row, 1).Value = row - 1; // S.I.No
                        worksheet.Cell(row, 2).Value = record.date.ToString("yyyy-MM-dd");
                        //worksheet.Cell(row, 3).Value = record.date.Day;
                        //worksheet.Cell(row, 4).Value = record.dayOfWeek;
                        worksheet.Cell(row, 3).Value = record.internName;

                        // Add each log to a separate cell
                        if (record.records != null && record.records.Any())
                        {
                            var orderedLogs = record.records
                                .OrderBy(x => x.logs)
                                .Select(log => $"{log.logs:hh:mm tt} {(log.logType)}")
                                .ToList();

                            for (int i = 0; i < orderedLogs.Count; i++)
                            {
                                worksheet.Cell(row, 4 + i).Value = orderedLogs[i];
                            }
                        }
                        else
                        {
                            worksheet.Cell(row, 4).Value = "No records";
                        }

                        row++;
                    }

                    // Adjust column widths
                    worksheet.Columns().AdjustToContents();

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Return Excel file
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"InternLogRecords_{retriveInfo.startDate:yyyy/MM/dd}_to_{retriveInfo.endDate:yyyy/MM/dd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while generating the Excel file: {ex.Message}");
            }
        }



    }
}
