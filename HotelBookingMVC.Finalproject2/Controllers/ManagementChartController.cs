using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.ViewModels;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]

public class ManagementChartController : Controller
{
    private readonly HotelBookingDbContext _context;
    private readonly UserManager<HotelUser> _userManager;

    public ManagementChartController(HotelBookingDbContext context, UserManager<HotelUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var profilePicture = string.IsNullOrEmpty(user.ProfilePictureFileName)
                ? "default.png" // Hình ảnh mặc định nếu không có
                : user.ProfilePictureFileName;

            ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
        }
        else
        {
            ViewData["UserProfilePicture"] = "/uploads/profile_pictures/default.png";
        }
        // Lấy số lượng khách sạn từ HotelBookingDbContext
        var hotelCount = await _context.Hotels.CountAsync();

        // Lấy số lượng khách hàng từ UserManager (vai trò "Customer")
        var customersCount = (await _userManager.GetUsersInRoleAsync("Customer")).Count;

        // Lấy tổng số lượt đặt phòng từ HotelBookingDbContext
        var totalBookings = await _context.Bookings.CountAsync();

        // Gán dữ liệu vào ViewModel
        var viewModel = new ManagementChartViewModel
        {
            HotelCount = hotelCount,
            CustomersCount = customersCount,
            TotalBookings = totalBookings
        };

        return View(viewModel);
    }

    // API to return chart data in JSON format
    [HttpGet]
    public async Task<IActionResult> GetChartData()
    {
        // Lấy dữ liệu khách sạn theo ngày
        var hotelData = await _context.Hotels
            .GroupBy(h => h.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync();

        // Lấy dữ liệu khách hàng (vai trò "Customer") theo ngày
        var customers = await _userManager.GetUsersInRoleAsync("Customer");
        var customerData = customers
            .GroupBy(c => c.DateCreatedAt.Date) // Cần chắc chắn rằng HotelUser có trường CreatedAt
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Lấy dữ liệu đặt phòng theo ngày
        var bookingData = await _context.Bookings
            .GroupBy(b => b.BookingDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync();

        // Chuẩn bị dữ liệu cho biểu đồ
        var chartData = new
        {
            hotels = new
            {
                labels = hotelData.Select(h => h.Date.ToString("yyyy-MM-dd")).ToArray(),
                datasets = new[]
                {
                new
                {
                    label = "Hotels Count",
                    data = hotelData.Select(h => h.Count).ToArray(),
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    borderColor = "rgba(75, 192, 192, 1)",
                    borderWidth = 1
                }
            }
            },
            customers = new
            {
                labels = customerData.Select(c => c.Date.ToString("yyyy-MM-dd")).ToArray(),
                datasets = new[]
                {
                new
                {
                    label = "Customers Count",
                    data = customerData.Select(c => c.Count).ToArray(),
                    backgroundColor = "rgba(153, 102, 255, 0.2)",
                    borderColor = "rgba(153, 102, 255, 1)",
                    borderWidth = 1
                }
            }
            },
            bookings = new
            {
                labels = bookingData.Select(b => b.Date.ToString("yyyy-MM-dd")).ToArray(),
                datasets = new[]
                {
                new
                {
                    label = "Bookings Count",
                    data = bookingData.Select(b => b.Count).ToArray(),
                    backgroundColor = "rgba(255, 159, 64, 0.2)",
                    borderColor = "rgba(255, 159, 64, 1)",
                    borderWidth = 1
                }
            }
            }
        };

        return Json(chartData);
    }
}
