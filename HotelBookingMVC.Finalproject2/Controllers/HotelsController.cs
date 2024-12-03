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
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Models;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    //[Authorize]
    public class HotelsController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<HotelUser> _userManager;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(HotelBookingDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<HotelUser> userManager, ILogger<HotelsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logger = logger;

        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
            //var hotels = await _context.Hotels.Include(h => h.Media).ToListAsync();
            var hotels = await _context.Hotels
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
                    .Select(m => new MediaViewModel(m.Media))
                    .ToList()
            }).ToList();

            return View(hotelViewModels);
        }

        // GET: Hotels/Details/{id}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.HotelMediaDetails)
                .ThenInclude(hmd => hmd.Media) // Ensure Media is included
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
                    .Select(m => m.Media != null ? new MediaViewModel(m.Media) : null) // Check for null Media
                    .ToList()
            };

            return View(hotelViewModel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel hotelViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User); // Get the current user's ID

                var existingHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.UserID == userId);
                if (existingHotel != null)
                {
                    ModelState.AddModelError(string.Empty, "You have already created a hotel.");
                    return View(hotelViewModel);
                }

                var hotel = new Hotel
                {
                    HotelID = Guid.NewGuid(),
                    Name = hotelViewModel.Name,
                    Address = hotelViewModel.Address,
                    City = hotelViewModel.City,
                    State = hotelViewModel.State,
                    ZipCode = hotelViewModel.ZipCode,
                    PhoneNumber = hotelViewModel.PhoneNumber,
                    Email = hotelViewModel.Email,
                    Description = hotelViewModel.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserID = userId // Set the UserID
                };

                _context.Add(hotel);
                await _context.SaveChangesAsync();

                await SaveMediaFiles(hotel.HotelID, hotelViewModel.ImageFiles, MediaType.Image, null); // Pass null for roomId
                await SaveMediaFiles(hotel.HotelID, hotelViewModel.VideoFiles, MediaType.Video, null); // Pass null for roomIdz

                return RedirectToAction(nameof(Index));
            }
            return View(hotelViewModel);
        }

        // GET: Hotels/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
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
                //Media = await _context.Media.Where(m => m.HotelID == id).ToListAsync()
                Media = await _context.HotelMediaDetails
                    .Where(m => m.HotelId == id)
                    .Include(m => m.Media)
                    .Select(m => new MediaViewModel(m.Media))
                    .ToListAsync()
            };

            return View(hotelViewModel);
        }

        // POST: Hotels/Edit/{id}
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, HotelViewModel hotelViewModel)
        {
            if (id != hotelViewModel.HotelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var hotel = await _context.Hotels.Include(h => h.Media).FirstOrDefaultAsync(h => h.HotelID == hotelViewModel.HotelID);
                    var hotel = await _context.Hotels
                        .Include(h => h.HotelMediaDetails)
                        .Where(h => h.HotelID == hotelViewModel.HotelID)
                        .FirstOrDefaultAsync();

                    if (hotel == null)
                    {
                        return NotFound();
                    }

                    hotel.Name = hotelViewModel.Name;
                    hotel.Address = hotelViewModel.Address;
                    hotel.City = hotelViewModel.City;
                    hotel.State = hotelViewModel.State;
                    hotel.ZipCode = hotelViewModel.ZipCode;
                    hotel.PhoneNumber = hotelViewModel.PhoneNumber;
                    hotel.Email = hotelViewModel.Email;
                    hotel.Description = hotelViewModel.Description;
                    hotel.UpdatedAt = DateTime.Now;

                    // Add new media
                    await SaveMediaFiles(hotel.HotelID, hotelViewModel.ImageFiles, MediaType.Image, null);
                    await SaveMediaFiles(hotel.HotelID, hotelViewModel.VideoFiles, MediaType.Video, null);

                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotelViewModel.HotelID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotelViewModel);
        }

        // GET: Hotels/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelID == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            // Fetch associated media details
            var mediaToDelete = await _context.HotelMediaDetails
                .Where(h => h.HotelId == id)
                .Include(h => h.Media)
                .ToListAsync();

            foreach (var item in mediaToDelete)
            {
                var media = item.Media;
                if (media != null) // Ensure media is not null
                {
                    if (System.IO.File.Exists(media.FilePath))
                    {
                        System.IO.File.Delete(media.FilePath);
                    }
                    _context.Media.Remove(media);
                }
                else
                {
                    // Log or handle cases where media is null
                    _logger.LogWarning($"Media for HotelID {id} is null.");
                }
            }

            // Remove hotel entity
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
                .Include(m => m.HotelMediaDetails) // Include the hotel media details
                .ThenInclude(hmd => hmd.Hotel) // Include the hotel information if needed
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
            var hotelMediaDetails = await _context.HotelMediaDetails
                .Where(hmd => hmd.MediaId == mediaId)
                .ToListAsync();

            _context.HotelMediaDetails.RemoveRange(hotelMediaDetails);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the index or another appropriate action
        }

        private bool HotelExists(Guid id)
        {
            return _context.Hotels.Any(e => e.HotelID == id);
        }

        private async Task SaveMediaFiles(Guid hotelId, IList<IFormFile> files, MediaType mediaType, Guid? roomId)
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
                        var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "media", "hotel", folder);

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
                                //HotelID = hotelId,
                                //RoomID = roomId
                            };

                            _context.Media.Add(media);

                            var hotelMedia = new HotelMediaDetail
                            {
                                HotelId = hotelId,
                                MediaId = media.MediaID
                            };
                            _context.HotelMediaDetails.Add(hotelMedia);
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
                _logger.LogWarning("No files to save for hotel ID: {HotelID}", hotelId);
            }
        }
    }
}
