using LowFareAirDotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LowFareAirDotnet.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Privacy", "Home") : returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult FlightDetails()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
