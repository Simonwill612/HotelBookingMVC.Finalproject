using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    public class BaseController : Controller

    {
        private readonly HotelBookingDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public BaseController(HotelBookingDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        protected async Task<Media?> SaveMedia(IFormFile? file, MediaType mediaType, Guid hotelId, Guid? roomId)
        {
            if (file == null || file.Length <= 0) return null;

            var fileGuidName = Guid.NewGuid().ToString();
            var fileName = "";
            var fileExtension = "";
            var fileNameString = file.FileName;
            if (string.IsNullOrEmpty(fileNameString)) return null;

            try
            {
                string[] arrayExtension = fileNameString.Split('.');
                var fullFileName = "";
                if (arrayExtension.Length > 0)
                {
                    fileExtension = arrayExtension.Last();
                    if (!AppConstant.VALID_FILE_EXTENSIONS.Contains(fileExtension)) return null;

                    fileName = string.Join('.', arrayExtension.Take(arrayExtension.Length - 1));
                    fullFileName = fileGuidName + "." + fileExtension;
                }

                var physicalPath = Path.Combine(_environment.WebRootPath, "uploads", fullFileName);
                if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new Media
                {
                    MediaID = Guid.NewGuid(),
                    FileName = fullFileName,
                    FilePath = physicalPath,
                    MediaType = mediaType,
                    //HotelID = hotelId,
                    //RoomID = roomId
                };
            }
            catch
            {
                // Handle exception (optional logging)
                return null;
            }
        }
    }
}