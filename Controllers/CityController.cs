using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: City
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .Include(c => c.Country)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cities);
        }

        // GET: City/Details/5
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: City/Create
        [Authorize(Roles = "Editor,Administrador")]
        public IActionResult Create()
        {
            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name");
            return View();
        }

        // POST: City/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(City city)
        {
            if (ModelState.IsValid)
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", city.CountryCode);
            return View(city);
        }

        // GET: City/Edit/5
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", city.CountryCode);
            return View(city);
        }

        // POST: City/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(int id, City city)
        {
            if (id != city.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CityExists(city.ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", city.CountryCode);
            return View(city);
        }

        // GET: City/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CityExists(int id)
        {
            return await _context.Cities.AnyAsync(e => e.ID == id);
        }
    }
}