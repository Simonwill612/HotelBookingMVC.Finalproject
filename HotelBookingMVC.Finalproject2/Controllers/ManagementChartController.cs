using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.ViewModels;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Manager")]

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
        // Lấy số lượng khách sạn từ HotelBookingDbContext
        var hotelCount = await _context.Hotels.CountAsync();

        // Lấy số lượng khách hàng từ UserManager (vai trò "User")
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
        // Get hotel data (you can change this logic based on your chart data needs)
        var hotelsData = new
        {
            labels = new[] { "Hotels" },
            datasets = new[]
            {
                new
                {
                    label = "Hotels Count",
                    data = new[] { await _context.Hotels.CountAsync() },
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    borderColor = "rgba(75, 192, 192, 1)",
                    borderWidth = 1
                }
            }
        };

        // Get customer data (you can change this logic based on your chart data needs)
        var customersData = new
        {
            labels = new[] { "Customers" },
            datasets = new[]
            {
                new
                {
                    label = "Customers Count",
                    data = new[] { (await _userManager.GetUsersInRoleAsync("User")).Count },
                    backgroundColor = "rgba(153, 102, 255, 0.2)",
                    borderColor = "rgba(153, 102, 255, 1)",
                    borderWidth = 1
                }
            }
        };

        // Get booking data (you can change this logic based on your chart data needs)
        var bookingsData = new
        {
            labels = new[] { "Bookings" },
            datasets = new[]
            {
                new
                {
                    label = "Bookings Count",
                    data = new[] { await _context.Bookings.CountAsync() },
                    backgroundColor = "rgba(255, 159, 64, 0.2)",
                    borderColor = "rgba(255, 159, 64, 1)",
                    borderWidth = 1
                }
            }
        };

        // Return the data in a JSON format
        return Json(new { hotels = hotelsData, customers = customersData, bookings = bookingsData });
    }
}
