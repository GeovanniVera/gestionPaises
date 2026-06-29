using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using gestionpaises.Models;
using gestionpaises.Services;
using gestionpaises.Repositories.Interfaces;

namespace gestionpaises.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ImageValidationService _imageValidationService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CountryController> _logger;

        public CountryController(
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            ImageValidationService imageValidationService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<CountryController> logger)
        {
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _imageValidationService = imageValidationService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var countries = await _countryRepository.GetAllAsync();
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

            var country = await _countryRepository.GetCountryDetailsAsync(id);

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
            ViewBag.Capital = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                new List<City>(),
                "ID",
                "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create(Country country, IFormFile? bandera)
        {
            // Convertir códigos a mayúsculas y re-validar
            country.Code = country.Code?.ToUpper() ?? string.Empty;
            country.Code2 = country.Code2?.ToUpper() ?? string.Empty;
            ModelState.Remove("Code");
            ModelState.Remove("Code2");
            TryValidateModel(country);

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

            // Verificar duplicidad de llave primaria
            if (ModelState.IsValid)
            {
                var existing = await _countryRepository.GetByCodeAsync(country.Code);
                if (existing != null)
                {
                    ModelState.AddModelError("Code", "Ya existe un país registrado con este código.");
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

                await _countryRepository.AddAsync(country);
                _logger.LogInformation("País guardado correctamente.");
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("ModelState inválido, regresando a la vista.");
            ViewBag.Capital = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                new List<City>(),
                "ID",
                "Name",
                country.Capital);
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

            var country = await _countryRepository.GetByCodeAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            var countryWithCities = await _countryRepository.GetCountryDetailsAsync(id);
            var cities = countryWithCities?.Cities?.OrderBy(c => c.Name).ToList() ?? new List<City>();

            ViewBag.Capital = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                cities,
                "ID",
                "Name",
                country.Capital);

            return View(country);
        }

        // POST: Country/Edit/MEX
        // POST: Country/Edit/MEX
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Edit(string id, Country country, IFormFile? bandera)
        {
            // Forzamos el ID de la URL en la entidad
            country.Code = id;

            // 1. Eliminamos errores previos del ModelState provocados por el bindeo del input deshabilitado
            ModelState.Remove("Code");
            ModelState.Remove("Code2");

            // Normalizamos el string
            country.Code2 = country.Code2?.ToUpper() ?? string.Empty;

            // 2. Re-validamos limpiamente el objeto completo
            TryValidateModel(country);

            // 3. Procesamos validación de imagen si se adjuntó una nueva
            if (bandera != null)
            {
                var (esValido, mensajeError) = _imageValidationService.ValidarImagen(bandera);

                if (!esValido)
                {
                    ModelState.AddModelError("BanderaPath", mensajeError!);
                }
            }

            // 4. Si el ModelState pasó la prueba sin los errores de bindeo arrastrados
            if (ModelState.IsValid)
            {
                try
                {
                    if (bandera != null)
                    {
                        _logger.LogInformation("Guardando nueva bandera en disco...");
                        var carpetaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banderas");
                        Directory.CreateDirectory(carpetaUploads);

                        var nombreArchivo = await _imageValidationService.GuardarImagenAsync(bandera, carpetaUploads);
                        country.BanderaPath = nombreArchivo;
                    }
                    else
                    {
                        // Recuperamos el registro actual para no perder la bandera que ya tenía guardada
                        var paisExistente = await _countryRepository.GetByCodeAsync(id);
                        country.BanderaPath = paisExistente?.BanderaPath;
                    }

                    // Persistencia limpia usando tu repositorio
                    await _countryRepository.UpdateAsync(country);
                    _logger.LogInformation("País {Codigo} guardado exitosamente vía Repositorio.", id);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _countryRepository.ExistsAsync(country.Code))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            // 5. En caso de fallar alguna validación del negocio, reconstruimos la vista sin romperla
            _logger.LogWarning("ModelState inválido detectado para {Codigo}. Volviendo a procesar vista.", id);

            var countryWithCitiesForEdit = await _countryRepository.GetCountryDetailsAsync(id);
            var citiesForEdit = countryWithCitiesForEdit?.Cities?.OrderBy(c => c.Name).ToList() ?? new List<City>();

            ViewBag.Capital = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                citiesForEdit,
                "ID",
                "Name",
                country.Capital);

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

            var country = await _countryRepository.GetByCodeAsync(id);

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
            var country = await _countryRepository.GetByCodeAsync(id);
            if (country != null)
            {
                await _countryRepository.DeleteAsync(country);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
