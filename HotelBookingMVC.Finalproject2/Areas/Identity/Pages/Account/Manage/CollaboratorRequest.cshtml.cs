using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace _1640WebDevUMC.Areas.Identity.Pages.Account.Manage
{
    public class CollaboratorRequestModel : PageModel
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public CollaboratorRequestModel(UserManager<HotelUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; }

        [BindProperty]
        public string? FirstName { get; set; }

        [BindProperty]
        public string? LastName { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        [BindProperty]
        public string? Address2 { get; set; }

        [BindProperty]
        public string? Country { get; set; }

        [BindProperty]
        public string? State { get; set; }

        [BindProperty]
        public string? Zip { get; set; }

        [BindProperty]
        public string? ProfilePictureFileName { get; set; }

        [BindProperty]
        public DateTime? DateCreatedAt { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            FullName = $"{user.FirstName} {user.LastName}";
            Email = user.Email;
            Phone = user.PhoneNumber;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Address = user.Address;
            Address2 = user.Address2;
            Country = user.Country;
            State = user.State;
            Zip = user.Zip;
            ProfilePictureFileName = user.ProfilePictureFileName;
            DateCreatedAt = user.DateCreatedAt;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get a list of admin users
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminEmails = adminUsers.Select(u => u.Email).ToList();

            // Email content
            string subject = "New Collaborator Request";
            string body = $@"
                <p>User: {FullName}</p>
                <p>Email: {Email}</p>
                <p>Phone Number: {Phone}</p>
                <p>Reason: {Reason}</p>
                <p>Address: {Address}</p>";

            // Send email to all admins
            foreach (var adminEmail in adminEmails)
            {
                await _emailSender.SendEmailAsync(adminEmail, subject, body);
            }

            TempData["SuccessMessage"] = "Your request has been successfully submitted!";
            return RedirectToPage("./Index");
        }
    }
}
