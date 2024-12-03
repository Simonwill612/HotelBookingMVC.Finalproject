using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Models;
using HotelBookingMVC.Finalproject2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminDashboardController : Controller
{
    private readonly UserManager<HotelUser> _userManager;
    private readonly SignInManager<HotelUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly HotelIdentityDBContext _context;

    public AdminDashboardController(UserManager<HotelUser> userManager, SignInManager<HotelUser> signInManager, RoleManager<IdentityRole> roleManager, HotelIdentityDBContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;

        // Ensure roles exist
        EnsureRolesExistAsync().Wait();
    }

    // Method to ensure roles exist
    private async Task EnsureRolesExistAsync()
    {
        var roles = new[] { "Admin", "User", "Manager" }; // Add other roles as necessary

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    // GET: /Account/Index
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var customerData = new List<UserViewModel>();
        var managerData = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                continue;
            }

            // Check if user has 'Manager' role
            if (roles.Contains("Manager"))
            {
                managerData.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles
                });
            }
            else // Assuming the default role is customer or null
            {
                customerData.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles
                });
            }
        }

        var viewModel = new
        {
            Customers = customerData,
            Managers = managerData
        };

        return View(viewModel);
    }



    // GET: /Account/Details
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var userDetails = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PasswordHash = user.PasswordHash,
            Roles = userRoles,
        };

        return View(userDetails);
    }

    // GET: /Account/Edit
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        // Lấy danh sách tất cả các vai trò
        var allRoles = _roleManager.Roles
            .Where(role => role.Name != "Guest") // Loại bỏ vai trò "Guest"
            .Select(r => r.Name)
            .ToList();

        var model = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PasswordHash = user.PasswordHash,
            Roles = userRoles,
            AllRoles = allRoles, // Set all roles without "Guest"
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        // Update the user properties
        user.Email = model.Email;
        user.EmailConfirmed = model.EmailConfirmed;

        // Get the current user roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Determine roles to be added (roles in the model that are not in the current roles)
        var rolesToAdd = model.Roles.Except(currentRoles);

        // Determine roles to be removed (roles in the current roles that are not in the model)
        var rolesToRemove = currentRoles.Except(model.Roles);

        // Add new roles
        foreach (var role in rolesToAdd)
        {
            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                AddErrors(addResult);
                return View(model);
            }
        }

        // Remove roles not in the model
        foreach (var role in rolesToRemove)
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, role);
            if (!removeResult.Succeeded)
            {
                AddErrors(removeResult);
                return View(model);
            }
        }

        // Save changes
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            AddErrors(updateResult);
            return View(model);
        }

        await _context.SaveChangesAsync(); // Save changes in the database

        return RedirectToAction("Index");
    }


    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    // POST: /Account/Delete
    // GET: /Account/Delete
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userDetails = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PasswordHash = user.PasswordHash,
            Roles = await _userManager.GetRolesAsync(user)
        };

        return View(userDetails);
    }

    // POST: /Account/DeleteConfirmed
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return BadRequest("No current user found");
        }

        if (currentUser.Id == user.Id)
        {
            return BadRequest("Cannot delete the currently logged in admin account");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest("Failed to delete user");
        }

        return RedirectToAction(nameof(Index));
    }
}
