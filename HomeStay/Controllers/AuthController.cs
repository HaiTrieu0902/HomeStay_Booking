using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace HomeStay.Controllers
{
    public class AuthController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly HomestayDBContext _context;
        HomestayDBContext db = new HomestayDBContext();
        private INotyfService _notifyService { get; }
        public AuthController(HomestayDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        public IActionResult Index()
        {
            return View();
        }

        /* Validate Phone */
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.PhoneNumber.ToLower() == Phone.ToLower());
                if (customer != null)
                    return Json(data: "Số điện thoại: " + Phone + "đã được sử dụng");
                return Json(data: true);
            }
            catch(Exception ex)
            {
                return Json(data: true);
            }
        }

        /* Validate Email */
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.PhoneNumber.ToLower() == Email.ToLower());
                if (customer != null)
                    return Json(data: "Email: " + Email + "đã được sử dụng");
                return Json(data: true);
            }
            catch (Exception ex)
            {
                return Json(data: true);
            }
        }


        /* Get Login */
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        /* Post Login */
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model , string url = null)
        {
           try
           {
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Login", "Auth");
                }
                return RedirectToAction("Login", "Auth");
           }
           catch(Exception ex)
           {
                return RedirectToAction("Login", "Auth");
           }
        }


        /* GET Register */
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        /* POST Register */
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel customer)
        {
           try
            {
                if (ModelState.IsValid)
                {
                    Customer newCustomer = new Customer { 
                        Email = customer.Email.Trim().ToLower(),
                        FullName = customer.FullName.Trim().ToLower(),
                        Password = customer.Password.Trim().ToLower(),
                        Address = customer.Address.Trim().ToLower(),
                        PhoneNumber = customer.PhoneNumber.Trim().ToLower(),
                        Birthday = customer.Birthday,
                        Avatar = "default.png",
                        Active = true,

                    };
                    try
                    {
                        _context.Add(newCustomer);
                        await _context.SaveChangesAsync();
                        // Lưu session mã khách hàng
                        HttpContext.Session.SetString("CustomerId", newCustomer.CustomerId.ToString());
                        var accountId = HttpContext.Session.GetString("CustomerId");

                        // nhận diện
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, newCustomer.Email),
                            new Claim("CustomerId" , newCustomer.CustomerId.ToString()),
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims , "Login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        _notifyService.Success("Đăng ký thành công");
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Register", "Auth");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Register", "Auth");
            }
        }
    }
}
