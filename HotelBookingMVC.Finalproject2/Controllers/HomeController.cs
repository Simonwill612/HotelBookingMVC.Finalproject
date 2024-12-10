using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingMVC.Finalproject2.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, HotelBookingDbContext context, UserManager<HotelUser> userManager, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;

        }

        // Hiển thị danh sách khách sạn được nhóm theo thành phố
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

        // GET: Home/CreateFeedback
        [HttpGet]
        public IActionResult CreateFeedback(Guid hotelId)
        {
            var model = new FeedbackViewModel { HotelID = hotelId };
            return PartialView("_CreateFeedbackPartial", model);
        }



        // POST: Home/CreateFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeedback([Bind("HotelID,StarRating,Comment")] FeedbackViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Lưu thông báo yêu cầu đăng nhập vào TempData
                TempData["ErrorMessage"] = "You must log in to submit feedback.";
                return Redirect("/Identity/Account/Login"); // Chuyển hướng đến trang đăng nhập
            }

            var feedback = new Feedback
            {
                FeedbackID = Guid.NewGuid(),
                HotelID = model.HotelID,
                StarRating = model.StarRating,
                Comment = model.Comment,
                UserID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                DateCreated = DateTime.Now
            };

            _context.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.HotelID });
        }


        // show all hotels
        public async Task<IActionResult> Hotels(string[] selectedStates, int page = 1, int pageSize = 6)
        {
            var query = _context.Hotels.AsQueryable();

            // Lọc theo state nếu có
            if (selectedStates != null && selectedStates.Any())
            {
                query = query.Where(h => selectedStates.Contains(h.State));
            }

            // Tính tổng số khách sạn sau khi lọc
            var totalHotels = await query.CountAsync();

            // Lấy dữ liệu cho trang hiện tại
            var hotels = await query
                .Include(h => h.HotelMediaDetails)
                .ThenInclude(h => h.Media)
                .OrderBy(h => h.Name) // Sắp xếp theo tên (hoặc tuỳ chỉnh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    Media = h.HotelMediaDetails.Select(hmd => new MediaViewModel
                    {
                        MediaID = hmd.MediaId,
                        FileName = hmd.Media.FileName,
                        FilePath = hmd.Media.FilePath,
                        MediaType = hmd.Media.MediaType
                    }).ToList()
                }).ToListAsync();

            // Truyền thông tin phân trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalHotels / pageSize);

            // Lấy danh sách các state cho bộ lọc
            ViewBag.States = await _context.Hotels
                .Select(h => h.State)
                .Distinct()
                .ToListAsync();

            return View(hotels);
        }



        // search hotels for city
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

            var hotels = await hotelsQuery.Select(h => new HotelViewModel
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
                Media = h.HotelMediaDetails.Select(hmd => new MediaViewModel
                {
                    MediaID = hmd.MediaId,
                    FileName = hmd.Media.FileName,
                    FilePath = Path.Combine("/media/hotel",
                        hmd.Media.MediaType == MediaType.Image ? "img" : "video",
                        hmd.Media.FileName),
                    MediaType = hmd.Media.MediaType
                }).ToList()
            }).ToListAsync();

            return View("SearchResults", hotels);
        }



        // Hiển thị chi tiết khách sạn
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

            // Lấy danh sách feedbacks liên quan đến khách sạn
            var feedbacks = await _context.Feedbacks
                .Where(f => f.HotelID == id)
                .Include(f => f.Hotel)
                .ToListAsync();

            var feedbackViewModels = feedbacks.Select(async feedback => new FeedbackViewModel
            {
                FeedbackID = feedback.FeedbackID,
                HotelID = feedback.HotelID,
                HotelName = feedback.Hotel.Name,
                StarRating = feedback.StarRating,
                Comment = feedback.Comment,
                UserID = feedback.UserID,
                UserEmail = (await _userManager.FindByIdAsync(feedback.UserID))?.Email ?? "Unknown",
                DateCreated = feedback.DateCreated
            }).Select(t => t.Result).ToList();

            // Truyền feedbacks vào ViewBag
            ViewBag.Feedbacks = feedbackViewModels;

            // Lấy danh sách phòng
            var rooms = await _context.Rooms
                .Where(r => r.HotelID == id)
                .Include(r => r.RoomMediaDetails)
                .ThenInclude(rmd => rmd.Media)
                .Select(r => new
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
                        FilePath = Path.Combine("/media/room",
                            rmd.Media.MediaType == MediaType.Image ? "img" : "video",
                            rmd.Media.FileName),
                        rmd.Media.MediaType
                    }).ToList()
                }).ToListAsync();

            // Lấy ngày hiện tại (DateOnly)
            var today = DateTime.Today;

            // Lấy danh sách RoomID từ bảng Booking theo ngày
            var bookedRooms = await _context.Bookings
                .Where(b => b.CheckInDate.Date <= today && b.CheckOutDate.Date >= today)
                .Select(b => b.RoomID)
                .ToListAsync();

            // Cập nhật trạng thái khả dụng của từng phòng
            var updatedRooms = rooms.Select(r => new
            {
                r.RoomID,
                r.HotelID,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                r.Description,
                Availability = !bookedRooms.Contains(r.RoomID), // Trạng thái khả dụng
                r.DateCreatedAt,
                r.DateUpdatedAt,
                r.Media
            }).ToList();

            // Số phòng khả dụng
            var availableRooms = updatedRooms.Count(r => r.Availability);

            // Nếu có roomId, lấy chi tiết phòng
            var selectedRoom = roomId != null
                ? updatedRooms.FirstOrDefault(r => r.RoomID == roomId)
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
                Media = hotel.HotelMediaDetails.Select(hmd => new
                {
                    hmd.Media.FileName,
                    FilePath = Path.Combine("/media/hotel",
                        hmd.Media.MediaType == MediaType.Image ? "img" : "video",
                        hmd.Media.FileName),
                    hmd.Media.MediaType
                }).ToList()
            };

            // Truyền danh sách phòng và số phòng khả dụng vào ViewBag
            ViewBag.Rooms = updatedRooms;
            ViewBag.AvailableRooms = availableRooms;

            // Truyền chi tiết phòng được chọn (nếu có)
            if (selectedRoom != null)
            {
                ViewBag.SelectedRoom = selectedRoom;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactUsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get list of admin users
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminEmails = adminUsers.Select(u => u.Email).ToList();

            // Prepare email content
            string subject = model.Subject;
            string body = $@"
            <p><strong>Name:</strong> {model.Name}</p>
            <p><strong>Email:</strong> {model.Email}</p>
            <p><strong>Message:</strong></p>
            <p>{model.Message}</p>";

            // Send email to all admins
            foreach (var adminEmail in adminEmails)
            {
                await _emailSender.SendEmailAsync(adminEmail, subject, body);
            }

            TempData["SuccessMessage"] = "Your message has been sent to all admins successfully!";
            return RedirectToAction("Contact");
        }


        public IActionResult AboutUs()
        {
            return View();
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
    }
}
