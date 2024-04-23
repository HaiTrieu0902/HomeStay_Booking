using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeStay.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using X.PagedList;
using HomeStay.Helper;
using System.Security.Claims;

namespace HomeStay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomsController : Controller
    {
        private readonly HomestayDBContext _context;
        public INotyfService _notifyService { get; }
        public RoomsController(HomestayDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Rooms
        public async Task<IActionResult> Index(int? page , int? status, int? categoryID ,string searchValue =""  )
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            /* var homestayDBContext = _context.Rooms.Include(r => r.Category);
             return View(await homestayDBContext.ToListAsync());*/

            /* Inital list status */
            var activeStatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Active" },
                new SelectListItem { Value = "0", Text = "Inactive" }
            };
            ViewData["ListActiveStatus"] = new SelectList(activeStatusList, "Value", "Text", status);


            /* start logic */
            var pageNumber = page == null || page < 0 ? 1 : page.Value;
            var pageSize = 10;
            List<Room> rooms = _context.GetListRoom(status,categoryID,searchValue);
            // IQueryable<Room> roomQuery = _context.Rooms.AsNoTracking().Include(r => r.Category);
            // if (categoryID != 0)
            // {
            //     roomQuery = roomQuery.Where(item => item.CategoryId == categoryID);
            // }

            // if (status.HasValue)
            // {
            //     bool isActive = status == 1;
            //     roomQuery = roomQuery.Where(item => item.Active == isActive);
            // }
            // if (!string.IsNullOrEmpty(searchValue))
            // {
            //     roomQuery = roomQuery.Where(item => item.Title.Contains(searchValue) || item.Detail.Contains(searchValue));
            // }

            // var listRooms = await roomQuery.OrderByDescending(item => item.RoomId).ToListAsync();
            var models = new PagedList<Room>(rooms, pageNumber, pageSize);
           /* ViewBag.CurrentPage = pageNumber;*/
            ViewBag.CurrentSearch = searchValue;
            ViewData["ListCategory"] = new SelectList(_context.Categories, "CategoryId", "CategoryName" , categoryID);
            // var customer = await _context.Rooms.FromSqlRaw("SELECT * FROM Room").ToListAsync();
            return View(models);
        }

        // GET: Admin/Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

           // var room = _context.GetRoomById(id);
           // var room = await _context.Rooms.Include(item => item.Category).FindAsync(id);

            var room = await _context.Rooms.AsNoTracking().Include(item => item.Category)
              .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }

            return View(room);
        }

        // GET: Admin/Rooms/Create
        public IActionResult Create()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,Title,Detail,Price,Area,Capacity,Description,Active,Status,Avatar,CategoryId")] Room room , Microsoft.AspNetCore.Http.IFormFile fthumb)
        {
            if (fthumb != null)
            {
                string extention = Path.GetExtension(fthumb.FileName);
                string originalFileName = Path.GetFileNameWithoutExtension(fthumb.FileName);
                string fileName = $"{originalFileName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{extention}";
                room.Avatar = await utils.UploadImage(fthumb, @"rooms", fileName);
                // CreateAt
                // room.CreateAt = DateTime.Now;
            }

            if (string.IsNullOrEmpty(room.Avatar))
            {
                room.Avatar = "default.png";
            }
            _context.CreateRoom(room);

            // _context.Add(room);
            // await _context.SaveChangesAsync();
            _notifyService.Success($"Create rooms successfully");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", room.CategoryId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }
           
            // var room = await _context.Rooms.FindAsync(id);
            var room = _context.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }

            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", room.CategoryId);
            return View(room);
        }

        // POST: Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,Title,Detail,Price,Area,Capacity,Description,Active,Status,Avatar,CategoryId")] Room room , Microsoft.AspNetCore.Http.IFormFile fthumb)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }
            try

            {
                // var roomOlder = _context.Rooms.FromSqlRaw($"SELECT * FROM Room Where RoomId = {id}").AsNoTracking().FirstOrDefault();
                var roomOlder = _context.GetRoomById(id);
                if (fthumb != null)
                {
                    string extention = Path.GetExtension(fthumb.FileName);
                    string originalFileName = Path.GetFileNameWithoutExtension(fthumb.FileName);
                    string fileName = $"{originalFileName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{extention}";
                    room.Avatar = await utils.UploadImage(fthumb, @"rooms", fileName);
                }

                if (string.IsNullOrEmpty(room.Avatar))
                {
                    if(roomOlder.Avatar != "")
                    {
                        room.Avatar = roomOlder.Avatar;
                    } else
                    {
                        room.Avatar = "default.png";
                    }
                }
                _context.UpdateRoom(room);
                //    _context.Update(room);
                //    await _context.SaveChangesAsync();
               _notifyService.Success($"Update rooms successfully ");
            }
            catch (DbUpdateConcurrencyException)
            {   
                if (!RoomExists(room.RoomId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", room.CategoryId);
            return RedirectToAction(nameof(Index));
         

        }

        // GET: Admin/Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = _context.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }

            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }

            return View(room);
        }

        // POST: Admin/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'HomestayDBContext.Rooms'  is null.");
            }
            var room = _context.GetRoomById(id);
            if (room != null)
            {
                // _context.Rooms.Remove(room);
                _context.DeleteRoom(id);
            }
            
            // await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        /* handle delete room not view */
        public async Task<IActionResult> DeleteRoomNotView(int id)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'HomestayDBContext.Rooms'  is null.");
            }
            var room = _context.GetRoomById(id);
            if (room != null)
            {
                // _context.Rooms.Remove(room);
                _context.DeleteRoom(id);
                _notifyService.Success("Delete Complete");
            }
            
            // await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        /* handle filter category */
        /*       public IActionResult FillterRoomsCategory(int categoryId = 0, int status = -1)
               {
                   var url = $"/Admin/Rooms?categoryId={categoryId}";

                   if (categoryId == 0 && status == -1)
                   {
                       url = $"/Admin/Rooms";
                   }
                   else if (categoryId != 0 && status != -1)
                   {
                       url = $"/Admin/Rooms?categoryId={categoryId}&status={status}";
                   }
                   else if (status != -1)
                   {
                       url = $"/Admin/Rooms?status={status}";
                   }

                   return Json(new { status = "Success", redirectUrl = url });
               }*/
        public IActionResult FillterRoomsCategory(int categoryId = 0, int status = -1, string searchValue = "")
        {
            var url = "/Admin/Rooms";
            if (categoryId != 0)
            {
                url += $"?categoryId={categoryId}";
            }

            if (status != -1)
            {
                url += utils.AppendSeparator(url) + $"status={status}";
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                url += utils.AppendSeparator(url) + $"searchValue={searchValue}";
            }
            return Json(new { status = "Success", redirectUrl = url });
        }
      
        private bool RoomExists(int id)
        {
          return (_context.Rooms?.Any(e => e.RoomId == id)).GetValueOrDefault();
        }
    }
}
