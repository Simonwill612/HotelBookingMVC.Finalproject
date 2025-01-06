// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HotelBookingMVC.Finalproject2.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<HotelUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<HotelUser> _userManager; // Thêm UserManager vào đây

        public LogoutModel(SignInManager<HotelUser> signInManager, ILogger<LogoutModel> logger, UserManager<HotelUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager; // Khởi tạo UserManager
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Xóa giỏ hàng khỏi session bằng khóa "Cart"
            HttpContext.Session.Remove("Cart");

            // Đăng xuất người dùng
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Redirect về trang chính hoặc trang trước đó
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/Index", new { area = "" });  // Redirect về trang chủ
            }
        }
    }
}
