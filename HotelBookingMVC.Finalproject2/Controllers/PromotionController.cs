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
using Microsoft.AspNetCore.Authorization;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Admin")]

    public class PromotionsController : Controller
    {
        private readonly HotelBookingDbContext _context;

        public PromotionsController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET: Promotions
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions.Include(p => p.Hotel).ToListAsync();
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
            if (ModelState.IsValid)
            {
                if (promotionViewModel.HotelIDs.Contains(Guid.Empty))
                {
                    // Nếu "All" được chọn, thêm khuyến mãi cho tất cả khách sạn
                    var allHotels = await _context.Hotels.Select(h => h.HotelID).ToListAsync();
                    foreach (var hotelId in allHotels)
                    {
                        var promotion = new Promotion
                        {
                            Code = promotionViewModel.Code,
                            DiscountAmount = promotionViewModel.DiscountAmount,
                            IsActive = promotionViewModel.IsActive,
                            ExpirationDate = promotionViewModel.ExpirationDate,
                            QuantityLimit = promotionViewModel.QuantityLimit,
                            HotelID = hotelId // Thêm khuyến mãi cho từng khách sạn
                        };

                        _context.Promotions.Add(promotion);
                    }
                }
                else
                {
                    foreach (var hotelId in promotionViewModel.HotelIDs)
                    {
                        var promotion = new Promotion
                        {
                            Code = promotionViewModel.Code,
                            DiscountAmount = promotionViewModel.DiscountAmount,
                            IsActive = promotionViewModel.IsActive,
                            ExpirationDate = promotionViewModel.ExpirationDate,
                            QuantityLimit = promotionViewModel.QuantityLimit,
                            HotelID = hotelId // Gán HotelID cho khuyến mãi
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
            // Lấy khuyến mãi theo mã
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code);

            // Kiểm tra xem khuyến mãi có tồn tại và hợp lệ không
            if (promotion != null)
            {
                // Kiểm tra xem khuyến mãi có đang hoạt động và chưa hết hạn không
                if (promotion.IsActive && promotion.ExpirationDate >= DateTime.Now)
                {
                    // Nếu HotelID là null, nó hợp lệ cho tất cả các khách sạn
                    if (promotion.HotelID == null || promotion.HotelID == hotelId)
                    {
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
