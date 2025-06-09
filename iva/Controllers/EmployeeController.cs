using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using RCC;
using System.Diagnostics;
using System;


namespace iva.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _db;
        public DevControllerWrapper dev = new DevControllerWrapper("http://it.ivatechnos.com:2888/");
        public EmployeeController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        [Route("employee")]
        public IActionResult List()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var officeId = HttpContext.Session.GetInt32("companyId");


            var getList = _db.employee
            .Where(s => s.companyId == officeId)
            .Select(s => new employee
            {
                employeeId = s.employeeId,
                employeeName = s.employeeName,
                employeeCode = s.employeeCode,
                department = s.department,
                position = s.position,
                experience = s.experience,
                rfid = s.rfid,
                IsActive = s.IsActive,
                from_date = s.from_date,
                to_date = s.to_date
            }).ToList();
            ViewBag.employeeList = getList;
            return View();
        }



        [Route("api/att/GetCompanyBasedDepartment")]

        [HttpGet]
        public JsonResult GetCompanyBasedDepartment()
        {
            var officeId = HttpContext.Session.GetInt32("companyId");


            var departments = _db.department
                               .Where(s => s.companyId == officeId)
                               .Select(s => new { s.departmentName }).ToList();
            return Json(departments);
        }




        [Route("att/GetEmployeeDetail")]
        public JsonResult GetEmployeeDetail(int employeeId)
        {
            var list = new employee();
            try
            {

                var checkEmployeeId = _db.employee.Where(w => w.employeeId == employeeId).Select(s => new employee

                {
                    employeeId = s.employeeId,
                    employeeName = s.employeeName,
                    employeeCode = s.employeeCode,
                    department = s.department,
                    position = s.position,
                    experience = s.experience,
                    rfid = s.rfid,
                    IsActive = s.IsActive,
                    from_date = s.from_date,
                    to_date = s.to_date
                }).FirstOrDefault();

                if (checkEmployeeId != null)
                {
                    return Json(new { success = true, list = checkEmployeeId });
                }
                else
                {
                    return Json(new { success = true, list = "employee not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }

        [Route("att/GetDeviceList")]
        public JsonResult GetDeviceList(int employeeId)
        {
            var list = new List<deviceList>();
            try
            {
                var getList = _db.scanner.Select(s => new deviceList { id = s.id, name = s.room_name }).ToList();
                var Type = "Employee";

                if (getList != null)
                {
                    foreach (var d in getList)
                    {
                        var obj = new deviceList();
                        var chkMap = _db.emp_mapping_scanner.Where(w => w.employeeId == employeeId && w.scannerId == d.id && w.type == Type).Select(s => new { s.id }).FirstOrDefault();
                        if (chkMap != null)
                        {
                            obj.id = d.id;
                            obj.name = d.name;
                            obj.status = true;
                        }
                        else
                        {
                            obj.id = d.id;
                            obj.name = d.name;
                            obj.status = false;
                        }
                        list.Add(obj);
                    }
                    return Json(new { success = true, list = list });
                }
                else
                {
                    return Json(new { success = false, message = "device not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("att/MappingDevice")]
        public JsonResult MappingDevice(int employeeId, devices[] dList)
        {
            var list = new List<deviceList>();
            try
            {
                var Type = "Employee";

                var getList = _db.emp_mapping_scanner.Where(w => w.employeeId == employeeId && w.type == Type).Select(s => new { id = s.id }).ToList();
                var getRfId = _db.employee.Where(w => w.employeeId == employeeId ).Select(s => new { rfId = s.rfid }).FirstOrDefault();
                var de = _db.emp_mapping_scanner.FirstOrDefault();
                if (getList.Count != 0)
                {
                    foreach (var dev in getList)
                    {
                        var device = _db.emp_mapping_scanner.SingleOrDefault(e => e.id == dev.id && e.type == Type);
                        var d = (from em in _db.emp_mapping_scanner select em).ToList();
                        _db.emp_mapping_scanner.Remove(device);
                    }
                    var getAllDevices = _db.scanner
                                        .Select(s => new { s.dev_code })
                                        .ToList();
                    foreach (var dt in getAllDevices)
                    {
                        if (dt != null)
                        {
                            var r = dev.Del(dt.dev_code, getRfId.rfId);
                            if (r == true)
                            {
                                dev.Save(dt.dev_code);
                            }
                        }
                    }
                    foreach (var dt in dList)
                    {
                        var devId = int.Parse(dt.deviceId);
                        var a = new emp_mapping_scanner()
                        {
                            employeeId = employeeId,
                            scannerId = devId,
                            mapping_by = employeeId,
                            mapping_dt = DateTime.Now,
                            type = Type
                        };
                        _db.emp_mapping_scanner.Add(a);
                        var getDevice = _db.scanner.Where(w => w.id == devId).Select(s => new { s.dev_code }).FirstOrDefault();
                        var r = dev.Add(getDevice.dev_code, getRfId.rfId);
                        if (r == true)
                        {
                            dev.Save(getDevice.dev_code);
                        }
                    }
                    _db.SaveChanges();
                    return Json(new { success = true, message = "update" });
                }
                else
                {
                    foreach (var dt in dList)
                    {
                        var devId = int.Parse(dt.deviceId);
                        var a = new emp_mapping_scanner()
                        {
                            employeeId = employeeId,
                            scannerId = devId,
                            mapping_by = employeeId,
                            mapping_dt = DateTime.Now,
                            type = Type
                        };
                        _db.emp_mapping_scanner.Add(a);
                        var getDevice = _db.scanner.Where(w => w.id == devId).Select(s => new { s.dev_code }).FirstOrDefault();
                        var r = dev.Add(getDevice.dev_code, getRfId.rfId);
                        if (r == true)
                        {
                            dev.Save(getDevice.dev_code);
                        }
                    }
                    _db.SaveChanges();
                    return Json(new { success = true, message = "add" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("att/UpdateEmployee")]
        public JsonResult UpdateEmployee(employeeReqModel obj)
        {
            try
            {
                if (obj.employeeId == 0 && obj.employeeId != null)
                {
                    var checkCode = _db.employee.FirstOrDefault(s => s.employeeCode == obj.employeeCode);
                    var checkRFID = _db.employee.FirstOrDefault(s => s.rfid == obj.rfid);

                    if (checkCode != null)
                    {
                        return Json(new { success = false, message = "Employee code already exists" });
                    }
                    else if (checkRFID != null)
                    {
                        return Json(new { success = false, message = "RFID already exists" });
                    }
                    else
                    {
                        var cDate = DateTime.Now;
                        var employeeJoiningDate = obj.from_date == DateTime.MinValue ? cDate : obj.from_date;
                        var employeeRelievingDate = obj.to_date;
                        var officeId = HttpContext.Session.GetInt32("companyId");
                        int officialOfficeId = officeId.Value;

                        var employeeData = new employee()
                        {
                            employeeName = obj.employeeName,
                            employeeCode = obj.employeeCode,
                            department = obj.department,
                            position = obj.position,
                            rfid = obj.rfid,
                            experience = obj.experience,
                            created_dt = cDate,
                            IsActive = obj.IsActive,
                            from_date = employeeJoiningDate,
                            to_date = employeeRelievingDate,
                            companyId = officialOfficeId
                        };
                        _db.employee.Add(employeeData);
                        _db.SaveChanges();
                        var getList = _db.employee.Select(s => new employee
                        {
                            employeeId = s.employeeId,
                            employeeName = s.employeeName,
                            employeeCode = s.employeeCode,
                            department = s.department,
                            position = s.position,
                            experience = s.experience,
                            rfid = s.rfid,
                            IsActive = s.IsActive,
                        }).ToList();

                        return Json(new { success = true, list = getList , message = "New Employee Added Successfully"});
                    }
                }
                else if (obj.employeeId != 0 && obj.employeeId != null)
                {
                    var cDate = DateTime.Now;
                    var checkCode = _db.employee.Where(w => w.employeeCode == obj.employeeCode && w.employeeId != obj.employeeId).Select(s => new { s.employeeId }).FirstOrDefault();
                    var checkRFID = _db.employee.Where(w => w.rfid == obj.rfid && w.employeeId != obj.employeeId).Select(s => new { s.employeeId }).FirstOrDefault();

                    if (checkCode != null)
                    {
                        return Json(new { success = false, message = "Employee code already exists to another employee" });
                    }
                    else if (checkRFID != null)
                    {
                        return Json(new { success = false, message = "RFID already exists to another employee" });
                    }
                    else
                    {
                        var checkId = _db.employee.FirstOrDefault(s => s.employeeId == obj.employeeId);
                        if (checkId != null)
                        {
                            checkId.employeeName = obj.employeeName;
                            checkId.employeeCode = obj.employeeCode;
                            checkId.position = obj.position;
                            checkId.department = obj.department;
                            checkId.position = obj.position;
                            checkId.experience = obj.experience;
                            checkId.rfid = obj.rfid;
                            checkId.updated_dt = cDate;
                            checkId.updated_by = 1;
                            checkId.created_by = 1;
                            checkId.created_dt = DateTime.Now;
                            checkId.IsActive = obj.IsActive;
                            checkId.from_date = obj.from_date;
                            checkId.to_date = obj.to_date;
                            _db.SaveChanges();
                            var getEmpList = _db.employee.Select(s => new employee
                            {
                                employeeId = s.employeeId,
                                employeeName = s.employeeName,
                                employeeCode = s.employeeCode,
                                department = s.department,
                                position = s.position,
                                experience = s.experience,
                                rfid = s.rfid,
                                IsActive = s.IsActive,
                                from_date = s.from_date,
                                to_date = s.to_date,
                            }).ToList();
                            return Json(new { success = true, list = getEmpList, message = "Employee Updated Successfully" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "employee not found" });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "employee details not added" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message ,message = ex.Message});
            }
        }

        [Route("att/DeleteEmployee")]
        public JsonResult DeleteEmployee(int employeeId)
        {
            try
            {
                var checkDeviceId = _db.employee.FirstOrDefault(s => s.employeeId == employeeId);
                if (checkDeviceId != null)
                {
                    var checkEmployeeMappingScanners = _db.emp_mapping_scanner.Where(s => s.employeeId == checkDeviceId.employeeId).ToList();
                    _db.employee.Remove(checkDeviceId);
                    Console.WriteLine("Employee removed.");
                   
                    if (checkEmployeeMappingScanners.Any())
                    {
                        _db.emp_mapping_scanner.RemoveRange(checkEmployeeMappingScanners);
                        Console.WriteLine("Employee mapping scanners removed.");
                    }
                    _db.SaveChanges();
                    Console.WriteLine("Changes saved to the database.");
                    var getList = _db.employee.Select(s => new employee
                    {
                        employeeId = s.employeeId,
                        employeeName = s.employeeName,
                        employeeCode = s.employeeCode,
                        department = s.department,
                        position = s.position,
                        experience = s.experience,
                        rfid = s.rfid
                    }).ToList();
                    return Json(new { success = true, list = getList, message = "Employee deleted Successfully" });
                }
                else
                {
                    return Json(new { success = false, list = "employee not found" , message ="Employee not found" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, list = ex.Message, message = ex.Message });
            }
        }

        [Route("visitor")]
        public IActionResult Visitor()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var officeId = HttpContext.Session.GetInt32("companyId");

            var getList = _db.intern
                .Where(s => s.companyId == officeId)
                .Select(s => new intern
                {
                    id = s.id,
                    name = s.name,
                    phoneNo = s.phoneNo,
                    purpose = s.purpose,
                    type = s.type,
                    rfid = s.rfid,
                    IsActive = s.IsActive,
                    department = s.department,
                    internCode = s.internCode,
                }).ToList();
               

            ViewBag.visitorList = getList;
            return View();
        }

        [HttpPost]
        [Route("att/UpdateVisitorDetail")]
        public JsonResult UpdateVisitorDetail(internModel obj)
        {
            var list = new intern();
            try
            {
                if (obj.id == 0 && obj.id != null)
                {
                    var checkPhoneNo = _db.intern.FirstOrDefault(s => s.phoneNo == obj.phoneNo);
                    var checkRFID = _db.intern.FirstOrDefault(s => s.rfid == obj.rfid);

                    if (checkPhoneNo != null)
                    {
                        return Json(new { success = false, message = "Visitor phone number already exists" });
                    }
                    else if (checkRFID != null)
                    {
                        return Json(new { success = false, message = "RFID already exists" });
                    }
                    else
                    {
                        var cDate = DateTime.Now;
                        var internJoiningDate = obj.from_date == DateTime.MinValue ? cDate : obj.from_date;
                        var internRelievingDate = obj.to_date;
                        var officeId = HttpContext.Session.GetInt32("companyId");
                        int officialOfficeId = officeId.Value;
                        var newVisitor = new intern()
                        {
                            name = obj.name,
                            phoneNo = obj.phoneNo,
                            purpose = obj.purpose,
                            type = obj.type,
                            rfid = obj.rfid,
                            IsActive = obj.IsActive,
                            from_date = internJoiningDate,
                            to_date = internRelievingDate,
                            department = obj.department,
                            internCode = obj.internCode,
                            companyId = officialOfficeId

                        };
                        _db.intern.Add(newVisitor);
                        _db.SaveChanges();
                        var getList = _db.intern.Select(s => new intern
                        {
                            id = s.id,
                            name = s.name,
                            phoneNo = s.phoneNo,
                            purpose = s.purpose,
                            type = s.type,
                            rfid = s.rfid,
                            IsActive = s.IsActive,
                            department = s.department,
                            internCode = s.internCode
                        }).ToList();
                        return Json(new { success = true, list = getList, message = "New Visitor Added Successfully" });
                    }
                }
                else if (obj.id != 0 && obj.id != null)
                {
                    var checkPhoneNo = _db.intern.FirstOrDefault(s => s.phoneNo == obj.phoneNo && s.id != obj.id);
                    var checkRFID = _db.intern.FirstOrDefault(s => s.rfid == obj.rfid && s.id != obj.id);
                    if (checkPhoneNo != null)
                    {
                        return Json(new { success = false, message = "Visitor phone number already exists for another visitor" });
                    }
                    else if (checkRFID != null)
                    {
                        return Json(new { success = false, message = "RFID already exists for another visitor" });
                    }
                    else
                    {
                        var checkId = (from s in _db.intern where s.id == obj.id select s).FirstOrDefault();
                        if (checkId != null)
                        {
                            checkId.name = obj.name;
                            checkId.phoneNo = obj.phoneNo;
                            checkId.purpose = obj.purpose;
                            checkId.type = obj.type;
                            checkId.rfid = obj.rfid;
                            checkId.IsActive = obj.IsActive;
                            checkId.from_date = obj.from_date;
                            checkId.to_date = obj.to_date;
                            checkId.department = obj.department;
                            checkId.internCode = obj.internCode;
                            var cDate = DateTime.Now;
                           
                            _db.SaveChanges();
                            var getVisitList = _db.intern.Select(s => new intern
                            {
                                id = s.id,
                                name = s.name,
                                phoneNo = s.phoneNo,
                                purpose = s.purpose,
                                type = s.type,
                                rfid = s.rfid,
                                IsActive = s.IsActive,
                                department = s.department,
                                internCode = s.internCode
                            }).ToList();
                            return Json(new { success = true, list = getVisitList, message = "Visitor Updated Successfully" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "visitor not found" });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "visitor details not added" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("att/DeleteVisitor")]
        public JsonResult DeleteVisitor(int visitorId)
        {
            try
            {
                var visitor = _db.intern.FirstOrDefault(s => s.id == visitorId);

                if (visitor == null)
                {
                    return Json(new { success = false, message = "Visitor not found" });
                }

                _db.intern.Remove(visitor);
                _db.SaveChanges();

                var updatedList = _db.intern.Select(s => new intern
                {
                    id = s.id,
                    name = s.name,
                    phoneNo = s.phoneNo,
                    purpose = s.purpose,
                    type = s.type,
                    rfid = s.rfid,
                    department = s.department,
                    internCode = s.internCode

                }).ToList();

                return Json(new { success = true, list = updatedList , message = "Visitor deleted Successfully"});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("att/GetVisitorDetail")]
        public JsonResult GetVisitorDetail(int visitorId)
        {

            var list = new intern();
            try
            {
                var checkVisitor = (from s in _db.intern where s.id == visitorId select s).FirstOrDefault();
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
                return Json(new { success = true, list = ex.Message, message = ex.Message });
            }
        }

        [Route("att/VisitorMappingDevice")]
        public JsonResult VisitorMappingDevice(int visitorId, devices[] dList)
        {
            var list = new List<deviceList>();
            try
            {
                var Type = "INTERN";
                var getList = _db.emp_mapping_scanner.Where(w => w.employeeId == visitorId && w.type == Type).Select(s => new { id = s.id }).ToList();

                var getRfId = _db.intern.Where(w => w.id == visitorId).Select(s => new { rfId = s.rfid }).FirstOrDefault();
                var de = _db.emp_mapping_scanner.FirstOrDefault();
                if (getList.Count != 0)
                {
                    foreach (var dev in getList)
                    {
                        var device = _db.emp_mapping_scanner.SingleOrDefault(e => e.id == dev.id && e.type == Type);
                        var d = (from em in _db.emp_mapping_scanner select em).ToList();
                        _db.emp_mapping_scanner.Remove(device);
                    }
                    var getAllDevices = _db.scanner
                                       .Select(s => new { s.dev_code })
                                       .ToList();
                    foreach (var dt in getAllDevices)
                    {
                        if (dt != null)
                        {
                            var r = dev.Del(dt.dev_code, getRfId.rfId);
                            if (r == true)
                            {
                                dev.Save(dt.dev_code);
                            }
                        }
                    }
                    foreach (var dt in dList)
                    {
                        var devId = int.Parse(dt.deviceId);
                        var a = new emp_mapping_scanner()
                        {
                            employeeId = visitorId,
                            scannerId = devId,
                            mapping_by = visitorId,
                            mapping_dt = DateTime.Now,
                            type = Type
                        };
                        _db.emp_mapping_scanner.Add(a);
                        var getDevice = _db.scanner.Where(w => w.id == devId).Select(s => new { s.dev_code }).FirstOrDefault();
                        var r = dev.Add(getDevice.dev_code, getRfId.rfId);
                        if (r == true)
                        {
                            dev.Save(getDevice.dev_code);
                        }
                    }
                    _db.SaveChanges();
                    return Json(new { success = true, message = "update" });
                }
                else
                {
                    foreach (var dt in dList)
                    {
                        var devId = int.Parse(dt.deviceId);
                        var a = new emp_mapping_scanner()
                        {
                            employeeId = visitorId,
                            scannerId = devId,
                            mapping_by = visitorId,
                            mapping_dt = DateTime.Now,
                            type = Type
                        };
                        _db.emp_mapping_scanner.Add(a);
                        var getDevice = _db.scanner.Where(w => w.id == devId).Select(s => new { s.dev_code }).FirstOrDefault();
                        var r = dev.Add(getDevice.dev_code, getRfId.rfId);
                        if (r == true)
                        {
                            dev.Save(getDevice.dev_code);
                        }
                    }
                    _db.SaveChanges();
                    return Json(new { success = true, message = "add" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("att/GetVisitorDeviceList")]
        public JsonResult GetVisitorDeviceList(int visitorId)
        {
            var list = new List<deviceList>();
            try
            {
                var getList = _db.scanner.Select(s => new deviceList { id = s.id, name = s.room_name }).ToList();
                var Type = "INTERN";

                if (getList != null)
                {
                    foreach (var d in getList)
                    {
                        var obj = new deviceList();
                        var chkMap = _db.emp_mapping_scanner.Where(w => w.employeeId == visitorId && w.scannerId == d.id && w.type == Type).Select(s => new { s.id }).FirstOrDefault();
                        if (chkMap != null)
                        {
                            obj.id = d.id;
                            obj.name = d.name;
                            obj.status = true;
                        }
                        else
                        {
                            obj.id = d.id;
                            obj.name = d.name;
                            obj.status = false;
                        }
                        list.Add(obj);
                    }
                    return Json(new { success = true, list = list });
                }
                else
                {
                    return Json(new { success = false, message = "device not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [Route("department")]
        public IActionResult Department()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var officeId = HttpContext.Session.GetInt32("companyId");


            var departments = _db.department
                              .Where(s => s.companyId == officeId)
                              .Select(s => new { s.departmentName,s.companyId,s.id }).ToList();


            ViewBag.departmentList = departments;
            return View();
        }


        [HttpPost]
        [Route("att/AddDepartmentDetail")]
        public JsonResult AddDepartmentDetail(departmentNewList obj)
        {
            try
            {
                if (obj == null)
                {
                    return Json(new { success = false, message = "Invalid data received." });
                }

                if (obj.id == 0)
                {
                    var checkDepartment = _db.department.FirstOrDefault(s => s.departmentName == obj.departmentName);

                    if (checkDepartment != null)
                    {
                        return Json(new { success = false, message = "Department already Exist." });
                    }

                    var officeId = HttpContext.Session.GetInt32("companyId");

                    if (officeId == null)
                    {
                        return Json(new { success = false, message = "Company ID not found in session." });
                    }
                    var newDepartment = new department
                    {
                        departmentName = obj.departmentName,
                        companyId = (int)officeId
                    };

                    _db.department.Add(newDepartment);
                    _db.SaveChanges();

                    var getList = _db.department
                   .Where(s => s.companyId == officeId)
                   .Select(s => new departmentNewList
                   {
                       departmentName = s.departmentName,
                       id = s.id,

                   }).ToList();

                    return Json(new { success = true, message = "Department added successfully.", list = getList });
                }

                return Json(new { success = false, message = "Invalid Department ID." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }




        [HttpPost]
        [Route("att/UpdateDepartmentDetail")]
        public JsonResult UpdateDepartmentDetail(departmentNewList obj)
        {
            try
            {
                if (obj == null || obj.id <= 0)
                {
                    return Json(new { success = false, message = "Invalid department ID." });
                }

                var existingDepartment = _db.department.FirstOrDefault(s => s.id == obj.id);

                if (existingDepartment == null)
                {
                    return Json(new { success = false, message = "Department not found." });
                }
                var officeId = HttpContext.Session.GetInt32("companyId");

                // Update existing holiday details
                existingDepartment.departmentName = obj.departmentName;
                existingDepartment.companyId = (int)officeId;
                _db.SaveChanges();

                var getList = _db.department
                .Where(s => s.companyId == officeId)
                .Select(s => new departmentNewList
                {
                    departmentName = s.departmentName,
                    id = s.id,
                }).ToList();
                return Json(new { success = true, message = "Department updated successfully.", list = getList });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }



        [HttpPost]
        [Route("att/DeleteDepartment")]
        public JsonResult DeleteHoliday(int departmentId)
        {
            try
            {
                var Department = _db.department.FirstOrDefault(s => s.id == departmentId);

                if (Department == null)
                {
                    return Json(new { success = false, message = "Department not found" });
                }

                _db.department.Remove(Department);
                _db.SaveChanges();

                var officeId = HttpContext.Session.GetInt32("companyId");

                var getList = _db.department
                   .Where(s => s.companyId == officeId)
                   .Select(s => new departmentNewList
                   {
                       departmentName = s.departmentName,
                       id = s.id,

                   }).ToList();
                return Json(new { success = true, list = getList, message = "Department Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("att/GetDepartmentDetail")]
        public JsonResult GetDepartmentDetail(int departmentId)
        {
            try
            {
                var checkDepartment = (from s in _db.department where s.id == departmentId select s).FirstOrDefault();
               if (checkDepartment != null)
                {
                    return Json(new { success = true, list = checkDepartment });
                }
                else
                {
                    return Json(new { success = true, list = "Department not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, list = ex.Message });
            }
        }


    }
}
