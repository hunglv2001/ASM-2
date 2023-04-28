using Microsoft.AspNetCore.Mvc;

namespace AppDev1670.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
