using iva.Data;
using iva.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
namespace iva.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public DevControllerWrapper dev = new DevControllerWrapper("http://it.ivatechnos.com:2888/");
        public AccountController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            var companies = _db.company.Select(c => new CompanyViewModel
            {
                Id = c.id,
                CompanyName = c.companyName
            }).ToList();
            return View(companies);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string username, string password,int companyId)
        {
            var getList = _db.scanner.Select(s => new scanner
            {
                id = s.id,
                dev_code = s.dev_code,
                room_name = s.room_name,
                ts_add = s.ts_add,
                ts_last_online = s.ts_last_online,
                type = s.type,
                ts_mod = s.ts_mod
            }).ToList();
            Thread backgroundThread = new Thread(() =>
            {
                if (getList.Count != 0)
                {
                    foreach (var s in getList)
                    {
                        var syncResult = dev.Sync(s.dev_code);
                        Console.WriteLine($"Device Code: {s.dev_code}, Sync Result: {syncResult}, Dev Lsat error : {dev.LastErr}");
                    }
                }
            });
            backgroundThread.Start();

            var login = (from s in _db.login where s.userName == username && s.password == password && s.companyId == companyId select s).FirstOrDefault();

            var companyName = _db.company
               .Where(x => x.id == login.loginId)
               .Select(x => x.companyName)
               .FirstOrDefault();


            if (login != null)
            {
                string userRole = null;

               
                if (login.role == 1)
                {
                     userRole = "super admin";
                }
                else
                {
                    userRole = "admin";
                }

                if (!string.IsNullOrEmpty(userRole))
                {
                    HttpContext.Session.SetString("userRole", userRole);
                }
                HttpContext.Session.SetInt32("loginId", login.loginId);


                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("companyname", companyName);
                HttpContext.Session.SetInt32("companyId", login.companyId);
                ViewBag.username = username;
                return RedirectToAction("List", "Dashboard");
            }
            else
            {
                ViewBag.Error = "Invalid login credentials.";
                var companies = _db.company.Select(c => new CompanyViewModel
                {
                    Id = c.id,
                    CompanyName = c.companyName
                }).ToList();
                return View(companies);
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Append(".AspNetCore.Session", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });
            return RedirectToAction("Login", "Account");
        }
    }
}
