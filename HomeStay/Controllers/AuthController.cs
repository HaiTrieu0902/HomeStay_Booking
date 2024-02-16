using Microsoft.AspNetCore.Mvc;

namespace HomeStay.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        /* Login */
        public IActionResult Login()
        {
            return View();
        }


        /* Register */
        public IActionResult Register()
        {
            return View();
        }
    }
}
