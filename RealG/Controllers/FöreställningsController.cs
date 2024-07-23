using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealG.Data;

namespace RealG.Controllers
{
    public class FöreställningsController : Controller

    {
        private readonly RealGContext _context; 
        
        public FöreställningsController(RealGContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }// Inside your controller class
        public ActionResult Showtimes()
        {
            // Assuming you have a DbContext instance called context
            var showtimes = _context.Föreställning
                                   .Include(f => f.Film) // Including related Movie data if necessary
                                   .Include(f => f.Salong) // Including related Salong data if necessary
                                   .ToList();

            // Pass the list of showtimes to the view
            return View(showtimes);
        }

    }
}
