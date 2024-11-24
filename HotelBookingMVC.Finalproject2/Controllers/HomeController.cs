using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelBookingDbContext _context;

        public HomeController(ILogger<HomeController> logger, HotelBookingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Nhóm khách sạn theo từng thành phố
            var groupedHotels = await _context.Hotels
                .Include(h => h.HotelMediaDetails)
                .ThenInclude(h => h.Media)
                .GroupBy(h => h.City)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(h => new HotelViewModel
                    {
                        HotelID = h.HotelID,
                        Name = h.Name,
                        Address = h.Address,
                        City = h.City,
                        State = h.State,
                        ZipCode = h.ZipCode,
                        PhoneNumber = h.PhoneNumber,
                        Email = h.Email,
                        Description = h.Description,
                        CreatedAt = h.CreatedAt,
                        UpdatedAt = h.UpdatedAt,
                        Media = h.HotelMediaDetails
                            .Select(rmd => new MediaViewModel
                            {
                                MediaID = rmd.MediaId,
                                FileName = rmd.Media.FileName,
                                FilePath = rmd.Media.FilePath,
                                MediaType = rmd.Media.MediaType
                            }).ToList()
                    }).ToList());

            return View(groupedHotels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string city = "")
        {
            var hotelsQuery = _context.Hotels
                .Include(h => h.HotelMediaDetails)
                .ThenInclude(h => h.Media)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                hotelsQuery = hotelsQuery.Where(h => h.City.Contains(city));
            }

            var hotels = await hotelsQuery.ToListAsync();

            // Nhóm khách sạn theo thành phố giống như trong action Index
            var groupedHotels = hotels.GroupBy(h => h.City)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(h => new HotelViewModel
                    {
                        HotelID = h.HotelID,
                        Name = h.Name,
                        Address = h.Address,
                        City = h.City,
                        State = h.State,
                        ZipCode = h.ZipCode,
                        PhoneNumber = h.PhoneNumber,
                        Email = h.Email,
                        Description = h.Description,
                        CreatedAt = h.CreatedAt,
                        UpdatedAt = h.UpdatedAt,
                        Media = h.HotelMediaDetails
                            .Select(rmd => new MediaViewModel
                            {
                                MediaID = rmd.MediaId,
                                FileName = rmd.Media.FileName,
                                FilePath = rmd.Media.FilePath,
                                MediaType = rmd.Media.MediaType
                            }).ToList()
                    }).ToList());

            return View("Index", groupedHotels);
        }

        // GET: Hotels/Details/{id}
        public async Task<IActionResult> Details(Guid? id, Guid? roomId)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin khách sạn
            var hotel = await _context.Hotels
                .Include(h => h.HotelMediaDetails)
                .ThenInclude(hmd => hmd.Media)
                .FirstOrDefaultAsync(h => h.HotelID == id);

            if (hotel == null)
            {
                return NotFound();
            }

            // Lấy danh sách phòng
            var rooms = await _context.Rooms
                .Where(r => r.HotelID == id)
                .Include(r => r.RoomMediaDetails)
                .ThenInclude(rmd => rmd.Media)
                .ToListAsync();

            // Nếu có roomId, lấy chi tiết phòng
            var selectedRoom = roomId != null
                ? await _context.Rooms
                    .Include(r => r.RoomMediaDetails)
                    .ThenInclude(rmd => rmd.Media)
                    .FirstOrDefaultAsync(r => r.RoomID == roomId)
                : null;

            // Truyền thông tin khách sạn vào ViewBag
            ViewBag.Hotel = new
            {
                hotel.HotelID,
                hotel.Name,
                hotel.Address,
                hotel.City,
                hotel.State,
                hotel.ZipCode,
                hotel.PhoneNumber,
                hotel.Email,
                hotel.Description,
                hotel.CreatedAt,
                hotel.UpdatedAt,
                Media = hotel.HotelMediaDetails.Select(m => new
                {
                    m.Media.FileName,
                    m.Media.FilePath,
                    m.Media.MediaType
                })
            };

            // Truyền danh sách phòng vào ViewBag
            ViewBag.Rooms = rooms.Select(r => new
            {
                r.RoomID,
                r.HotelID,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                r.Description,
                r.Availability,
                r.DateCreatedAt,
                r.DateUpdatedAt,
                Media = r.RoomMediaDetails.Select(rmd => new
                {
                    rmd.Media.FileName,
                    rmd.Media.FilePath,
                    rmd.Media.MediaType
                })
            }).ToList();

            // Truyền chi tiết phòng được chọn (nếu có)
            if (selectedRoom != null)
            {
                ViewBag.SelectedRoom = new
                {
                    selectedRoom.RoomID,
                    selectedRoom.HotelID,
                    selectedRoom.RoomNumber,
                    selectedRoom.Type,
                    selectedRoom.PricePerNight,
                    selectedRoom.Description,
                    selectedRoom.Availability,
                    selectedRoom.DateCreatedAt,
                    selectedRoom.DateUpdatedAt,
                    Media = selectedRoom.RoomMediaDetails.Select(rmd => new
                    {
                        rmd.Media.FileName,
                        rmd.Media.FilePath,
                        rmd.Media.MediaType
                    })
                };
            }

            return View();
        }





    }
}
