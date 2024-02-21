using Microsoft.AspNetCore.Mvc;

namespace HomeStay.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsUserAuthenticated => User.Identity.IsAuthenticated;
        protected string UserName => User.Identity.Name;
    }
}
