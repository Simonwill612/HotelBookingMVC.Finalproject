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
                    Price = i.Price,
                    Quantity = i.Quantity,
                    CheckInDate = i.CheckInDate,
                    CheckOutDate = i.CheckOutDate
                }).ToList(),
                Subtotal = cart.CartItems.Sum(i => i.Price),
                Tax = CalculateTax(cart.CartItems.Sum(item => item.Price)),
                Total = cart.CartItems.Sum(i => i.Price) + CalculateTax(cart.CartItems.Sum(item => item.Price))
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
            var totalPrice = price * nights;

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

            return Json(new { success = true, message = "Room added to cart." });
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
            var cart = GetCart();
            var item = cart?.CartItems.FirstOrDefault(i => i.CartItemID == cartItemId);

            if (item == null)
                return Json(new { success = false, message = "Item not found." });

            cart.CartItems.Remove(item);
            UpdateCart(cart);

            return Json(new { success = true, message = "Item removed from cart." });
        }

        // Tạo hóa đơn
        public async Task<IActionResult> ShowBill()
        {
            var cart = GetCart();

            // If the cart is empty, redirect to home
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Fetch the logged-in user details
            var userId = _userManager.GetUserId(User); // Get the user's ID
            var user = await _userManager.FindByIdAsync(userId); // Get the user from UserManager

            // Create the OrderViewModel and populate it with user details and cart items
            var orderViewModel = new OrderViewModel
            {
                CartItems = cart.CartItems.Select(item => new CartItemViewModel
                {
                    CartItemID = item.CartItemID,
                    RoomID = item.RoomID,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    CheckInDate = item.CheckInDate,
                    CheckOutDate = item.CheckOutDate
                }).ToList(),
                SubTotal = cart.CartItems.Sum(item => item.Price),
                Tax = CalculateTax(cart.CartItems.Sum(item => item.Price)),
                Total = cart.CartItems.Sum(item => item.Price) + CalculateTax(cart.CartItems.Sum(item => item.Price)),
                FirstName = user?.FirstName ?? string.Empty, // Assuming you have these properties in your user entity
                LastName = user?.LastName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Address = user?.Address ?? string.Empty,
                Address2 = user?.Address2 ?? string.Empty,
                Country = user?.Country ?? string.Empty,
                State = user?.State ?? string.Empty,
                Zip = user?.Zip ?? string.Empty,
                IsShippingSameAsBilling = false
            };

            return View(orderViewModel);
        }

        [HttpPost]
        [Route("Cart/ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(OrderViewModel billingInfo, PaymentViewModel paymentInfo) // Accept updated billing info from the form
        {
            var cart = GetCart();
            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty." });
            }

            // Simulate payment processing logic (replace with actual payment gateway if needed)
            bool paymentSuccess = SimulatePayment();

            if (!paymentSuccess)
            {
                return Json(new { success = false, message = "Payment failed. Please try again." });
            }

            var userId = _userManager.GetUserId(User); // Get the logged-in user's ID
            var user = await _userManager.FindByIdAsync(userId); // Get the user from UserManager

            try
            {
                // Update user's billing information if modified
                user.FirstName = billingInfo.FirstName;
                user.LastName = billingInfo.LastName;
                user.Email = billingInfo.Email;
                user.Address = billingInfo.Address;
                user.Address2 = billingInfo.Address2;
                user.Country = billingInfo.Country;
                user.State = billingInfo.State;
                user.Zip = billingInfo.Zip;

                // Create a new Order for the cart
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    SubTotal = cart.CartItems.Sum(item => item.Price),
                    Tax = CalculateTax(cart.CartItems.Sum(item => item.Price)), // Placeholder tax calculation
                    Total = cart.CartItems.Sum(item => item.Price) + CalculateTax(cart.CartItems.Sum(item => item.Price)),
                    Phone = user.PhoneNumber, // Correct way to get PhoneNumber from user object
                    Address = user.Address, // Use the updated address from the form
                    BillID = Guid.NewGuid() // Placeholder, use actual bill ID
                };

                // Save the Order
                _context.Orders.Add(order);
                // Update user in the database using UserManager
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user information." });
                }

                // Create a Bill for the Order, using updated billing info
                var bill = new Bill
                {
                    BillID = order.BillID,
                    UserID = userId,
                    FirstName = billingInfo.FirstName,
                    LastName = billingInfo.LastName,
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

                // Loop through the cart items and create bookings and payments
                foreach (var cartItem in cart.CartItems)
                {
                    // Get the room from the database using RoomID to find the RoomNumber
                    var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomID == cartItem.RoomID);
                    if (room == null)
                    {
                        return Json(new { success = false, message = $"Room with ID {cartItem.RoomID} not found." });
                    }

                    // Create and save the booking
                    var booking = new Booking
                    {
                        BookingID = Guid.NewGuid(),
                        UserID = userId,
                        RoomID = cartItem.RoomID,
                        CheckInDate = cartItem.CheckInDate,
                        CheckOutDate = cartItem.CheckOutDate,
                        TotalPrice = cartItem.Price + CalculateTax(cartItem.Price),
                        Status = "Confirmed",
                        BookingDate = DateTime.Now
                    };

                    _context.Bookings.Add(booking);

                    // Create and save the payment
                    var payment = new Payment
                    {
                        PaymentID = Guid.NewGuid(),
                        BookingID = booking.BookingID,
                        Amount = booking.TotalPrice,
                        PaymentMethod = "Credit Card", // Assuming credit card here
                        PaymentStatus = "Completed",
                        PaymentDate = DateTime.Now
                    };

                    _context.Payments.Add(payment);
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                // Validate card details format using regex
                var cardNumberValid = Regex.IsMatch(paymentInfo.CardNumber, @"^\d{16}$");
                var cvvValid = Regex.IsMatch(paymentInfo.CVV, @"^\d{3}$");
                var expirationDateValid = Regex.IsMatch(paymentInfo.ExpirationDate, @"^(0[1-9]|1[0-2])\/?([0-9]{2})$");
                var cardNameValid = !string.IsNullOrEmpty(paymentInfo.CardName);

                if (cardNumberValid && cvvValid && expirationDateValid && cardNameValid)
                {
                    // Send a confirmation email to the user
                    await SendConfirmationEmail(cart);

                    // Clear the cart after a successful payment
                    ClearCart();

                    // Return success message
                    return Json(new { success = true, message = "Payment successful and booking confirmed." });
                }
                else
                {
                    // Invalid card details
                    return Json(new { success = false, message = "Invalid card details. Please check and try again." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if needed)
                return Json(new { success = false, message = "An error occurred during the booking process. Please try again." });
            }
        }


        // Simulate payment (replace with actual payment logic)
        private bool SimulatePayment()
        {
            return true; // Always return success for simulation purposes
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


        private CartViewModel GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? null : JsonConvert.DeserializeObject<CartViewModel>(cartJson);
        }

        private void UpdateCart(CartViewModel cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
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
