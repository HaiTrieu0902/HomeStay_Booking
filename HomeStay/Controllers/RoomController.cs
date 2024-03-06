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
                    ViewData["ListPayment"] = new SelectList(_context.Payments, "PaymentId", "BankName");
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
        public async Task<IActionResult> FavoriteRooms(int? page)
        {
            var userClaims = User.Identity as ClaimsIdentity;
            if (userClaims != null)
            {
                userClaims.SetUserClaims(TempData);
                var idLogin = userClaims.FindFirst("CustomerId");

                if(idLogin != null)
                {
                    var pageNumber = page == null || page < 0 ? 1 : page.Value;
                    var pageSize = 6;
                    IQueryable<FavouriteRoom> roomFavouriteQuery = _context.FavouriteRooms.AsNoTracking().Where(item => item.CustomerId == Convert.ToInt32(idLogin.Value)).OrderByDescending(item => item.RoomId);
                    var listRoomsFavourite = await roomFavouriteQuery.ToListAsync();

                    ViewBag.CountFavorite = listRoomsFavourite != null ? listRoomsFavourite.Count : 0;
                    var models = new PagedList<FavouriteRoom>(listRoomsFavourite.AsQueryable(), pageNumber, pageSize);
                    return View(models);
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }


        /* remove favourite room */
        [HttpPost]
        public async Task<IActionResult> RemoveFavoriteRooms(int id)
        {
            if (_context.FavouriteRooms == null)
            {
                return Problem("Entity set 'HomestayDBContext.FavouriteRooms'  is null.");
            }
            var favoriteRooms = await _context.FavouriteRooms.FindAsync(id);
            if (favoriteRooms != null)
            {
                _context.FavouriteRooms.Remove(favoriteRooms);
                _notifyService.Success($"Xóa phòng ra khỏi danh sách yêu thích thành công");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("FavoriteRooms", "Room");
        }


        /* create booking for user */
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingItemCLone bookingItem)
        {
            try
            {
                Booking newBooking = new Booking
                {
                   RoomId = bookingItem.RoomId,
                   PaymentId = bookingItem.PaymentId,
                   CheckIn = bookingItem.CheckIn,
                   CheckOut = bookingItem.CheckOut,
                   CountNight = bookingItem.CountNight,
                   AmountOfPeople = bookingItem.AmountOfPeople,
                   CustomerId = bookingItem.CustomerId,
                   TotalAmount = bookingItem.TotalAmount
                };
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();
                _notifyService.Success($"Bạn đã booking thành công");
                return Json(new { status = "Success", redirectUrl = $"/Room" });

            }
            catch (Exception ex)
            {
                _notifyService.Error($"Lỗi khi đặt phòng");
                return Json(new { status = "Success", redirectUrl = $"/Room/Details/{bookingItem.RoomId}" });
            }
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
                };
                var duplicateRoomId = _context.FavouriteRooms.AsNoTracking().SingleOrDefault(item => item.RoomId == favouriteRoom.RoomId && item.CustomerId == favouriteRoom.CustomerId);
                if (duplicateRoomId != null )
                {
                    _notifyService.Error($"Phòng này bạn đã lưu nó vào danh sách yêu thích!!");
                    return Json(new { status = "Error", redirectUrl = "/Room" , message = "Phòng này bạn đã lưu nó vào danh sách yêu thích!!" } );
                }
                else
                  {
                    _context.FavouriteRooms.Add(newFavourite);
                    await _context.SaveChangesAsync();
                    _notifyService.Success($"Thêm vào yêu thích thành công");
                    return Json(new { status = "Success", redirectUrl = "/Room" });

                  }
              
            }
            catch (Exception ex)
            {
                _notifyService.Error($"Lỗi khi thêm vào yêu thích");
                return Json(new { status = "Error", redirectUrl = "/Room" , message = "Đã có lỗi xảy ra ở đây" });
            }
        }


        // clone class when add if sometime has error 
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


        public class BookingItemCLone
        {
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
