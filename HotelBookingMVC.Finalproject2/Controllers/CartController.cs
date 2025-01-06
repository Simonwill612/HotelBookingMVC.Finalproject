using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using HotelBookingMVC.Finalproject2.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CartController(HotelBookingDbContext context, UserManager<HotelUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // Hiển thị chi tiết giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart() ?? new CartViewModel { CartItems = new List<CartItemViewModel>() };
            var cartViewModel = MapCartToViewModel(cart);
            return View(cartViewModel);
        }

        private CartViewModel MapCartToViewModel(CartViewModel cart)
        {
            return new CartViewModel
            {
                CartItems = cart.CartItems.Select(i => new CartItemViewModel
                {
                    CartItemID = i.CartItemID,
                    RoomID = i.RoomID,
                    RoomNumber = _context.Rooms.FirstOrDefault(r => r.RoomID == i.RoomID)?.RoomNumber ?? "Unknown", // Get RoomNumber
                    Price = i.Price,
                    Quantity = i.Quantity,
                    CheckInDate = i.CheckInDate,
                    CheckOutDate = i.CheckOutDate,
                    HotelID = _context.Rooms.FirstOrDefault(r => r.RoomID == i.RoomID)?.HotelID ?? Guid.Empty // Get HotelID based on RoomID
                }).ToList(),
                Subtotal = cart.CartItems.Sum(i => i.Price * i.Quantity),
                Tax = CalculateTax(cart.CartItems.Sum(item => item.Price * item.Quantity)),
                Total = cart.CartItems.Sum(i => i.Price * i.Quantity) + CalculateTax(cart.CartItems.Sum(item => item.Price * item.Quantity))
            };
        }




        [HttpPost]
        public IActionResult AddToCart(Guid productId, decimal price, DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate == DateTime.MinValue || checkOutDate == DateTime.MinValue || checkInDate >= checkOutDate)
            {
                return Json(new { success = false, message = "Please select valid check-in and check-out dates." });
            }

            var nights = (checkOutDate - checkInDate).Days;
            var totalPrice = price;

            // Kiểm tra tình trạng phòng đã được đặt
            var bookedDates = _context.Bookings
                .Where(b => b.RoomID == productId && b.Status == "Confirmed" &&
                            b.CheckInDate < checkOutDate && b.CheckOutDate > checkInDate)
                .ToList();

            // Kiểm tra khả năng có phòng
            bool isAvailable = bookedDates.All(booking =>
                !Enumerable.Range(0, (booking.CheckOutDate - booking.CheckInDate).Days).Any(dateOffset =>
                    booking.CheckInDate.Date.AddDays(dateOffset) >= checkInDate.Date &&
                    booking.CheckInDate.Date.AddDays(dateOffset) < checkOutDate.Date));

            if (!isAvailable)
            {
                return Json(new { success = false, message = "Room is not available for the selected dates." });
            }

            var cart = GetCart() ?? new CartViewModel();

            var existingItem = cart.CartItems?.FirstOrDefault(i => i.RoomID == productId && i.CheckInDate == checkInDate && i.CheckOutDate == checkOutDate);

            if (existingItem != null)
            {
                return Json(new { success = false, message = "Room for the selected dates is already in the cart." });
            }

            var newCartItem = new CartItemViewModel
            {
                CartItemID = Guid.NewGuid(),
                RoomID = productId,
                Price = totalPrice,
                Quantity = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate
            };

            cart.CartItems ??= new List<CartItemViewModel>();
            cart.CartItems.Add(newCartItem);
            UpdateCart(cart);

            // Return the redirect URL after success
            return Json(new { success = true, redirectUrl = Url.Action("ShowBill", "Cart") });
        }


        [HttpPost]
        public IActionResult UpdateCart(Guid cartItemId, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart?.CartItems.FirstOrDefault(i => i.CartItemID == cartItemId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            if (quantity > 0)
            {
                cartItem.Quantity = quantity;
                UpdateCart(cart);
                return Json(new { success = true, message = "Cart updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Invalid quantity." });
            }
        }
        [HttpPost]
        public IActionResult RemoveFromCart(Guid cartItemId)
        {
            try
            {
                // Lấy giỏ hàng từ session
                var cart = GetCart();
                if (cart == null || cart.CartItems == null)
                {
                    return Json(new { success = false, message = "Your cart is empty or cannot be found." });
                }

                // Tìm mục cần xóa trong giỏ hàng
                var item = cart.CartItems.FirstOrDefault(i => i.CartItemID == cartItemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item not found in your cart." });
                }

                // Xóa mục khỏi giỏ hàng
                cart.CartItems.Remove(item);

                // Cập nhật lại giỏ hàng trong session
                UpdateCart(cart);

                return Json(new { success = true, message = "Item has been removed from your cart." });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> ShowBill(string discountCode)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var profilePicture = string.IsNullOrEmpty(currentUser.ProfilePictureFileName)
                    ? "default.png"
                    : currentUser.ProfilePictureFileName;

                ViewData["UserProfilePicture"] = $"/uploads/profile_pictures/{profilePicture}";
            }
            // Lấy giỏ hàng hiện tại
            var cart = GetCart();

            // Chuyển hướng đến trang chủ nếu giỏ hàng trống
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin người dùng đã đăng nhập
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại. Vui lòng đăng nhập.";
                return Redirect("~/Identity/Account/Login");
            }

            // Fetch room details
            var rooms = await _context.Rooms
                .Include(r => r.RoomMediaDetails)
                    .ThenInclude(rmd => rmd.Media)
                .ToListAsync();

            // Chuyển đổi các mục trong giỏ hàng sang view model
            var cartItems = cart.CartItems.Select(item =>
            {
                var room = rooms.FirstOrDefault(r => r.RoomID == item.RoomID);
                var mediaUrl = room?.RoomMediaDetails
                    .Where(rmd => rmd.Media != null && !string.IsNullOrEmpty(rmd.Media.FileName))
                    .Select(rmd => rmd.Media.FileName)
                    .FirstOrDefault();

                return new CartItemViewModel
                {
                    CartItemID = item.CartItemID,
                    RoomID = item.RoomID,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    CheckInDate = item.CheckInDate,
                    CheckOutDate = item.CheckOutDate,
                    RoomNumber = room?.RoomNumber ?? "Unknown",
                    FilePath = mediaUrl
                };
            }).ToList();

            // Tính toán tổng
            var subTotal = cartItems.Sum(item => item.Price * item.Quantity);
            var discount = await CalculateDiscount(discountCode, subTotal); // Sử dụng await
            var tax = subTotal * 0.1m; // Tính thuế 10% trên SubTotal
            var total = subTotal - discount + tax; // Tổng = SubTotal - Discount + Tax

            // Tạo OrderViewModel
            var orderViewModel = new OrderViewModel
            {
                CartItems = cartItems,
                SubTotal = subTotal,
                Discount = discount,
                Tax = tax,
                Total = total,
                DiscountCode = discountCode, // Lưu mã giảm giá
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = user.Address ?? string.Empty,
                Address2 = user.Address2 ?? string.Empty,
                Country = user.Country ?? string.Empty,
                State = user.State ?? string.Empty,
                Zip = user.Zip ?? string.Empty,
                IsShippingSameAsBilling = false
            };

            return View(orderViewModel);
        }
        private async Task<decimal> CalculateDiscount(string discountCode, decimal subTotal)
        {
            // Nếu không có mã giảm giá, trả về 0
            if (string.IsNullOrEmpty(discountCode))
            {
                return 0;
            }

            // Tìm mã giảm giá trong cơ sở dữ liệu
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == discountCode && p.IsActive && p.ExpirationDate >= DateTime.Now);

            // Log mã giảm giá và subtotal
            Console.WriteLine($"Discount Code: {discountCode}, Subtotal: {subTotal}");

            // Nếu mã giảm giá hợp lệ, tính toán giảm giá tối đa không vượt quá subtotal
            if (promotion != null)
            {
                var discountAmount = Math.Min(promotion.DiscountAmount, subTotal);
                Console.WriteLine($"Valid Discount: {discountAmount}");
                return discountAmount; // Giảm giá không được vượt quá subtotal
            }

            // Nếu mã giảm giá không hợp lệ, trả về 0
            Console.WriteLine("Invalid Discount Code");
            return 0;
        }

        [HttpPost]
        [Route("Cart/ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(OrderViewModel billingInfo, PaymentViewModel paymentInfo)
        {
            // Lấy giỏ hàng hiện tại
            var cart = GetCart();
            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty." });
            }

            // Kiểm tra thông tin thẻ tín dụng
            var cardNumberValid = Regex.IsMatch(paymentInfo.CardNumber, @"^\d{16}$");
            var cvvValid = Regex.IsMatch(paymentInfo.CVV, @"^\d{3}$");
            var expirationDateValid = Regex.IsMatch(paymentInfo.ExpirationDate, @"^(0[1-9]|1[0-2])\/?([0-9]{2})$");
            var cardNameValid = !string.IsNullOrEmpty(paymentInfo.CardName);

            if (!cardNumberValid || !cvvValid || !expirationDateValid || !cardNameValid)
            {
                return Json(new { success = false, message = "Invalid card details. Please check and try again." });
            }

            // Lấy thông tin người dùng hiện tại
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Tạo giao dịch để đảm bảo tính toàn vẹn
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Cập nhật thông tin người dùng
                    user.FirstName = billingInfo.FirstName;
                    user.LastName = billingInfo.LastName;
                    user.PhoneNumber = billingInfo.Phone;
                    user.Email = billingInfo.Email;
                    user.Address = billingInfo.Address;
                    user.Address2 = billingInfo.Address2;
                    user.Country = billingInfo.Country;
                    user.State = billingInfo.State;
                    user.Zip = billingInfo.Zip;

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        throw new Exception("Failed to update user information.");
                    }

                    // Tạo Order
                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        DateCreated = DateTime.Now,
                        SubTotal = cart.CartItems.Sum(item => item.Price * item.Quantity),
                        Tax = CalculateTax(cart.CartItems.Sum(item => item.Price * item.Quantity)),
                        Total = cart.CartItems.Sum(item => item.Price * item.Quantity) + CalculateTax(cart.CartItems.Sum(item => item.Price * item.Quantity)),
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        BillID = Guid.NewGuid()
                    };

                    _context.Orders.Add(order);

                    // Tạo Bill
                    var bill = new Bill
                    {
                        BillID = order.BillID,
                        UserID = userId,
                        FirstName = billingInfo.FirstName,
                        LastName = billingInfo.LastName,
                        Phone = billingInfo.Phone,
                        Email = billingInfo.Email,
                        Address = billingInfo.Address,
                        Address2 = billingInfo.Address2,
                        Country = billingInfo.Country,
                        State = billingInfo.State,
                        Zip = billingInfo.Zip,
                        OrderId = order.Id,
                        IsShippingSameAsBilling = billingInfo.IsShippingSameAsBilling,
                        DateCreatedAt = DateTime.Now
                    };

                    _context.Bills.Add(bill);

                    // Tạo Booking và Payment cho mỗi CartItem
                    foreach (var cartItem in cart.CartItems)
                    {
                        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomID == cartItem.RoomID);
                        if (room == null)
                        {
                            throw new Exception($"Room with ID {cartItem.RoomID} not found.");
                        }

                        var booking = new Booking
                        {
                            BookingID = Guid.NewGuid(),
                            UserID = userId,
                            RoomID = cartItem.RoomID,
                            CheckInDate = cartItem.CheckInDate,
                            CheckOutDate = cartItem.CheckOutDate,
                            TotalPrice = cartItem.Price * cartItem.Quantity + CalculateTax(cartItem.Price * cartItem.Quantity),
                            Status = "Confirmed",
                            BookingDate = DateTime.Now
                        };

                        _context.Bookings.Add(booking);

                        var payment = new Payment
                        {
                            PaymentID = Guid.NewGuid(),
                            BookingID = booking.BookingID,
                            Amount = booking.TotalPrice,
                            PaymentMethod = "Credit Card",
                            PaymentStatus = "Completed",
                            PaymentDate = DateTime.Now
                        };

                        _context.Payments.Add(payment);
                    }

                    // Lưu toàn bộ thay đổi
                    await _context.SaveChangesAsync();

                    // Commit giao dịch
                    await transaction.CommitAsync();

                    await NotifyHotelManager(cart);

                    // Gửi email xác nhận
                    await SendConfirmationEmail(cart);

                    // Xóa giỏ hàng sau khi thanh toán thành công
                    ClearCart();

                    return Json(new { success = true, message = "Payment successful and booking confirmed." });
                }
                catch (Exception ex)
                {
                    // Hoàn tác giao dịch nếu có lỗi
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
                }
            }
        }



        private async Task NotifyHotelManager(CartViewModel cart)
        {
            // Lấy danh sách các HotelID từ giỏ hàng
            var hotelIds = cart.CartItems
                .Select(item => _context.Rooms.FirstOrDefault(r => r.RoomID == item.RoomID)?.HotelID)
                .Distinct()
                .ToList();

            foreach (var hotelId in hotelIds)
            {
                // Tìm khách sạn dựa trên HotelID
                var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelID == hotelId);
                if (hotel == null || string.IsNullOrEmpty(hotel.UserID)) continue;

                // Lấy thông tin người quản lý (manager) dựa trên UserID
                var manager = await _userManager.FindByIdAsync(hotel.UserID);
                if (manager == null || string.IsNullOrEmpty(manager.Email)) continue;

                var managerEmail = manager.Email;
                var managerName = manager.UserName; // Thay đổi nếu bạn có thuộc tính khác phù hợp hơn

                // Tạo chuỗi nội dung email
                var subject = $"[Booking Notification] New Booking for Hotel: {hotel.Name}";

                // Truy xuất thông tin phòng từ database
                var roomDetails = cart.CartItems
                    .Select(item =>
                    {
                        var room = _context.Rooms.FirstOrDefault(r => r.RoomID == item.RoomID);
                        return room != null
                            ? $"Room {room.RoomNumber}, from {item.CheckInDate.ToShortDateString()} to {item.CheckOutDate.ToShortDateString()}"
                            : "Room information not found";
                    })
                    .ToList();


                            var body = $@"
                    Dear {managerName},

                    A new booking has been made for your hotel '{hotel.Name}'.

                    Booking Details:
                    - Rooms:
                    {string.Join("\n", roomDetails)}

                    Please log in to the system for more details.

                    Best regards,
                    Hotel Booking System
                    ";

                            // Gửi email
                            await _emailSender.SendEmailAsync(managerEmail, subject, body);
                        }
                    }





        // Send confirmation email
        private async Task SendConfirmationEmail(CartViewModel cart)
        {
            var user = await _userManager.GetUserAsync(User);
            var emailContent = $"Dear {user.UserName},\n\nYour booking has been confirmed for the following rooms:\n";
            
            foreach (var item in cart.CartItems)
            {
                // Get the room from the database using RoomID to include RoomNumber in the email
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomID == item.RoomID);
                if (room != null)
                {
                    emailContent += $"- Room {room.RoomNumber}, from {item.CheckInDate} to {item.CheckOutDate}\n";
                }
                else
                {
                    emailContent += $"- Room ID {item.RoomID} not found\n";
                }
            }

            await _emailSender.SendEmailAsync(user.Email, "Booking Confirmation", emailContent);
        }


        [HttpGet]
        public IActionResult GetCartQuantity()
        {
            var cart = GetCart();
            var quantity = cart?.CartItems.Sum(item => item.Quantity) ?? 0; // Tổng số lượng sản phẩm
            return Json(new { quantity });
        }

        private CartViewModel GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            var cart = string.IsNullOrEmpty(cartJson) ? null : JsonConvert.DeserializeObject<CartViewModel>(cartJson);

            if (cart?.CartItems != null)
            {
                foreach (var item in cart.CartItems)
                {
                    if (item.Quantity == 0)
                    {
                        item.Quantity = 1;
                    }
                }
            }

            return cart;
        }

        private void UpdateCart(CartViewModel cart)
        {
            if (cart != null)
            {
                HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
            }
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }

        private decimal CalculateTax(decimal amount)
        {
            // Assuming a fixed tax rate of 10%
            return amount * 0.1m;
        }
    }
}