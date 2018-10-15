using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace OpenSoftware.OidcTemplate.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
