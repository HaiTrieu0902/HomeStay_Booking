using HomeStay.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Models;

namespace HomeStay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly HomestayDBContext _context;
        private INotyfService _notifyService { get; }

        public HomeController(HomestayDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }
        public IActionResult Index()
        {


            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);

                // Kiểm tra xem role của người dùng có phải là admin không
                var roleClaim = userClaims.FindFirst("Role");
                if (roleClaim != null && (roleClaim.Value == "Admin" || roleClaim.Value == "Manager" || roleClaim.Value == "Super Admin"))
                {
                    // Nếu là admin, tiếp tục hiển thị trang Index
                    var LitsAccount = _context.Accounts.Count();
                    var listCustomers = _context.Customers.Count();
                    var listRoom = _context.Rooms.Count();
                    var ListBooking = _context.Bookings.Count();
                    var totalAmount = _context.Bookings.Sum(b => b.TotalAmount);

                    ViewData["LitsAccount"] = LitsAccount != null ? LitsAccount : 0;
                    ViewData["listCustomer"] = listCustomers != null ? listCustomers : 0;
                    ViewData["listRoom"] = listRoom != null ? listRoom : 0;
                    ViewData["ListBooking"] = ListBooking != null ? ListBooking : 0;
                    ViewData["TotalAmount"] = totalAmount;

                    return View();
                }
                else
                {
                    _notifyService.Error("Bạn không có quyền vào trang Admin");
                    return RedirectToAction("Login", "Auth", new { area = "" });
                }
            }
            else
            {
                _notifyService.Error("Bạn không có quyền vào trang Admin");
                return RedirectToAction("Login", "Auth", new { area = "" });
            }

            /*
             * 
             * 

              var userClaims = User.Identity as ClaimsIdentity;
             if (userClaims != null)
             {
                 userClaims.SetUserClaims(TempData);
             }
             var LitsAccount = _context.Accounts.Count();
             var listCustomers = _context.Customers.Count();
             var listRooom = _context.Rooms.Count();
             var ListBooking = _context.Bookings.Count();

             var totalAmount = _context.Bookings.Sum(b => b.TotalAmount);
             ViewData["LitsAccount"] = LitsAccount != null ? LitsAccount :  0;
             ViewData["listCustomer"] = listCustomers != null ? listCustomers : 0;
             ViewData["listRoom"] = listRooom != null ? listRooom : 0;
             ViewData["ListBooking"] = ListBooking != null ? ListBooking : 0;
             ViewData["TotalAmount"] = totalAmount;
             return View();*/
        }

        /* Logout */
        [Route("/Admin/Home/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("CustomerId");
            /* return RedirectToAction("Login", "Auth", new { area = "" });*/
            return RedirectToAction("Login", "Auth", new { area = "" });

        }
    }
}
