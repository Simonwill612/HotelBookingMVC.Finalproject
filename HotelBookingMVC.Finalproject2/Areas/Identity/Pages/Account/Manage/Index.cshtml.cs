using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using HotelBookingMVC.Finalproject2.Models;

namespace HotelBookingMVC.Finalproject2.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly SignInManager<HotelUser> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public IndexModel(
            UserManager<HotelUser> userManager,
            SignInManager<HotelUser> signInManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }

            public IFormFile ProfilePicture { get; set; }
            public string ProfilePictureFileName { get; set; }
        }

        private async Task LoadAsync(HotelUser user)
        {
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Address2 = user.Address2,
                Country = user.Country,
                State = user.State,
                Zip = user.Zip,
                ProfilePictureFileName = user.ProfilePictureFileName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Address = Input.Address;
            user.Address2 = Input.Address2;
            user.Country = Input.Country;
            user.State = Input.State;
            user.Zip = Input.Zip;

            if (Input.ProfilePicture != null)
            {
                // Define the file path and save the file
                var fileName = Path.GetFileName(Input.ProfilePicture.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureFileName = fileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
