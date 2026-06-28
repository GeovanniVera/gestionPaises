using gestionpaises.Data;
using gestionpaises.Models;
using gestionpaises.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageValidationService _imageValidationService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CountryController> _logger;

        public CountryController(
            ApplicationDbContext context,
            ImageValidationService imageValidationService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<CountryController> logger)
        {
            _context = context;
            _imageValidationService = imageValidationService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(Country country, IFormFile? bandera)
        {
            _logger.LogInformation("Create POST iniciado. Archivo recibido: {NombreArchivo}, tamaño: {Tamaño}",
                bandera?.FileName ?? "null", bandera?.Length ?? 0);

            if (bandera != null)
            {
                _logger.LogInformation("Llamando a ValidarImagen...");
                var (esValido, mensajeError) = _imageValidationService.ValidarImagen(bandera);
                _logger.LogInformation("ValidarImagen terminó. Válido: {EsValido}, Mensaje: {Mensaje}", esValido, mensajeError);

                if (!esValido)
                {
                    ModelState.AddModelError("BanderaPath", mensajeError!);
                }
            }

            _logger.LogInformation("Revisando ModelState.IsValid: {EsValido}", ModelState.IsValid);

            if (ModelState.IsValid)
            {
                if (bandera != null)
                {
                    _logger.LogInformation("Guardando imagen en disco...");
                    var carpetaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banderas");
                    Directory.CreateDirectory(carpetaUploads);

                    var nombreArchivo = await _imageValidationService.GuardarImagenAsync(bandera, carpetaUploads);
                    country.BanderaPath = nombreArchivo;
                    _logger.LogInformation("Imagen guardada como: {NombreArchivo}", nombreArchivo);
                }

                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
                _logger.LogInformation("País guardado correctamente.");
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("ModelState inválido, regresando a la vista.");
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
        public async Task<IActionResult> Edit(string id, Country country, IFormFile? bandera)
        {
            if (id != country.Code)
            {
                return NotFound();
            }

            if (bandera != null)
            {
                var (esValido, mensajeError) = _imageValidationService.ValidarImagen(bandera);

                if (!esValido)
                {
                    ModelState.AddModelError("BanderaPath", mensajeError!);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (bandera != null)
                    {
                        var carpetaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banderas");
                        Directory.CreateDirectory(carpetaUploads);

                        var nombreArchivo = await _imageValidationService.GuardarImagenAsync(bandera, carpetaUploads);
                        country.BanderaPath = nombreArchivo;
                    }
                    else
                    {
                        // Si no se sube una nueva imagen, conservamos la que ya existía en la base.
                        var paisExistente = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Code == id);
                        country.BanderaPath = paisExistente?.BanderaPath;
                    }

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
