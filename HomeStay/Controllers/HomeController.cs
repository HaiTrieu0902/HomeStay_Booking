using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HomeStay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomestayDBContext _context;
        HomestayDBContext db = new HomestayDBContext();
        public HomeController(ILogger<HomeController> logger , HomestayDBContext context)
        {
            _logger = logger;
            _context = context;
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
            IQueryable<Room> roomQuery = _context.Rooms
                                                .AsNoTracking()
                                                .Include(r => r.Category)
                                                .Where(item => item.Active == true)
                                                .OrderByDescending(item => item.RoomId)
                                                .Take(10);
            var listRooms = await roomQuery.ToListAsync();
            ViewBag.ListRooms = listRooms;
            return View();
        }

        /* View  Privacy */
        public IActionResult Privacy()
        {
            return View();
        }

        /* View  Contact */
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}