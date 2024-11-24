using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly SignInManager<HotelUser> _signInManager;

        public IndexModel(UserManager<HotelUser> userManager, SignInManager<HotelUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
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

            [Required(ErrorMessage = "The ProfilePicture field is required.")]
            [DataType(DataType.Upload)]
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
                return NotFound("User not found.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Handle Profile Picture upload
            if (Input.ProfilePicture != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile_pictures", Input.ProfilePicture.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(stream);
                }
                user.ProfilePictureFileName = Input.ProfilePicture.FileName;
            }

            // Update the other profile details
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Address = Input.Address;
            user.Address2 = Input.Address2;
            user.Country = Input.Country;
            user.State = Input.State;
            user.Zip = Input.Zip;

            // Save updated user details
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                StatusMessage = "Your profile has been updated successfully.";
            }
            else
            {
                StatusMessage = "There was an error updating your profile.";
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToPage();
        }
    }
}
