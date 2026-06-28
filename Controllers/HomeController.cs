using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;

namespace gestionpaises.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCountries = await _context.Countries.CountAsync();
            ViewBag.TotalCities = await _context.Cities.CountAsync();
            ViewBag.TotalLanguages = await _context.CountryLanguages.CountAsync();
            return View();
        }

        public async Task<IActionResult> Reports(string id = "MEX")
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Code == id);

            if (country == null)
            {
                country = await _context.Countries.FirstOrDefaultAsync();
            }

            ViewBag.CountriesList = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
            return View(country);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
