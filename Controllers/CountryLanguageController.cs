using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryLanguageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryLanguageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CountryLanguage
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var languages = await _context.CountryLanguages
                .Include(cl => cl.Country)
                .OrderBy(cl => cl.Country.Name)
                .ThenBy(cl => cl.Language)
                .ToListAsync();

            return View(languages);
        }

        // GET: CountryLanguage/Details/MEX/Spanish
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(string countryCode, string language)
        {
            if (countryCode == null || language == null)
            {
                return NotFound();
            }

            var countryLanguage = await _context.CountryLanguages
                .Include(cl => cl.Country)
                .FirstOrDefaultAsync(cl => cl.CountryCode == countryCode && cl.Language == language);

            if (countryLanguage == null)
            {
                return NotFound();
            }

            return View(countryLanguage);
        }

        // GET: CountryLanguage/Create
        [Authorize(Roles = "Editor,Administrador")]
        public IActionResult Create()
        {
            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name");
            return View();
        }

        // POST: CountryLanguage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(CountryLanguage countryLanguage)
        {
            bool yaExiste = await _context.CountryLanguages.AnyAsync(cl =>
                cl.CountryCode == countryLanguage.CountryCode &&
                cl.Language == countryLanguage.Language);

            if (yaExiste)
            {
                ModelState.AddModelError(string.Empty, "Ese idioma ya está registrado para este país.");
            }

            if (ModelState.IsValid)
            {
                _context.CountryLanguages.Add(countryLanguage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", countryLanguage.CountryCode);
            return View(countryLanguage);
        }

        // GET: CountryLanguage/Edit/MEX/Spanish
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string countryCode, string language)
        {
            if (countryCode == null || language == null)
            {
                return NotFound();
            }

            var countryLanguage = await _context.CountryLanguages
                .FirstOrDefaultAsync(cl => cl.CountryCode == countryCode && cl.Language == language);

            if (countryLanguage == null)
            {
                return NotFound();
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", countryLanguage.CountryCode);
            return View(countryLanguage);
        }

        // POST: CountryLanguage/Edit/MEX/Spanish
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string countryCode, string language, CountryLanguage countryLanguage)
        {
            if (countryCode != countryLanguage.CountryCode || language != countryLanguage.Language)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(countryLanguage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool existe = await _context.CountryLanguages.AnyAsync(cl =>
                        cl.CountryCode == countryCode && cl.Language == language);

                    if (!existe)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Countries = new SelectList(_context.Countries.OrderBy(c => c.Name), "Code", "Name", countryLanguage.CountryCode);
            return View(countryLanguage);
        }

        // GET: CountryLanguage/Delete/MEX/Spanish
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string countryCode, string language)
        {
            if (countryCode == null || language == null)
            {
                return NotFound();
            }

            var countryLanguage = await _context.CountryLanguages
                .Include(cl => cl.Country)
                .FirstOrDefaultAsync(cl => cl.CountryCode == countryCode && cl.Language == language);

            if (countryLanguage == null)
            {
                return NotFound();
            }

            return View(countryLanguage);
        }

        // POST: CountryLanguage/Delete/MEX/Spanish
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(string countryCode, string language)
        {
            var countryLanguage = await _context.CountryLanguages
                .FirstOrDefaultAsync(cl => cl.CountryCode == countryCode && cl.Language == language);

            if (countryLanguage != null)
            {
                _context.CountryLanguages.Remove(countryLanguage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}