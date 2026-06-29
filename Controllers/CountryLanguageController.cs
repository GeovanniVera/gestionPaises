using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Models;
using gestionpaises.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryLanguageController : Controller
    {
        private readonly ICountryLanguageRepository _countryLanguageRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<CountryLanguageController> _logger;

        // Expresión regular que permite letras del alfabeto, espacios, guiones, 
        // la letra ñ/Ñ y todas las vocales con acento (áéíóúÁÉÍÓÚ).
        private readonly string _patronIdiomaSeguro = @"^[A-Za-z\s-ñÑáéíóúÁÉÍÓÚ]+$";

        public CountryLanguageController(
            ICountryLanguageRepository countryLanguageRepository,
            ICountryRepository countryRepository,
            ILogger<CountryLanguageController> logger)
        {
            _countryLanguageRepository = countryLanguageRepository;
            _countryRepository = countryRepository;
            _logger = logger;
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

        // GET: CountryLanguage/Details/MEX/Español
        [HttpGet("CountryLanguage/Details/{countryCode}/{language}")]
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(string countryCode, string language)
        {
            countryCode = countryCode?.ToUpper() ?? string.Empty;

            // Validación estricta para visualización: código de 3 caracteres e idioma limpio
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 3 ||
                string.IsNullOrEmpty(language) || !Regex.IsMatch(language, _patronIdiomaSeguro))
            {
                _logger.LogWarning("Parámetros no válidos detectados en Details para {Code}/{Lang}", countryCode, language);
                return BadRequest("Los parámetros proporcionados son inválidos o contienen caracteres no permitidos.");
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

        // GET: CountryLanguage/Edit/MEX/Español
        [HttpGet("CountryLanguage/Edit/{countryCode}/{language}")]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string countryCode, string language)
        {
            countryCode = countryCode?.ToUpper() ?? string.Empty;

            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 3 ||
                string.IsNullOrEmpty(language) || !Regex.IsMatch(language, _patronIdiomaSeguro))
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

        // POST: CountryLanguage/Edit/MEX/Español
        [HttpPost("CountryLanguage/Edit/{countryCode}/{language}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string countryCode, string language, CountryLanguage countryLanguage)
        {
            countryCode = countryCode?.ToUpper() ?? string.Empty;
            countryLanguage.CountryCode = countryLanguage.CountryCode?.ToUpper() ?? string.Empty;

            if (countryCode != countryLanguage.CountryCode || language != countryLanguage.Language)
            {
                return NotFound();
            }

            ModelState.Remove("CountryCode");
            ModelState.Remove("Language");
            TryValidateModel(countryLanguage);

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

        // GET: CountryLanguage/Delete/BEL/<script>...
        // Usamos el parámetro comodín {*language} para capturar cadenas complejas con '/' de forma nativa
        [HttpGet("CountryLanguage/Delete/{countryCode}/{*language}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string countryCode, string language)
        {
            countryCode = countryCode?.ToUpper() ?? string.Empty;

            // Al eliminar, omitimos el filtro Regex estricto para posibilitar el purgado de datos hostiles
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 3 || string.IsNullOrEmpty(language))
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

        // POST: CountryLanguage/Delete/BEL/<script>...
        [HttpPost("CountryLanguage/Delete/{countryCode}/{*language}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(string countryCode, string language)
        {
            countryCode = countryCode?.ToUpper() ?? string.Empty;

            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(language))
            {
                return NotFound();
            }

            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

            if (countryLanguage != null)
            {
                await _countryLanguageRepository.DeleteAsync(countryLanguage);
                _logger.LogInformation("Registro XSS almacenado {Code}/{Lang} eliminado con éxito.", countryCode, language);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: CountryLanguage/FuerzaBrutaDelete?countryCode=BEL&language=<script>...
        [HttpGet("CountryLanguage/FuerzaBrutaDelete")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> FuerzaBrutaDelete(string countryCode, string language)
        {
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(language))
            {
                return BadRequest("Faltan parámetros.");
            }

            var countryLanguage = await _countryLanguageRepository.GetByKeyAsync(countryCode, language);

            if (countryLanguage != null)
            {
                await _countryLanguageRepository.DeleteAsync(countryLanguage);
                _logger.LogInformation("Registro XSS eliminado mediante QueryString.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}