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

        public async Task<IActionResult> Index(string state = "", string city = "")
        {
            if (string.IsNullOrEmpty(state) && string.IsNullOrEmpty(city)) {
                var hotelViewModels = await _context.Hotels
                    .Include(h => h.HotelMediaDetails)
                    .Select(h => new HotelViewModel
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
                    }).ToListAsync();

                var hotels = await _context.Hotels
                                           .Where(h => h.State.Contains(state) && h.City.Contains(city))
                                           .Include(h => h.HotelMediaDetails)
                                                .ThenInclude(h => h.Media)
                                           .ToListAsync();
                return View(hotelViewModels);

            }
            else
            {
                var hotels = await _context.Hotels
                                       .Where(h => h.State.Contains(state) && h.City.Contains(city))
                                       .Include(h => h.HotelMediaDetails)
                                            .ThenInclude(h => h.Media)
                                       .ToListAsync();
                var hotelViewModels = hotels.Select(h => new HotelViewModel
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
                    //Media = h.Media.ToList()
                    Media = h.HotelMediaDetails
                            .Select(rmd => new MediaViewModel
                            {
                                MediaID = rmd.MediaId,
                                FileName = rmd.Media.FileName,
                                FilePath = rmd.Media.FilePath,
                                MediaType = rmd.Media.MediaType
                            }).ToList()
                }).ToList();

                return View(hotelViewModels);
            }



            
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

            var hotelViewModels = hotels.Select(h => new HotelViewModel
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
            }).ToList();

            return View("Index", hotelViewModels);
        }


    }
}
