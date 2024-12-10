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

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Manager")]

    public class RoomsController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HotelsController> _logger;


        public RoomsController(HotelBookingDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<HotelsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Guid hotelId)
        {
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
                    Availability = r.Availability,
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

            ViewBag.HotelName = await _context.Hotels
                .Where(h => h.HotelID == hotelId)
                .Select(h => h.Name)
                .FirstOrDefaultAsync();
            ViewBag.HotelID = hotelId;

            return View(roomViewModels);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
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
                MediaViewModels = room.RoomMediaDetails.Select(rmd => new MediaViewModel
                {
                    MediaID = rmd.MediaId,
                    FileName = rmd.Media.FileName,
                    FilePath = rmd.Media.FilePath,
                    MediaType = rmd.Media.MediaType
                }).ToList()
            };

            return View(roomViewModel);
        }

        public IActionResult Create(Guid hotelId)
        {
            var viewModel = new RoomViewModel
            {
                HotelID = hotelId
            };

            ViewBag.HotelID = hotelId;

            return View(viewModel);
        }

        [HttpPost]
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
                MediaViewModels = room.RoomMediaDetails.Select(rmd => new MediaViewModel
                {
                    MediaID = rmd.MediaId,
                    FileName = rmd.Media.FileName,
                    FilePath = rmd.Media.FilePath,
                    MediaType = rmd.Media.MediaType
                }).ToList()
            };

            return View(roomViewModel);
        }

        [HttpPost]
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
                return RedirectToAction(nameof(Index), new { hotelId = roomViewModel.HotelID });
            }
            return View(roomViewModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { hotelId = room.HotelID });
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
                _logger.LogWarning("No files to save for hotel ID: {RoomID}", roomId);
            }
        }

    }
}
