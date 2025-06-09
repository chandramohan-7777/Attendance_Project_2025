using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;



namespace iva.Controllers
{
    public class DeviceController : Controller
    {

        public static List<string> DeviceSaved = new List<string>();
        private readonly AppDbContext _db;
        public DevControllerWrapper dev = new DevControllerWrapper("http://it.ivatechnos.com:2888/");
        public DeviceController(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            var userRole = httpContextAccessor.HttpContext.Session.GetString("userRole");
            if (userRole == "admin")
            {
                // Redirect if user role is admin
                RedirectToAction("AccessDenied", "Home");
            }
            _db = appDbContext;
        }

        [Route("device")]
        public IActionResult List()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var getList = _db.scanner.Select(s => new scannerViewModel
            {
                id = s.id,
                dev_code = s.dev_code,
                room_name = s.room_name,
                ts_add = s.ts_add,
                ts_last_online = s.ts_last_online,
                type = s.type,
                ts_mod = s.ts_mod,
                serverStatus = false

            }).ToList();


            // Update serverStatus for each item in getList
            var currentList = getList.Select(item =>
            {
                item.serverStatus = dev.Sync(item.dev_code);
                return item;
            }).ToList();

            ViewBag.Username = username;
            ViewBag.DeviceList = currentList;
            return View();

        }


