using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Helpers;

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


        /* Validate Phone */
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            var customer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.PhoneNumber.ToLower() == Phone.ToLower());
            if (customer != null)
            {
                return Json($"PhoneNumber '{Phone}' đã được sử dụng.");
            }
            return Json(true);
        }

        /* Validate Email */
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            var customer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.Email.ToLower() == Email.ToLower());
            if (customer != null)
            {
                return Json($"Email '{Email}' đã được sử dụng.");
            }
            return Json(true);
        }

        /* Get Login */
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
/*            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                _notifyService.Success("Đăng nhập thành công");
                return RedirectToAction("Index", "Home");
            }
            return View();*/
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.Identity.IsAuthenticated)
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
                    if (!isEmail) return View(customer);

                    var localCustomer = _context.Customers.AsNoTracking().SingleOrDefault(item => item.Email.Trim() == customer.Email.Trim());
                    var localAccountAdmin = _context.Accounts.AsNoTracking().Include(item =>item.Role).SingleOrDefault(item => item.Email.Trim() == customer.Email.Trim());

                    if (localCustomer != null)
                    {
                        // Đăng nhập với tài khoản khách hàng
                        if (customer.Password.Trim() != localCustomer.Password.Trim())
                        {
                            _notifyService.Error("Mật khẩu không đúng!");
                            return View(customer);
                        }

                        if (!localCustomer.Active)
                        {
                            _notifyService.Error("Tài khoản hiện tại đang không có quyền truy cập, vui lòng liên hệ admin");
                            return View(customer);
                        }

                        // Lưu thông tin đăng nhập vào session
                        HttpContext.Session.SetString("CustomerId", localCustomer.CustomerId.ToString());
                        var accountId = HttpContext.Session.GetString("CustomerId");

                        // Tạo danh sách claims
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, localCustomer.Email),
                            new Claim("CustomerId", localCustomer.CustomerId.ToString()),
                            new Claim("FullName", localCustomer.FullName),
                            new Claim("Email", localCustomer.Email),
                        };

                        // Tạo identity và authentication properties
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = true,
                        };
                        
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        _notifyService.Success("Đăng nhập thành công");
                        return RedirectToAction("Index", "Home");
                    }
                    else if (localAccountAdmin != null)
                    {
                        // Đăng nhập với tài khoản admin
                        if (customer.Password.Trim() != localAccountAdmin.Password.Trim())
                        {
                            _notifyService.Error("Mật khẩu không đúng!");
                            return View(customer);
                        }

                        if (!localAccountAdmin.Active)
                        {
                            _notifyService.Error("Tài khoản hiện tại đang không có quyền truy cập, vui lòng liên hệ superAdmin");
                            return View(customer);
                        }
                       
                        HttpContext.Session.SetString("CustomerId", localAccountAdmin.AccountId.ToString());
                        var accountId = HttpContext.Session.GetString("CustomerId");
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, localAccountAdmin.Email),
                            new Claim("CustomerId", localAccountAdmin.AccountId.ToString()),
                            new Claim("FullName", localAccountAdmin.AccountName),
                            new Claim("Role", localAccountAdmin.Role.RoleName),
                            new Claim("Email", localAccountAdmin.Email),
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = true,
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Register", "Auth");
                    }
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(customer);
        }


        /* GET Register */
        [HttpGet]
        public IActionResult Register()
        {
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
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
                        FullName = customer.FullName.Trim(),
                        Password = customer.Password.Trim().ToLower(),
                        Address = customer?.Address?.Trim().ToLower(),
                        PhoneNumber = customer.PhoneNumber.Trim().ToLower(),
                        Birthday = customer.Birthday,
                        Avatar = "default.png",
                        Active = true,

                    };
                    try
                    {
                        var duplicateEmail = _context.Customers.AsNoTracking().SingleOrDefault(item => item.Email.ToLower() == customer.Email.Trim().ToLower());
                        var duplicateEmailAccount = _context.Accounts.AsNoTracking().SingleOrDefault(item => item.Email.ToLower() == customer.Email.Trim().ToLower());
                        var duplicatePhoneNumber = _context.Customers.AsNoTracking().SingleOrDefault(item => item.PhoneNumber== customer.PhoneNumber.Trim());

                      


                        if (duplicateEmail != null || duplicateEmailAccount !=null) {
                            _notifyService.Warning($"Email: {customer.Email} đã tồn tại vui lòng điền email khác!");
                         /*   ModelState.AddModelError("Email", "Email đã tồn tại vui lòng nhập email khác");*/
                            return View(customer);
                          
                        }
                        if (duplicatePhoneNumber != null)
                        {
                            _notifyService.Warning($"SDT: {customer.PhoneNumber} đã tồn tại vui lòng điền số điện thoại khác!");
                            return View(customer);
                        }

                        int yearDay = DateTime.Today.Year;
                        if (yearDay - customer.Birthday.Year < 18)
                        {
                            ModelState.AddModelError("Birthday", "Ít nhất phải 18 tuổi mới được đăng ký");
                            return View(customer);
                        }
                         _context.Add(newCustomer);
                         await _context.SaveChangesAsync();
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


        /* Logout */
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("CustomerId"); 
            return RedirectToAction("Login", "Auth"); 
        }



    }


}
