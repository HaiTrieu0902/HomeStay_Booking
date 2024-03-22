using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeStay.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Security.Claims;
using HomeStay.Helper;

namespace HomeStay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminRolesController : Controller
    {
        private readonly HomestayDBContext _context;
        HomestayDBContext db = new HomestayDBContext();
        public INotyfService _notifyService { get; }


        public AdminRolesController(HomestayDBContext context , INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminRoles
        public async Task<IActionResult> Index()
        {
            try
            {
                // Thực hiện truy vấn dữ liệu bằng SQL thuần
                /*   var roles = await _context.Roles.FromSqlRaw("SELECT * FROM Roles Where RoleId = 1").ToListAsync();*/
                /*  return _context.Roles != null ?
                       View(await _context.Roles.ToListAsync()) :
                       Problem("Entity set 'HomestayDBContext.Roles'  is null.");*/

                var roles = await _context.Roles.FromSqlRaw("SELECT * FROM Roles").ToListAsync();
                var userClaims = User.Identity as ClaimsIdentity;
                if (userClaims != null)
                {
                    userClaims.SetUserClaims(TempData);
                }
                return View (roles);
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred: {ex.Message}");
            }
        }

        // GET: Admin/AdminRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
              
            }
            var role = await _context.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }


            return View(role);
        }

        // GET: Admin/AdminRoles/Create
        public IActionResult Create()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View();
        }

        // POST: Admin/AdminRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                // _context.Add(role);
                // await _context.SaveChangesAsync();
                _context.CreateRole(role);
                _notifyService.Success("Create sucessfully");
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/AdminRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.GetRoleById(id);
        
            if (role == null)
            {
                return NotFound();
            }

            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }

            return View(role);
        }

        // POST: Admin/AdminRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleName")] Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateRole(role);
                    _notifyService.Success("Edit sucessfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                     
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/AdminRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/AdminRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'HomestayDBContext.Roles'  is null.");
            }
            var role = await _context.GetRoleById(id);
            if (role != null)
            {
                _context.DeleteRole(id);
            }
            
            // await _context.SaveChangesAsync();
            _notifyService.Success("Delete sucessfully");
            return RedirectToAction(nameof(Index));
        }



        /* Function hanlde delete value Not User View */
        public async Task<IActionResult> DeleteRoleNotView(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'HomestayDBContext.Roles'  is null.");
            }
            var role = await _context.GetRoleById(id);
            if (role != null)
            {
                _context.DeleteRole(id);
            }
            
            // await _context.SaveChangesAsync();
            _notifyService.Success("Delete sucessfully");
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
          return (_context.Roles?.Any(e => e.RoleId == id)).GetValueOrDefault();
        }
    }
}
