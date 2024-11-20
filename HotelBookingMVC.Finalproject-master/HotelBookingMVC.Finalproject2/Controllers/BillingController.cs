//using Microsoft.AspNetCore.Mvc;
//using HotelBookingMVC.Finalproject2.ViewModels;
//using HotelBookingMVC.Finalproject2.Models; // Assuming your models are in this namespace
//using System.Threading.Tasks;
//using HotelBookingMVC.Finalproject2.Data.Entities;
//using HotelBookingMVC.Finalproject2.Data;

//public class BillingController : Controller
//{
//    private readonly HotelBookingDbContext _context;

//    public BillingController(HotelBookingDbContext context)
//    {
//        _context = context;
//    }

//    // POST: Show Bill
//    [HttpPost]
//    public async Task<IActionResult> ShowBill(OrderViewModel model)
//    {
//        // Check if the model state is valid
//        if (ModelState.IsValid)
//        {
//            // Map the view model to your Bill model
//            var bill = new Bill
//            {
//                FirstName = model.FirstName,
//                LastName = model.LastName,
//                Email = model.Email,
//                Address = model.Address,
//                Address2 = model.Address2,
//                Country = model.Country,
//                State = model.State,
//                Zip = model.Zip,
//                IsShippingSameAsBilling = model.IsShippingSameAsBilling,
//                SaveInfoForNextTime = model.SaveInfoForNextTime,
//                DateCreatedAt = DateTime.UtcNow, // Assuming you want to set creation time
//                // Add other necessary properties
//            };

//            // Save the bill to the database
//            _context.Bills.Add(bill);
//            await _context.SaveChangesAsync();

//            // Optionally redirect to a confirmation page or back to the billing info page
//            return RedirectToAction("Confirmation"); // Create a confirmation action/view as needed
//        }

//        // If the model is invalid, return the same view with the current model to display validation errors
//        return View("BillingForm", model);
//    }

//    // GET: Show Bill
//    [HttpGet]
//    public IActionResult ShowBill(OrderViewModel model)
//    {
//        // Display the bill with the provided model data
//        return View(model);
//    }

//    // Confirmation action
//    public IActionResult Confirmation()
//    {
//        return View();
//    }
//}
