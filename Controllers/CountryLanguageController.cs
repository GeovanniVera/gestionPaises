using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Models;
using gestionpaises.Repositories.Interfaces;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryLanguageController : Controller
    {
        private readonly ICountryLanguageRepository _countryLanguageRepository;
        private readonly ICountryRepository _countryRepository;

        public CountryLanguageController(
            ICountryLanguageRepository countryLanguageRepository,
            ICountryRepository countryRepository)
        {
            _countryLanguageRepository = countryLanguageRepository;
            _countryRepository = countryRepository;
        }

        // GET: CountryLanguage
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var languages = await _countryLanguageRepository.GetAllAsync();
            var sortedLanguages = languages
                .OrderBy(cl => cl.Country?.Name ?? "")
                .ThenBy(cl => cl.Language)
                .ToList();

            return View(sortedLanguages);
        }

        // GET: CountryLanguage/Details/MEX/Spanish
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(string countryCode, string language)
        {
            if (countryCode == null || language == null)
            {
                return NotFound();
            }

            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

            if (countryLanguage == null)
            {
                return NotFound();
            }

            return View(countryLanguage);
        }

        // GET: CountryLanguage/Create
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create()
        {
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(CountryLanguage countryLanguage)
        {
            // Convertir código a mayúsculas y re-validar
            countryLanguage.CountryCode = countryLanguage.CountryCode?.ToUpper() ?? string.Empty;
            ModelState.Remove("CountryCode");
            TryValidateModel(countryLanguage);

            bool yaExiste = await _countryLanguageRepository.ExistsAsync(
                countryLanguage.CountryCode,
                countryLanguage.Language);

            if (yaExiste)
            {
                ModelState.AddModelError("Language", "Este idioma ya está registrado para este país.");
            }

            if (ModelState.IsValid)
            {
                await _countryLanguageRepository.AddAsync(countryLanguage);
                return RedirectToAction(nameof(Index));
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", countryLanguage.CountryCode);
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

            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

            if (countryLanguage == null)
            {
                return NotFound();
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", countryLanguage.CountryCode);
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
                    await _countryLanguageRepository.UpdateAsync(countryLanguage);
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool existe = await _countryLanguageRepository.ExistsAsync(countryCode, language);

                    if (!existe)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", countryLanguage.CountryCode);
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

            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

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
            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

            if (countryLanguage != null)
            {
                await _countryLanguageRepository.DeleteAsync(countryLanguage);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}