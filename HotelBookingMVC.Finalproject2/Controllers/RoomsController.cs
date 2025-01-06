using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Manager")]

    public class RoomsController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HotelsController> _logger;
        private readonly UserManager<HotelUser> _userManager;


        public RoomsController(HotelBookingDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<HotelUser> userManager, ILogger<HotelsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(Guid hotelId)
        {
            // Lấy thông tin người dùng hiện tại và gán hình ảnh vào ViewData
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

            // Lấy danh sách phòng và kiểm tra tính khả dụng
            var currentDate = DateTime.Now;  // Lấy ngày hiện tại để kiểm tra tính khả dụng

            // Lấy danh sách phòng đã được đặt trong ngày hiện tại
            var bookings = await _context.Bookings
                .Where(b => b.Room.HotelID == hotelId &&
                            (b.CheckInDate <= currentDate && b.CheckOutDate >= currentDate))
                .Select(b => b.RoomID)
                .ToListAsync();

            // Lấy danh sách phòng và kèm thông tin media
            var roomViewModels = await _context.Rooms
                .Where(r => r.HotelID == hotelId)
                .Include(r => r.RoomMediaDetails)
                .ThenInclude(rmd => rmd.Media)
                .Select(r => new RoomViewModel
                {
                    RoomID = r.RoomID,
                    HotelID = r.HotelID,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type,
                    PricePerNight = r.PricePerNight,
                    Description = r.Description,
                    Availability = !bookings.Contains(r.RoomID),  // Kiểm tra phòng có khả dụng không
                    DateCreatedAt = r.DateCreatedAt,
                    DateUpdatedAt = r.DateUpdatedAt,
                    MediaViewModels = r.RoomMediaDetails
                        .Select(rmd => new MediaViewModel
                        {
                            MediaID = rmd.MediaId,
                            FileName = rmd.Media.FileName,
                            FilePath = rmd.Media.FilePath,
                            MediaType = rmd.Media.MediaType,
                        })
                        .ToList()
                })
                .ToListAsync();

            // Truyền tên khách sạn và ID khách sạn vào ViewBag
            ViewBag.HotelName = await _context.Hotels
                .Where(h => h.HotelID == hotelId)
                .Select(h => h.Name)
                .FirstOrDefaultAsync();
            ViewBag.HotelID = hotelId;

            return View(roomViewModels);
        }




        public async Task<IActionResult> Create(Guid hotelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var profilePicture = string.IsNullOrEmpty(user.ProfilePictureFileName)
                    ? "default.png" 
                    : user.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }
            else
            {
                ViewData["UserProfilePicture"] = "/uploads/profile_pictures/default.png";
            }

            // Tạo view model với HotelID được truyền vào
            var viewModel = new RoomViewModel
            {
                HotelID = hotelId
            };

            // Truyền thêm HotelID vào ViewBag (nếu cần)
            ViewBag.HotelID = hotelId;

            return View(viewModel);
        }


        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel roomViewModel)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    RoomID = Guid.NewGuid(),
                    HotelID = roomViewModel.HotelID,
                    RoomNumber = roomViewModel.RoomNumber,
                    Type = roomViewModel.Type,
                    PricePerNight = roomViewModel.PricePerNight,
                    Description = roomViewModel.Description,
                    Availability = roomViewModel.Availability,
                    DateCreatedAt = DateTime.Now,
                    DateUpdatedAt = DateTime.Now
                };

                _context.Add(room);
                await _context.SaveChangesAsync();

                if (roomViewModel.ImageFiles != null)
                {
                    await SaveFileRoomMediaDetails(room.HotelID, room.RoomID, roomViewModel.ImageFiles, MediaType.Image);
                }
                if (roomViewModel.VideoFiles != null)
                {
                    await SaveFileRoomMediaDetails(room.HotelID, room.RoomID, roomViewModel.VideoFiles, MediaType.Video);
                }

                return RedirectToAction(nameof(Index), new { hotelId = roomViewModel.HotelID });
            }
            return View(roomViewModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
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
            var room = await _context.Rooms
                .Include(r => r.RoomMediaDetails)
                .ThenInclude(rmd => rmd.Media)
                .FirstOrDefaultAsync(r => r.RoomID == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomViewModel = new RoomViewModel
            {
                RoomID = room.RoomID,
                HotelID = room.HotelID,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                PricePerNight = room.PricePerNight,
                Description = room.Description,
                Availability = room.Availability,
                DateCreatedAt = room.DateCreatedAt,
                DateUpdatedAt = room.DateUpdatedAt,
                RoomMediaDetails = await _context.RoomMediaDetails
                    .Where(m => m.RoomId == id)
                    .Include(m => m.Media)
                    .Select(m => new MediaViewModel(m.Media))
                    .ToListAsync()
            };

            return View(roomViewModel);
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RoomViewModel roomViewModel)
        {
            if (id != roomViewModel.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var room = await _context.Rooms
                        .Include(r => r.RoomMediaDetails)
                        .ThenInclude(rmd => rmd.Media)
                        .FirstOrDefaultAsync(r => r.RoomID == id);

                    if (room == null)
                    {
                        return NotFound();
                    }

                    room.RoomNumber = roomViewModel.RoomNumber;
                    room.Type = roomViewModel.Type;
                    room.PricePerNight = roomViewModel.PricePerNight;
                    room.Description = roomViewModel.Description;
                    room.Availability = roomViewModel.Availability;
                    room.DateUpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    if (roomViewModel.ImageFiles != null)
                    {
                        await SaveFileRoomMediaDetails(room.HotelID, room.RoomID, roomViewModel.ImageFiles, MediaType.Image);
                    }
                    if (roomViewModel.VideoFiles != null)
                    {
                        await SaveFileRoomMediaDetails(room.HotelID, room.RoomID, roomViewModel.VideoFiles, MediaType.Video);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(roomViewModel.RoomID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { hotelId = roomViewModel.HotelID });
            }
            return View(roomViewModel);
        }
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
            var room = await _context.Rooms
                .Include(r => r.RoomMediaDetails)
                .ThenInclude(rmd => rmd.Media)
                .FirstOrDefaultAsync(r => r.RoomID == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomMediaDetails)
                .FirstOrDefaultAsync(r => r.RoomID == id);

            if (room != null)
            {
                // Delete associated media and RoomMediaDetails
                foreach (var roomMediaDetail in room.RoomMediaDetails)
                {
                    var media = await _context.Media.FindAsync(roomMediaDetail.MediaId);
                    if (media != null)
                    {
                        _context.Media.Remove(media);
                    }
                    _context.RoomMediaDetails.Remove(roomMediaDetail);
                }

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }

            // Redirect to the Index page
            return RedirectToAction(nameof(Index), new { hotelId = room.HotelID });
        }

        // GET: Hotels/DeleteMedia/{mediaId}
        public async Task<IActionResult> DeleteMedia(Guid? mediaId)
        {
            if (mediaId == null)
            {
                return NotFound();
            }

            // Fetch the media item along with its associated hotel
            var media = await _context.Media
                .Include(m => m.RoomMediaDetails) // Include the hotel media details
                .ThenInclude(hmd => hmd.Room) // Include the hotel information if needed
                .FirstOrDefaultAsync(m => m.MediaID == mediaId);

            if (media == null)
            {
                return NotFound();
            }

            // Create a view model to pass to the view
            var mediaViewModel = new MediaViewModel
            {
                MediaID = media.MediaID,
                FileName = media.FileName,
                FilePath = media.FilePath,
                MediaType = media.MediaType,
                // You can add more properties if needed
            };

            return View(mediaViewModel);
        }

        // POST: Hotels/DeleteMedia/{mediaId}
        [HttpPost, ActionName("DeleteMedia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMediaConfirmed(Guid mediaId)
        {
            // Find the media item by its ID
            var media = await _context.Media.FindAsync(mediaId);
            if (media == null)
            {
                return NotFound();
            }

            // Delete the file from the file system if it exists
            if (System.IO.File.Exists(media.FilePath))
            {
                System.IO.File.Delete(media.FilePath);
            }

            // Remove the media entity from the database
            _context.Media.Remove(media);

            // Remove associated HotelMediaDetails if necessary
            var roomMediaDetails = await _context.RoomMediaDetails
                .Where(hmd => hmd.MediaId == mediaId)
                .ToListAsync();

            _context.RoomMediaDetails.RemoveRange(roomMediaDetails);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = roomMediaDetails.FirstOrDefault()?.RoomId });
        }
        private bool RoomExists(Guid id)
        {
            return _context.Rooms.Any(e => e.RoomID == id);
        }

        private async Task SaveFileRoomMediaDetails(Guid hotelId, Guid roomId, List<IFormFile> files, MediaType mediaType)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var folder = mediaType == MediaType.Image ? "img" : "video";
                        var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "media", "room", folder);

                        // Log directory creation
                        _logger.LogInformation("Attempting to create directory at: {DirectoryPath}", directoryPath);

                        // Ensure the directory exists
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                            _logger.LogInformation("Directory created at: {DirectoryPath}", directoryPath);
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(directoryPath, uniqueFileName);

                        try
                        {
                            _logger.LogInformation("Attempting to save file to: {FilePath}", filePath);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            _logger.LogInformation("File {FileName} saved successfully to {FilePath}", fileName, filePath);

                            var media = new Media
                            {
                                MediaID = Guid.NewGuid(),
                                FileName = uniqueFileName,
                                FilePath = filePath,
                                MediaType = mediaType,
                            };

                            _context.Media.Add(media);

                            var roomMedia = new RoomMediaDetail
                            {
                                RoomId = roomId,
                                MediaId = media.MediaID
                            };
                            _context.RoomMediaDetails.Add(roomMedia);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error saving file {FileName}: {ExceptionMessage}", fileName, ex.Message);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("File {FileName} has zero length and will not be saved.", file.FileName);
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No files to save for room ID: {RoomID}", roomId);
            }
        }

    }
}
