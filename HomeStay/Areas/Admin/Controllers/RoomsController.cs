﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeStay.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page , int? status, int? categoryID = 0   )
        {
            /* var homestayDBContext = _context.Rooms.Include(r => r.Category);
             return View(await homestayDBContext.ToListAsync());*/

            /* Inital list status */
            var activeStatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Active" },
                new SelectListItem { Value = "0", Text = "Inactive" }
            };
            ViewData["ListActiveStatus"] = new SelectList(activeStatusList, "Value", "Text", status);

            var pageNumber = page == null || page < 0 ? 1 : page.Value;
             var pageSize = 10;
             List<Room> listRooms = new List<Room>();
            if(categoryID != 0)
            {
                listRooms = _context.Rooms.AsNoTracking().Where(item=>item.CategoryId ==categoryID).Include(r => r.Category).OrderByDescending(item => item.RoomId).ToList();
            } else
            {
                listRooms = _context.Rooms.AsNoTracking().Include(r => r.Category).OrderByDescending(item => item.RoomId).ToList();
            }
            PagedList<Room> models = new PagedList<Room>(listRooms.AsQueryable(), pageNumber, pageSize);


            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCategory = categoryID;
           
            ViewData["ListCategory"] = new SelectList(_context.Categories, "CategoryId", "CategoryName" , categoryID);
            var customer = await _context.Rooms.FromSqlRaw("SELECT * FROM Room").ToListAsync();
            return View(models);
        }

        // GET: Admin/Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Admin/Rooms/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,Title,Detail,Price,Area,Capacity,Description,Active,Status,Avatar,CategoryId")] Room room)
        {
            _context.Add(room);
            await _context.SaveChangesAsync();
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
           
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", room.CategoryId);
            return View(room);
        }

        // POST: Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,Title,Detail,Price,Area,Capacity,Description,Active,Status,Avatar,CategoryId")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }
            try
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                _notifyService.Success($"Update rooms successfully");
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

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
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
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        /* handle delete room not view */
        public async Task<IActionResult> DeleteRoomNotView(int id)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'HomestayDBContext.Rooms'  is null.");
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _notifyService.Success($"Delete rooms successfully");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        /* handle filter category */
        /*  public IActionResult FillterRoomsCategory(int category = 0)
          {
              var url = $"/Admin/Rooms?categoryId={category}";
              if(category == 0)
              {
                  url = $"/Admin/Rooms";
              }
              return Json(new {status = "Success" , redirectUrl = url});
          }*/
        public IActionResult FillterRoomsCategory(int categoryId = 0, int status = -1)
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
        }


        public IActionResult FillterRoomsActive(int active = 0)
        {
            return View();
        }




        private bool RoomExists(int id)
        {
          return (_context.Rooms?.Any(e => e.RoomId == id)).GetValueOrDefault();
        }
    }
}