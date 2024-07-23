using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealG.Data;
using RealG.Models;

namespace RealG.Controllers
{
 
    public class MoviesController : Controller
    {
        private readonly RealGContext _context;

        public MoviesController(RealGContext context)
        {
            _context = context;
        }

        // GET: Movies
        //public async Task<IActionResult> Index()
        //{
        //    return _context.Movie != null ?
        //                View(await _context.Movie.ToListAsync()) :
        //                Problem("Entity set 'RealGContext.Movie'  is null.");
        //}

        // GET: Movies
        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }

            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Titel!.Contains(searchString));
            }

            return View(await movies.ToListAsync());
        }
       





        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movie
                .Include(m => m.Föreställningar)
                .ThenInclude(f => f.Salong)
                .Include(m => m.Föreställningar)
                .ThenInclude(f => f.Bokningar)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var selectedShowtime = await _context.Föreställning
                .Include(f => f.Salong)
                .FirstOrDefaultAsync(f => f.FöreställningId == id);

            if (selectedShowtime == null)
            {
                return NotFound();
            }

            int seatsPerRow = selectedShowtime.Salong.Number;
            int rows = selectedShowtime.Salong.Row;

            var performances = movie.Föreställningar.Select(f => new
            {
                f.FöreställningId,
                f.Datum,
                f.Tider,
                SalonName = f.Salong.Namn,
                TotalSeats = f.Salong.AntalStolar,
                AvailableSeats = f.Salong.AntalStolar - f.Bokningar.Count
            }).ToList();

            ViewBag.Rows = rows;
            ViewBag.SeatsPerRow = seatsPerRow;
            ViewBag.Performances = performances;
            ViewBag.MovieId = id;
            return View(movie);
        }


        [HttpPost]
        public async Task<IActionResult> Purchase(string name, string email, string selectedSeats, int selectedShowtimeId)
        {
            Console.WriteLine("Selected Seats: " + selectedSeats);
            Console.WriteLine("Selected Showtime ID: " + selectedShowtimeId);

            // Fetch the selected showtime by its ID
            var selectedShowtime = await _context.Föreställning
                .Include(f => f.Salong) // Include the Salon to get the number of chairs
                .FirstOrDefaultAsync(f => f.FöreställningId == selectedShowtimeId);

            if (selectedShowtime == null)
            {
                return Json(new { success = false, message = "Invalid showtime ID" });
            }

            // Fetch the corresponding movie ID
            int movieId = selectedShowtime.FilmId; // Assuming that 'FilmId' is the property that represents the movie ID

            var seats = selectedSeats.Split(',').Select(s => s.Split('-')).Select(seat => new Salong
            {
                Row = int.Parse(seat[0]),
                Number = int.Parse(seat[1])
            }).ToList();

            // Create the booking
            var booking = new Bokning
            {
                KundNamn = name,
                KundEmail = email,
                Föreställning = selectedShowtime,
                BokadDatum = selectedShowtime.Datum + selectedShowtime.Tider,
                StolNummer = selectedSeats ,
                MovieId = movieId // Save the movie ID
            };

            _context.Bokning.Add(booking);
            await _context.SaveChangesAsync();





            return View("~/Views/Boknings/Confirmation.cshtml", booking);


        }


        public IActionResult LoadSeatingChart(int showtimeId)
        {
            var selectedShowtime = _context.Föreställning
                .Include(f => f.Salong)
                .Include(f => f.Bokningar) // Include bookings to get booked seats
                .FirstOrDefault(f => f.FöreställningId == showtimeId);

            if (selectedShowtime == null)
            {
                return NotFound();
            }

            // Extract booked seats
            List<string> bookedSeats = selectedShowtime.Bokningar
                                        .Select(b => b.StolNummer)
                                        .ToList();

            // Generate the HTML for the seating chart
            var seatingChartHtml = GenerateSeatingChartHtml(selectedShowtime.Salong.Row, selectedShowtime.Salong.Number, bookedSeats);

            return Content(seatingChartHtml, "text/html");
        }


        // MoviesController.cs
        private string GenerateSeatingChartHtml(int rows, int seatsPerRow, List<string> bookedSeats)
        {
            var html = new StringBuilder();
            html.Append("<div class='seating-chart'>");

            for (int row = 0; row < rows; row++)
            {
                html.Append("<div class='seat-row'>");
                for (int seatNum = 0; seatNum < seatsPerRow; seatNum++)
                {
                    int seatIndex = row * seatsPerRow + seatNum + 1;
                    string seatId = $"{row}-{seatNum}";
                    string seatClass = bookedSeats.Contains(seatId) ? "seat booked-red" : "seat"; // Use "seat booked-red" for booked seats

                    // Add the "disabled" attribute to prevent selecting booked seats
                    string seatAttributes = bookedSeats.Contains(seatId) ? "disabled" : "";

                    html.AppendFormat("<div class='{0}' data-row='{1}' data-seat='{2}' {3}>{4}</div>", seatClass, row, seatNum, seatAttributes, seatIndex);
                }
                html.Append("</div>");
            }
            html.Append("</div>");

            return html.ToString();
        }









        [HttpGet]
        public async Task<JsonResult> GetPerformancesForDate(string dateString, int movieId)
        {
            DateTime selectedDate;
            if (!DateTime.TryParse(dateString, out selectedDate))
            {
                return Json(new { success = false, message = "Invalid date format" });
            }

            var performances = await _context.Föreställning
                .Include(f => f.Salong)
                .Where(f => f.Datum.Date == selectedDate && f.FilmId == movieId)
                .Select(f => new {
                    föreställningId = f.FöreställningId,
                    salongNamn = f.Salong.Namn,
                    tider = f.Tider,
                    antalStolar = f.Salong.AntalStolar - f.Bokningar.Count
                })
                .ToListAsync();

            var performanceViews = performances.Select(p => new {
                p.föreställningId,
                p.salongNamn,
                tider = p.tider.ToString(@"hh\:mm"),
                p.antalStolar
            }).ToList();

            return Json(new { success = true, performances = performanceViews });
        }
      





    }
}
