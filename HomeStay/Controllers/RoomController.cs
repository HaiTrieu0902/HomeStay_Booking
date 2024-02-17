using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page, int? categoryId = 0, string searchValue = "", string filter ="")
        {
            /* initial value lọc theo chiều giảm dần và chiều tăng dần */
            var activeStatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "decrease", Text = "Giá giảm dần" },
                new SelectListItem { Value = "increase", Text = "Giá tăng đần" }
            };
            ViewData["ListActiveFilter"] = new SelectList(activeStatusList, "Value", "Text", filter);

            /* start func */
            var pageNumber = page == null || page < 0 ? 1 : page.Value;
            var pageSize = 9;
            IQueryable<Room> roomQuery = _context.Rooms.AsNoTracking().Include(r => r.Category).Where(item => item.Active == true).OrderByDescending(item => item.RoomId);
            if (categoryId != 0)
            {
                roomQuery = roomQuery.Where(item => item.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                roomQuery = roomQuery.Where(item => item.Title.Contains(searchValue) || item.Detail.Contains(searchValue) || item.Area.Contains(searchValue))  ;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "increase")
                {
                    roomQuery = roomQuery.OrderBy(item => item.Price);
                }
                else 
                {
                    roomQuery = roomQuery.OrderByDescending(item => item.Price);
                }

            }
            var listRooms = await roomQuery.ToListAsync();
            var models = new PagedList<Room>(listRooms.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentSearch = searchValue;
            ViewBag.CurrentCategory = categoryId;
            ViewData["ListCategory"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            return View(models);
        }



        public async Task<IActionResult> ListRoom(int? page, int? categoryID = 0, string searchValue = "")
        {
            try
            {
                var pageNumber = page == null || page < 0 ? 1 : page.Value;
                var pageSize = 12;
                IQueryable<Room> roomQuery = _context.Rooms.AsNoTracking().Include(r => r.Category);
                if (categoryID != 0)
                {
                    roomQuery = roomQuery.Where(item => item.CategoryId == categoryID);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roomQuery = roomQuery.Where(item => item.Title.Contains(searchValue) || item.Detail.Contains(searchValue));
                }

                var listRooms = await roomQuery.OrderByDescending(item => item.RoomId).ToListAsync();
                var models = new PagedList<Room>(listRooms.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentSearch = searchValue;
                ViewData["ListCategory"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryID);
                return View(models);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Home");

            }
        }
        public IActionResult Details(int id)
        {
           try {
                var room = _context.Rooms.FirstOrDefault(item => item.RoomId == id);
                if (room != null)
                {
                    return View(room);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }



        
        /* hadnle fillter rooms category */
        public IActionResult FillterRoomsCategory(string filter = "", string searchValue = "" , int? categoryId =0)
        {
            var url = "/Room";
           
            if (!string.IsNullOrEmpty(filter))
            {
                url += $"?filter={filter}";
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                url += utils.AppendSeparator(url) + $"searchValue={searchValue}";
            }
            if (categoryId != 0)
            {
                url += utils.AppendSeparator(url) + $"categoryId={categoryId}";
            }
            return Json(new { status = "Success", redirectUrl = url });
        }


    }
}
