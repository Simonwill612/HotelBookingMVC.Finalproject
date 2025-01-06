using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PromotionsController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager;

        public PromotionsController(UserManager<HotelUser> userManager, HotelBookingDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Promotions
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các promotion và kèm theo thông tin khách sạn
            var promotions = await _context.Promotions
                .Include(p => p.Hotel)
                .ToListAsync();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var profilePicture = string.IsNullOrEmpty(user?.ProfilePictureFileName)
                    ? "default.png" // Sử dụng ảnh mặc định nếu không có
                    : user.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }
            else
            {
                // Nếu chưa đăng nhập, sử dụng ảnh mặc định
                ViewData["UserProfilePicture"] = "/uploads/profile_pictures/default.png";
            }

            return View(promotions);
        }

        // GET: Promotions/Create
        public async Task<IActionResult> Create()
        {
            var hotels = await _context.Hotels.ToListAsync();
            var hotelList = hotels.Select(h => new { h.HotelID, h.Name }).ToList();
            // Thêm tùy chọn "All"
            var allOption = new { HotelID = Guid.Empty, Name = "All" }; // Sử dụng Guid.Empty cho tùy chọn "All"
            hotelList.Insert(0, allOption); // Đưa "All" lên đầu danh sách
            ViewBag.Hotels = new SelectList(hotelList, "HotelID", "Name");
            return View(new PromotionViewModel());
        }
        [HttpPost]
        public async Task<JsonResult> Create(PromotionViewModel promotionViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var profilePicture = string.IsNullOrEmpty(user.ProfilePictureFileName)
                    ? "default.png" // Hình ảnh mặc định
                    : user.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }
            if (ModelState.IsValid)
            {
                if (promotionViewModel.HotelIDs.Contains(Guid.Empty))
                {
                    // Create a promotion for all hotels by setting HotelID to null
                    var promotion = new Promotion
                    {
                        Code = promotionViewModel.Code,
                        DiscountAmount = promotionViewModel.DiscountAmount,
                        IsActive = promotionViewModel.IsActive,
                        ExpirationDate = promotionViewModel.ExpirationDate,
                        QuantityLimit = promotionViewModel.QuantityLimit,
                        HotelID = null // Null to indicate it applies to all hotels
                    };

                    _context.Promotions.Add(promotion);
                }
                else
                {
                    // Create separate promotions for each selected hotel
                    foreach (var hotelId in promotionViewModel.HotelIDs)
                    {
                        var promotion = new Promotion
                        {
                            Code = promotionViewModel.Code,
                            DiscountAmount = promotionViewModel.DiscountAmount,
                            IsActive = promotionViewModel.IsActive,
                            ExpirationDate = promotionViewModel.ExpirationDate,
                            QuantityLimit = promotionViewModel.QuantityLimit,
                            HotelID = hotelId // Assign the specific hotel ID
                        };

                        _context.Promotions.Add(promotion);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors)
                                   .Select(x => x.ErrorMessage);
            return Json(new { success = false, message = "Model state is invalid.", errors });
        }
        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var profilePicture = string.IsNullOrEmpty(user.ProfilePictureFileName)
                    ? "default.png" // Hình ảnh mặc định
                    : user.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }
            var promotionViewModel = new PromotionViewModel
            {
                PromotionID = promotion.PromotionID,
                Code = promotion.Code,
                DiscountAmount = promotion.DiscountAmount,
                IsActive = promotion.IsActive,
                ExpirationDate = promotion.ExpirationDate,
                QuantityLimit = promotion.QuantityLimit,
                HotelIDs = promotion.HotelID != null ? new List<Guid> { promotion.HotelID.Value } : new List<Guid>() // Chỉ lấy giá trị không nullable
            };
            var hotels = await _context.Hotels.ToListAsync();
            var hotelList = hotels.Select(h => new { h.HotelID, h.Name }).ToList();
            // Thêm tùy chọn "All"
            var allOption = new { HotelID = Guid.Empty, Name = "All" }; // Sử dụng Guid.Empty cho tùy chọn "All"
            hotelList.Insert(0, allOption); // Đưa "All" lên đầu danh sách
            ViewBag.Hotels = new SelectList(hotelList, "HotelID", "Name");
            return View(promotionViewModel);
        }
        // POST: Promotions/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit(PromotionViewModel promotionViewModel)
        {
            if (ModelState.IsValid)
            {
                var promotion = await _context.Promotions.FindAsync(promotionViewModel.PromotionID);
                if (promotion == null)
                {
                    return Json(new { success = false, message = "Promotion not found." });
                }
                // Map thuộc tính từ ViewModel sang thực thể
                promotion.Code = promotionViewModel.Code;
                promotion.DiscountAmount = promotionViewModel.DiscountAmount;
                promotion.IsActive = promotionViewModel.IsActive;
                promotion.ExpirationDate = promotionViewModel.ExpirationDate;
                promotion.QuantityLimit = promotionViewModel.QuantityLimit;
                // Chỉ gán HotelID nếu có giá trị
                if (promotionViewModel.HotelIDs.Count > 0 && promotionViewModel.HotelIDs[0] != Guid.Empty)
                {
                    promotion.HotelID = promotionViewModel.HotelIDs[0]; // Lấy HotelID đầu tiên
                }
                else
                {
                    promotion.HotelID = null; // Hoặc xử lý theo cách khác nếu không có khách sạn nào được chọn
                }
                _context.Update(promotion);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Model state is invalid." });
        }
        // POST: Promotions/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(Guid id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Promotion not found." });
        }
        // POST: Validate Promotion
        [HttpPost]
        public async Task<JsonResult> ValidatePromotion(string code, Guid hotelId)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code);

            if (promotion != null)
            {
                // Check if the promotion is active and not expired
                if (promotion.IsActive && promotion.ExpirationDate >= DateTime.Now)
                {
                    if (promotion.HotelID == null || promotion.HotelID == hotelId)
                    {
                        // Null HotelID means promotion applies to all hotels
                        return Json(new { success = true, discountAmount = promotion.DiscountAmount });
                    }
                    else
                    {
                        return Json(new { success = false, message = "This promotion code is not valid for the selected hotel." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "This promotion code is inactive or has expired." });
                }
            }

            return Json(new { success = false, message = "Invalid promotion code." });
        }
    }
}