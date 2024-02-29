using AspNetCoreHero.ToastNotification.Abstractions;
using HomeStay.Helper;
using HomeStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;
using X.PagedList;


namespace HomeStay.Controllers
{
    public class RoomController : Controller
    {

        private readonly HomestayDBContext _context;
        HomestayDBContext db = new HomestayDBContext();
        private INotyfService _notifyService { get; }
        public RoomController(HomestayDBContext context , INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
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

            /* get người dùng login */
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }

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



        /* get ListRoom Room */
        [HttpGet]
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

        /* get Details Room */
        [HttpGet]
        public IActionResult Details(int id)
        {
           try {

                /* get người dùng login */
                var userClaims = User.Identity as ClaimsIdentity;
                if (userClaims != null)
                {
                    userClaims.SetUserClaims(TempData);
                }
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


        /* get FavoriteRooms */
        [HttpGet]
        public IActionResult FavoriteRooms()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
            }
            return View();
        }

        /* handle fillter rooms category */
        [HttpGet]
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


        // class data when add 
       


        /* handle add to favorite room */
        [HttpPost]
        [Route("Room/AddFavoriteRoomsByUser")]
        public async Task<IActionResult> AddFavoriteRoomsByUser([FromBody] FavouriteClone favouriteRoom)
        {
            try {
                FavouriteRoom newFavourite = new FavouriteRoom
                 {
                    CustomerId = favouriteRoom.CustomerId,
                      RoomId = favouriteRoom.RoomId,
                      Area = favouriteRoom.Area,
                      Avatar = favouriteRoom.Avatar,
                      Detail = favouriteRoom.Detail,
                      Price = favouriteRoom.Price,
                      Title = favouriteRoom.Title,
                       /* CustomerId = 19,
                        RoomId =44,
                        Area = "Cà Mau city",
                        Avatar = "SP020_20240115164812529.jpeg",
                        Detail = "detail nha hihiihi",
                        Price = 950000,
                        Title = "Buon ngu qua",*/
                };

                var duplicateRoomId = _context.FavouriteRooms.AsNoTracking().SingleOrDefault(item => item.RoomId == favouriteRoom.RoomId && item.CustomerId == favouriteRoom.CustomerId);
                if (duplicateRoomId != null )
                {
                     return Json(new { status = "Error", redirectUrl = "/Room" , message = "Phòng này bạn đã lưu nó vào danh sách yêu thích!!" } );
                }
                else
                  {
                    _context.FavouriteRooms.Add(newFavourite);
                    await _context.SaveChangesAsync();
                    _notifyService.Success($"Create customer sucessfully ");
                    
                    return Json(new { status = "Success", redirectUrl = "/Room" });

                  }
              
            }
            catch (Exception ex)
            {
                _notifyService.Error($"Lỗi khi thêm vào yêu thích");
                return Json(new { status = "Error", redirectUrl = "/Room" , message = "Đã có lỗi xảy ra ở đây" });
            }
        }

        public class FavouriteClone
        {
            public int CustomerId { get; set; }
            public int RoomId { get; set; }
            public string Title { get; set; } = null!;
            public double Price { get; set; }
            public string Detail { get; set; } = null!;
            public string Area { get; set; } = null!;
            public string Avatar { get; set; } = null!;
        }

    }
   
}
