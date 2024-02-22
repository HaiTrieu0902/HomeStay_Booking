using HomeStay.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeStay.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View();
        }
    }
}
