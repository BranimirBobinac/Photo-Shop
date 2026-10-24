using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return User.IsInRole("Admin") ? RedirectToAction("Index", "Photo") : RedirectToAction("Browse", "Photo");
            return View();
        }
    }
}
