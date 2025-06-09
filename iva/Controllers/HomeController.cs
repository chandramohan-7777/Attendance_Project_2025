using iva.Data;
using iva.Models;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Iva.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor httpContextAccessor;
        public HomeController(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _db = appDbContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       
        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult AccessDenied()
        {
            return View();
        }



    }
}