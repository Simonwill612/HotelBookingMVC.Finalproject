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
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                //.Include(h => h.Media)
                .Include(h => h.HotelMediaDetails)
                .FirstOrDefaultAsync(m => m.HotelID == id);

            if (hotel == null)
            {
                return NotFound();
            }

            var hotelViewModel = new HotelViewModel
            {
                HotelID = hotel.HotelID,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                State = hotel.State,
                ZipCode = hotel.ZipCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Description = hotel.Description,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt,
                Media = hotel.HotelMediaDetails
                    .Select(m => new MediaViewModel(m.Media))
                    .ToList()
            };

            return View(hotelViewModel);
        }



    }
}
