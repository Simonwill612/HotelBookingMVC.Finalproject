using System;
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

    public class MediaController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MediaController(HotelBookingDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid hotelId, Guid roomId, IList<IFormFile> files, MediaType mediaType)
        {
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "media", hotelId.ToString(), roomId.ToString(), mediaType.ToString(), fileName);

                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var media = new Media
                        {
                            MediaID = Guid.NewGuid(),
                            FileName = fileName,
                            FilePath = Path.Combine("/media", hotelId.ToString(), roomId.ToString(), mediaType.ToString(), fileName),
                            MediaType = mediaType,
                            //HotelID = hotelId
                        };
                        _context.Media.Add(media);


                        if (roomId != Guid.Empty)
                        {
                            var roomMediaDetail = new RoomMediaDetail
                            {
                                RoomId = roomId,
                                MediaId = media.MediaID
                            };
                            _context.RoomMediaDetails.Add(roomMediaDetail);

                        }

                        if (hotelId != Guid.Empty)
                        {
                            var hotelMediaDetail = new HotelMediaDetail
                            {
                                HotelId = hotelId,
                                MediaId = media.MediaID
                            };
                            _context.HotelMediaDetails.Add(hotelMediaDetail);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", "Rooms", new { id = roomId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid mediaId, Guid roomId)
        {
            var mediaDetail = await _context.RoomMediaDetails
                .Include(m => m.Media)
                .FirstOrDefaultAsync(m => m.MediaId == mediaId);

            if (mediaDetail == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, mediaDetail.Media.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Media.Remove(mediaDetail.Media);
            _context.RoomMediaDetails.Remove(mediaDetail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Rooms", new { id = roomId });
        }
    }
}
