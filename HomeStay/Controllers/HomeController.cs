﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace HomeStay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomestayDBContext _context;
        private INotyfService _notifyService { get; }
        HomestayDBContext db = new HomestayDBContext();
        public HomeController(ILogger<HomeController> logger , HomestayDBContext context, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index()
        {
            /* string sqlQuery = @"
                 SELECT TOP 10 * 
                 FROM Room
                 WHERE Active = 1 
                 ORDER BY RoomId DESC";
             var listRooms = await _context.Rooms
                                         .FromSqlRaw(sqlQuery)
                                         .Include(r => r.Category)
                                         .AsNoTracking()
                                         .ToListAsync();
             */
            /* var userClaims = User.Identity as ClaimsIdentity;
             if (userClaims != null)
             {
                 var usernameClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
                 var idLogin = userClaims.FindFirst("CustomerId");
                 var useLogin = userClaims.FindFirst("FullName");
                 var emailLogin = userClaims.FindFirst("Email");

                 if (usernameClaim != null)
                 {
                     TempData["IdAccount"] = idLogin?.Value;
                     TempData["NameAccount"] = useLogin?.Value;
                     TempData["EmailAccount"] = emailLogin?.Value;
                 }

             }*/
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            IQueryable<Room> roomQuery = _context.Rooms
                                                .AsNoTracking()
                                                .Include(r => r.Category)
                                                .Where(item => item.Active == true)
                                                .OrderByDescending(item => item.RoomId)
                                                .Take(10);
            var listRooms = await roomQuery.ToListAsync();
            ViewBag.ListRooms = listRooms;
            return View(listRooms);
        }



        /* View  Contact */
        public async Task<IActionResult> HistoryBooking()
        {






            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
                var idLogin = userClaims.FindFirst("CustomerId");

                if (idLogin != null)
                {
                    IQueryable<Booking> bookingQuery = _context.Bookings
                                              .AsNoTracking()
                                              .Include(r => r.Room)
                                              .Include(r => r.Payment)
                                              .Include(r => r.Customer)
                                              .Where(item => item.CustomerId == Convert.ToInt32(idLogin.Value))
                                              .OrderByDescending(item => item.BookingId)
                                              .Take(1000);

                    var listBookings = await bookingQuery.ToListAsync();
                    double totalBookingPrice = listBookings.Sum(booking => booking.TotalAmount);
                    ViewBag.TotalBooking = totalBookingPrice != null ? totalBookingPrice : 0;
                    ViewBag.CountBooking = listBookings != null ? listBookings.Count : 0;
                    ViewBag.ListRooms = listBookings;
                    return View(listBookings);
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();








           
        }


        /* View  Privacy */
        public IActionResult Privacy()

        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View();
        }

        /* View  Contact */
        public IActionResult Contact()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}