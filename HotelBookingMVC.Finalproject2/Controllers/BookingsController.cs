using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Models;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Manager")]

    public class BookingsController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager;

        public BookingsController(UserManager<HotelUser> userManager, HotelBookingDbContext context)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Bookings
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
            var bookings = await _context.Bookings
                .Include(b => b.Room) // Include related Room entity
                .ToListAsync();

            // Map Booking entities to BookingViewModel
            var bookingViewModels = bookings.Select(b => new BookingViewModel
            {
                BookingID = b.BookingID,
                UserID = b.UserID,
                RoomID = b.RoomID,
                RoomNumber = b.Room?.RoomNumber,
                BookingDate = b.BookingDate,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalPrice = b.TotalPrice,
                Status = b.Status
            }).ToList();

            return View(bookingViewModels);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
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
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Map the Booking entity to BookingViewModel
            var bookingViewModel = new BookingViewModel
            {
                BookingID = booking.BookingID,
                RoomNumber = booking.Room.RoomNumber,
                BookingDate = booking.BookingDate,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                // Add other properties as needed
            };

            return View(bookingViewModel);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Find the booking by ID
            var booking = await _context.Bookings
                .Include(b => b.Payments)  // Make sure to include related payments
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking != null)
            {
                // Delete related payments
                if (booking.Payments != null)
                {
                    _context.Payments.RemoveRange(booking.Payments); // Remove all payments associated with this booking
                }

                // Remove the booking
                _context.Bookings.Remove(booking);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool BookingExists(Guid id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
     
        public async Task<IActionResult> Confirmation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var profilePicture = string.IsNullOrEmpty(user.ProfilePictureFileName)
                    ? "default.png" // Hình ảnh mặc định
                    : user.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }

            return View();
        }
    }
}
