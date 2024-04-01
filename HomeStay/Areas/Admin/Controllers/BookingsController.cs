using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace HomeStay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingsController : Controller
    {
       
        private readonly HomestayDBContext _context;
        private INotyfService _notifyService { get; }


        public BookingsController(HomestayDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page < 0 ? 1 : page.Value;
            var pageSize = 10;
            var listBookings = _context.Bookings.AsNoTracking().Include(item => item.Customer).Include(item => item.Payment).Include(item => item.Room).OrderByDescending(item => item.BookingId);
            PagedList<Booking> models = new PagedList<Booking>(listBookings, pageNumber, pageSize);
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View(models);
        }



       
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.AsNoTracking().Include(item => item.Customer).Include(item => item.Payment).Include(item => item.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            ViewData["ListCustomer"] = new SelectList(_context.Customers, "CustomerId", "FullName", booking.CustomerId);
            ViewData["ListPayment"] = new SelectList(_context.Payments, "PaymentId", "PaymentMethod", booking.PaymentId);
            ViewData["ListRoom"] = new SelectList(_context.Rooms, "RoomId", "Title", booking.RoomId);
            return View(booking);
        }


        [HttpPost]
        public async Task<IActionResult> EditBookingAdmin( [FromBody] BookingItemCLone booking)
        {
            try
            {
                Booking updateBooking = new Booking
                {
                    BookingId = booking.BookingId,
                    RoomId = booking.RoomId,
                    PaymentId = booking.PaymentId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    CountNight = booking.CountNight,
                    AmountOfPeople = booking.AmountOfPeople,
                    CustomerId = booking.CustomerId,
                    TotalAmount = booking.TotalAmount
                };
                _context.Bookings.Update(updateBooking);
                await _context.SaveChangesAsync();
                _notifyService.Success($"Bạn đã booking thành công");
                return Json(new { status = "Success", redirectUrl = $"/Admin/Bookings" });

            }
            catch (Exception ex)
            {
                _notifyService.Error($"Đã có lỗi xảy ra khi đặt phòng");
                return Json(new { status = "Success", redirectUrl = $"/Admin/Bookings/Edit/{booking.BookingId}" });
            }
        }

        private bool BookingsExists(int id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }

        public class BookingItemCLone
        {
            public int BookingId { get; set; }
            public int? RoomId { get; set; }
            public int CustomerId { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
            public int AmountOfPeople { get; set; }
            public int PaymentId { get; set; }
            public double TotalAmount { get; set; }
            public int CountNight { get; set; }

        }
    }
}
