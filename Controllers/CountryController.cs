using gestionpaises.Data;
using gestionpaises.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var countries = await _context.Countries
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(countries);
        }

        // GET: Country/Details/MEX
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Code == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Country/Create
        [Authorize(Roles = "Editor,Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Country/Edit/MEX
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Country/Edit/MEX
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string id, Country country)
        {
            if (id != country.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CountryExists(country.Code))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Country/Delete/MEX
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Code == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Country/Delete/MEX
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CountryExists(string code)
        {
            return await _context.Countries.AnyAsync(e => e.Code == code);
        }

    }
}
