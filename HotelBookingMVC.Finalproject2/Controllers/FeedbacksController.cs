using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using Microsoft.AspNetCore.Identity;
using HotelBookingMVC.Finalproject2.Models;
using HotelBookingMVC.Finalproject2.ViewModels;

namespace HotelBookingMVC.Finalproject2.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly HotelBookingDbContext _context;
        private readonly UserManager<HotelUser> _userManager; // Inject UserManager

        public FeedbacksController(HotelBookingDbContext context, UserManager<HotelUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Store the UserManager
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var hotelBookingDbContext = _context.Feedbacks.Include(f => f.Hotel);
            return View(await hotelBookingDbContext.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Hotel)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create(Guid hotelId)
        {
            var hotel = _context.Hotels.FirstOrDefault(h => h.HotelID == hotelId);
            if (hotel == null)
            {
                return NotFound(); // Nếu không tìm thấy HotelID, trả về lỗi 404
            }

            var model = new FeedbackViewModel
            {
                HotelID = hotelId // Gắn sẵn HotelID vào ViewModel
            };

            return View(model);
        }


        // POST: Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelID,StarRating,Comment")] FeedbackViewModel feedbackViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine(string.Join(", ", errors)); // Log validation errors
                ViewData["Hotels"] = new SelectList(_context.Hotels, "HotelID", "Name", feedbackViewModel.HotelID);
                return View(feedbackViewModel);
            }

            var feedback = new Feedback
            {
                FeedbackID = Guid.NewGuid(),
                HotelID = feedbackViewModel.HotelID,
                StarRating = feedbackViewModel.StarRating,
                Comment = feedbackViewModel.Comment,
                DateCreated = DateTime.Now,
                UserID = _userManager.GetUserId(User)
            };

            _context.Add(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["HotelID"] = new SelectList(_context.Hotels, "HotelID", "Address", feedback.HotelID);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FeedbackID,HotelID,StarRating,DateCreated,Comment,UserID")] Feedback feedback)
        {
            if (id != feedback.FeedbackID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelID"] = new SelectList(_context.Hotels, "HotelID", "Address", feedback.HotelID);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Hotel)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(Guid id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackID == id);
        }
    }
}
