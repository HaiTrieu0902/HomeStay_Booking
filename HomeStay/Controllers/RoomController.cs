using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeStay.Controllers
{
    public class RoomController : Controller
    {

        private readonly HomestayDBContext _context;
        HomestayDBContext db = new HomestayDBContext();
        public RoomController(HomestayDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            var room = _context.Rooms.FirstOrDefault(item => item.RoomId == id);
            if (room != null) 
            {
               return RedirectToAction("Index");

            } else
            {
                return View(room);
            }
        }

    }
}
