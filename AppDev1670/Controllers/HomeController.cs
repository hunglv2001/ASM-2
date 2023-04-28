using AppDev1670.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppDev1670.Utils;

namespace AppDev1670.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(Role.CUSTOMER))
                {
                    return RedirectToAction("Index", "Customer");
                }
                if (User.IsInRole(Role.ADMIN))
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (User.IsInRole(Role.STORE_OWNER))
                {
                    return RedirectToAction("Index", "Books");
                }        
            }

            return RedirectToAction("Index", "Customer");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
