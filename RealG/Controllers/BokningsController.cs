using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealG.Data;
using RealG.Models;

namespace RealG.Controllers
{
    public class BokningsController : Controller
    {
        private readonly RealGContext _context;

        public BokningsController(RealGContext context)
        {
            _context = context;
        }

        // GET: Boknings
        public async Task<IActionResult> Index()
        {
            var realGContext = _context.Bokning.Include(b => b.Föreställning);
            return View(await realGContext.ToListAsync());
        }





        [HttpPost]
        public async Task<IActionResult> SearchBookings(string bookingId)
        {
            if (!string.IsNullOrWhiteSpace(bookingId))
            {
                // Perform a database query to find the booking by ID
                var booking = await _context.Bokning
                    .Include(b => b.Föreställning)
                    .FirstOrDefaultAsync(b => b.BokningId == Convert.ToInt32(bookingId));

                if (booking != null)
                {
                    // Fetch the associated movie name using the MovieId
                    var movieName = await _context.Movie
                        .Where(m => m.Id == booking.MovieId)
                        .Select(m => m.Titel)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(movieName))
                    {
                        // You can use movieName here as needed
                        // For example, you can store it in TempData
                        TempData["MovieName"] = movieName;

                        // Render the Bokningar view with the booking details
                        return View("Bokningar", new List<Bokning> { booking });
                    }
                }
            }

            // If the booking is not found or the movie name is not found, redirect back to the same view with an error message
            TempData["ErrorMessage"] = "Booking or movie not found.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Confirmation(string day, string name, string email, string selectedSeats, int selectedShowtimeId, string namn)
        {
            // Process the booking information here
            // ...

            // Pass the booking details to the view
            ViewBag.BookingDetails = new { Day = day, Name = name, Email = email, SelectedSeats = selectedSeats , ShowtimeId = selectedShowtimeId};
            return View();
        }











    }
}
