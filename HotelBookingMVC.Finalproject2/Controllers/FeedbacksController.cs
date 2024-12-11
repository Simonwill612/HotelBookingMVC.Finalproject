using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.ViewModels;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    [Authorize(Roles = "Manager")]

    public class FeedbacksController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager;

        public FeedbacksController(HotelBookingDbContext context, UserManager<HotelUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks.Include(f => f.Hotel).ToListAsync();

            var feedbackViewModels = feedbacks.Select(async feedback => new FeedbackViewModel
            {
                FeedbackID = feedback.FeedbackID,
                HotelID = feedback.HotelID,
                HotelName = feedback.Hotel.Name,
                StarRating = feedback.StarRating,
                Comment = feedback.Comment,
                UserID = feedback.UserID,
                UserEmail = (await _userManager.FindByIdAsync(feedback.UserID))?.Email ?? "Unknown",
                DateCreated = feedback.DateCreated
            }).Select(t => t.Result).ToList();

            return View(feedbackViewModels);
        }


        // GET: Feedbacks/Details/5
        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var feedback = await _context.Feedbacks.Include(f => f.Hotel)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);

            if (feedback == null) return NotFound();

            // Lấy thông tin User Email từ UserID
            var user = await _userManager.FindByIdAsync(feedback.UserID);
            var userEmail = user?.Email ?? "Unknown";

            // Truyền email vào View thông qua ViewBag
            ViewBag.UserEmail = userEmail;

            return View(feedback);
        }


       

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            ViewData["HotelID"] = new SelectList(_context.Hotels, "HotelID", "Address", feedback.HotelID);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FeedbackID,HotelID,StarRating,DateCreated,Comment,UserID")] Feedback feedback)
        {
            if (id != feedback.FeedbackID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Feedbacks.Any(e => e.FeedbackID == feedback.FeedbackID))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["HotelID"] = new SelectList(_context.Hotels, "HotelID", "Address", feedback.HotelID);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var feedback = await _context.Feedbacks.Include(f => f.Hotel)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedback == null) return NotFound();

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null) _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