        [Route("att/GetDeviceDetail")]
        public JsonResult GetDeviceDetail(int deviceId)
        {
            var list = new employee();
            try
            {
                var checkDevice = (from s in _db.scanner where s.id == deviceId select s).FirstOrDefault();
                if (checkDevice != null)
                {
                    return Json(new { success = true, list = checkDevice });
                }
                else
                {
                    return Json(new { success = true, list = "device not found" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }

        [Route("att/UpdateDeviceDetail")]
        public JsonResult UpdateDeviceDetail(deviceReq obj)
        {
            var list = new employee();
            try
            {
                if (obj.id == 0)
                {
                    var chkName = (from s in _db.scanner where s.room_name == obj.name select s).FirstOrDefault();
                    var chkCode = (from s in _db.scanner where s.dev_code == obj.code select s).FirstOrDefault();

                    if (chkName != null)
                    {
                        return Json(new { success = false, message = "Device name already exist!" });
                    }
                    else if (chkCode != null)
                    {
                        return Json(new { success = false, message = "Device code already exist!" });
                    }
                    else
                    {
                        var d = new scanner()
                        {
                            dev_code = obj.code,
                            room_name = obj.name,
                            type = obj.type,
                            ts_add = DateTime.Now,
                            ts_mod = DateTime.Now,
                            ts_last_online = DateTime.Now
                        };
                        _db.scanner.Add(d);
                        _db.SaveChanges();
                        var getList = _db.scanner.Select(s => new scanner { id = s.id, dev_code = s.dev_code, room_name = s.room_name, ts_add = s.ts_add, ts_last_online = s.ts_last_online, type = s.type, ts_mod = s.ts_mod }).ToList();
                        return Json(new { success = true, message = "Device added successfully!", list = getList });
                    }
                }
                else
                {
                    var chkName = (from s in _db.scanner where s.room_name == obj.name && s.id != obj.id select s).FirstOrDefault();
                    var chkCode = (from s in _db.scanner where s.dev_code == obj.code && s.id != obj.id select s).FirstOrDefault();

                    if (chkName != null)
                    {
                        return Json(new { success = false, message = "Device name already exist!" });
                    }
                    else if (chkCode != null)
                    {
                        return Json(new { success = false, message = "Device code already exist!" });
                    }
                    else
                    {
                        var ds = (from s in _db.scanner where s.id == obj.id select s).FirstOrDefault();
                        if (ds != null)
                        {
                            ds.dev_code = obj.code;
                            ds.room_name = obj.name;
                            ds.type = obj.type;
                            ds.ts_add = DateTime.Now;
                            ds.ts_mod = DateTime.Now;
                            ds.ts_last_online = DateTime.Now;
                            _db.SaveChanges();
                            var getList = _db.scanner.Select(s => new scanner { id = s.id, dev_code = s.dev_code, room_name = s.room_name, ts_add = s.ts_add, ts_last_online = s.ts_last_online, type = s.type, ts_mod = s.ts_mod }).ToList();

                            return Json(new { success = true, message = "Device updated successfully!", list = getList });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Device not found!" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }

        [Route("att/GetMappingDetail")]
        public JsonResult GetMappingDetail(int id, string companyName)
        {
            try
            {
                var dCode = (from s in _db.scanner
                             where s.id == id
                             select s.dev_code).FirstOrDefault();
                if (dCode == null)
                {
                    return Json(new { success = false, message = "Device code not found." });
                }
                var r = dev.List(dCode);
                var lst = new List<EmployeeData>();

                var officeId = (from s in _db.company
                                where s.companyName == companyName
                                select s.id).FirstOrDefault();


                foreach (var d in r)
                {
                    var employeeList = (from s in _db.employee
                                        where s.rfid == d && s.companyId == officeId
                                        select new EmployeeData
                                        {
                                            empName = s.employeeName,
                                            empCode = s.employeeCode,
                                            empRfid = s.rfid,
                                            empId = s.employeeId.ToString()
                                        }).FirstOrDefault();
                    var vList = (from s in _db.intern
                                 where s.rfid == d && s.companyId == officeId
                                 select new EmployeeData
                                 {
                                     empName = s.name,
                                     empCode = s.type,
                                     empRfid = s.rfid,
                                     empId = s.id.ToString()
                                 }).FirstOrDefault();

                    if (employeeList != null)
                    {
                        lst.Add(employeeList);
                    }
                    if (vList != null)
                    {
                        lst.Add(vList);
                    }
                }
                return Json(new { success = true, employees = lst });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving mapping data: " + ex.Message });
            }
        }

        [Route("att/DeleteDevice")]
        public JsonResult DeleteDevice(int deviceId)
        {
            try
            {
                var chkDevice = (from s in _db.scanner where s.id == deviceId select s).FirstOrDefault();
                if (chkDevice != null)
                {
                    _db.scanner.Remove(chkDevice);
                    _db.SaveChanges();
                    var getList = _db.scanner.Select(s => new scanner { id = s.id, dev_code = s.dev_code, room_name = s.room_name, ts_add = s.ts_add, ts_last_online = s.ts_last_online, type = s.type, ts_mod = s.ts_mod }).ToList();

                    return Json(new { success = true, message = "Device deleted successfully!", list = getList });
                }
                else
                {
                    return Json(new { success = false, message = "Device not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }

        [Route("att/GetDeviceMappingDetail")]
        public JsonResult GetDeviceMappingDetail(int id, string companyName)
        {
            try
            {
                var dCode = (from s in _db.scanner
                             where s.id == id
                             select s.dev_code).FirstOrDefault();
                if (dCode == null)
                {
                    Console.WriteLine("Device code is null for ID: " + id);
                    return Json(new { success = false, message = "Device code not found." });
                }

                var officeId = (from s in _db.company
                                where s.companyName == companyName
                                select s.id).FirstOrDefault();

                var allEmployees = _db.employee
               .Where(s => s.IsActive == true && s.companyId == officeId)
               .Select(s => new AllEmployee
               {
                   empId = s.employeeId,
                   empName = s.employeeName,
                   empCode = s.employeeCode,
                   empRfid = s.rfid,
               })
               .ToList();
                var allInterns = _db.intern
                     .Where(s => s.IsActive == true && s.companyId == officeId)
                    .Select(s => new AllEmployee
                    {
                        empId = s.id,
                        empName = s.name,
                        empCode = s.internCode,
                        empRfid = s.rfid,
                    })
                    .ToList();
                var combinedList = allEmployees.Concat(allInterns).ToList();
                var result = new List<EmployeeInformation>();
                bool r = dev.Sync(dCode);
                if (r == true)
                {
                    var deviceCodeList = dev.List(dCode);
                    foreach (var emp in combinedList)
                    {
                        bool employeeNewStatus = deviceCodeList.Contains(emp.empRfid);
                        result.Add(new EmployeeInformation
                        {
                            employeeNewId = emp.empId,
                            employeeNewName = emp.empName,
                            employeeNewCode = emp.empCode,
                            employeeNewRfid = emp.empRfid,
                            employeeNewStatus = employeeNewStatus,
                        });
                    }
                }
                else
                {
                    return Json(new { message = "Device has been offline mode " });
                }

                return Json(new { success = true, list = result, deviceCode = dCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving mapping data: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult HandleButtonClick(string SavedMessage)
        {
            try
            {
                if (DeviceSaved.Count != 0)
                {
                    foreach (var deviceCode in DeviceSaved)
                    {
                        dev.Save(deviceCode);
                    }
                    Console.WriteLine(SavedMessage);

                    return Json(new { success = true, message = "Device Saved Successfully" });
                }
                else
                {
                    return Json(new { success = true, message = "Already All devices are Synced..." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving mapping data: " + ex.Message });
            }
        }

        [Route("att/addNewPeoples")]
        [HttpPost]
        public JsonResult addNewPeoples(string deviceCode, [FromForm] Newemployees[] newList)
        {
            if (!string.IsNullOrEmpty(deviceCode))
            {
                if (!DeviceSaved.Contains(deviceCode))
                {
                    DeviceSaved.Add(deviceCode);
                }
            }
            if (newList == null || !newList.Any())
            {
                return Json(new { success = false, message = "No employees to add." });
            }
            else
            {
                var list = new List<deviceList>();
                var deviceCodeList = dev.List(deviceCode);
                foreach (var employee in newList)
                {
                    bool employeeExists = deviceCodeList.Contains(employee.rfid);
                    if (employeeExists)
                    {
                        Console.WriteLine("Employee already exists in this device.");
                    }
                    else
                    {
                        try
                        {
                            var addedSuccessfully = dev.Add(deviceCode, employee.rfid);
                            if (!addedSuccessfully)
                            {
                                //return Json(new { success = false, message = $"Error deleting {employee.rfid}: {dev.LastErr}" });
                                return Json(new { success = false, message = "Try again later" });

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while adding {employee.rfid}: {ex.Message}");
                        }
                    }
                }
                return Json(new { success = true, message = "Employee Added successfully" });
            }
            return Json(new { success = true, message = "updated successfully" });
        }

        [Route("att/removeNewPeoples")]
        [HttpPost]
        public JsonResult removeNewPeoples(string deviceCode, [FromForm] Newemployees[] newList)
        {
            try
            {
                if (!string.IsNullOrEmpty(deviceCode))
                {
                    if (!DeviceSaved.Contains(deviceCode))
                    {
                        DeviceSaved.Add(deviceCode);
                    }
                }
                if (newList == null || !newList.Any())
                {
                    return Json(new { success = false, message = "No employees to remove." });
                }
                var list = new List<deviceList>();
                foreach (var dt in newList)
                {
                    var r = dev.Del(deviceCode, dt.rfid);
                    if (!r)
                    {
                        //return Json(new { success = false, message = $"Error deleting {dt.rfid}: {dev.LastErr}" });
                        return Json(new { success = false, message = "Try again later" });

                    }
                }
                return Json(new { success = true, message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }

        }


        [Route("api/att/GetDynamicCompany")]

        [HttpGet]
        public JsonResult GetCompanyBasedDepartment()
        {
            var company = _db.company
                               .Select(s => new { s.companyName }).ToList();
            return Json(company);
        }





    }
}
