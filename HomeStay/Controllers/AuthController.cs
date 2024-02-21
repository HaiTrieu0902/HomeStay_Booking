using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeStay.Controllers
{
    public class AuthController : Controller
    {
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
        [AcceptVerbs("Get", "Post")]
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
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public JsonResult ValidateEmail(string Email)
        {
            try
            {
                Console.WriteLine("Received Email: " + Email);
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.Email.ToLower() == Email.ToLower());
                if (customer != null)
                    return Json("Email " + Email + " đã được sử dụng.");
                return Json( data : true);
            }
            catch (Exception ex)
            {
                return Json(data : true);
            }
        }

        /* Get Login */
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                _notifyService.Success("Đăng nhập thành công");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /* Post Login */
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel customer)
        {
           try
           {
                if (ModelState.IsValid)
                {
                    bool isEmail = utils.IsValidEmail(customer.Email);
                    if(!isEmail) return View(customer);

                    var localCustomer = _context.Customers.AsNoTracking().SingleOrDefault(item =>item.Email.Trim() == customer.Email.Trim());
                    if(localCustomer == null) return RedirectToAction("Register", "Auth");


                    if(customer.Password.Trim() != localCustomer.Password.Trim())
                    {
                        _notifyService.Error("Mật khẩu không đúng!");
                        return View(customer);
                    }

                    // check xem có bị disable hay không
                    if(localCustomer.Active == false)
                    {
                        _notifyService.Error("Tài khoản hiện tại đang không có quyền truy cập, vui lòng liên hệ admin");
                        return View(customer);
                    }

                    HttpContext.Session.SetString("CustomerId", localCustomer.CustomerId.ToString());
                    var accountId = HttpContext.Session.GetString("CustomerId");

                    // nhận diện
                  /*  var claims = new List<Claim>
                         {
                             new Claim(ClaimTypes.Name, localCustomer.FullName),
                             new Claim("CustomerId" , localCustomer.CustomerId.ToString()),
                         };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);*/

                    List<Claim> claims = new List<Claim>()
                     {
                         new Claim(ClaimTypes.NameIdentifier, localCustomer.Email),
                         new Claim("CustomerId",localCustomer.CustomerId.ToString()),
                         new Claim("FullName",localCustomer.FullName),
                         new Claim("Email",localCustomer.Email),

                     };
                     ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                     AuthenticationProperties properties = new AuthenticationProperties()
                     {
                         AllowRefresh = true,
                         IsPersistent = true,
                     };
                     await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    /* await HttpContext.SignInAsync(claimsPrincipal);*/
                    _notifyService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");
                }
              
           }
           catch(Exception ex)
           {
                return RedirectToAction("Login", "Auth");
           }
            return View(customer);
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
                       /* List<Claim> claims = new List<Claim>()
                         {
                             new Claim(ClaimTypes.NameIdentifier, newCustomer.Email),
                             new Claim("CustomerId",newCustomer.CustomerId.ToString()),
                             new Claim("FullName",newCustomer.FullName),
                             new Claim("Email",newCustomer.Email),

                         };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = true,
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);*/
                        return RedirectToAction("Login", "Auth");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Register", "Auth");
                    }
                }
               
            }
            catch (Exception ex)
            {
                return RedirectToAction("Register", "Auth");
            }
            return View(customer);
        }
    }
}
