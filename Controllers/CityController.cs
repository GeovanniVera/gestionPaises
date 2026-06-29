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
    public class CityController : Controller
    {
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;

        public CityController(ICityRepository cityRepository, ICountryRepository countryRepository)
        {
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
        }

        // GET: City
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Index()
        {
            var cities = await _cityRepository.GetAllAsync();
            var sortedCities = cities.OrderBy(c => c.Name).ToList();
            return View(sortedCities);
        }

        // GET: City/Details/5
        [Authorize(Roles = "Consulta,Editor,Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: City/Create
        [Authorize(Roles = "Editor,Administrador")]
        public async Task<IActionResult> Create()
        {
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
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
                await _cityRepository.AddAsync(city);
                return RedirectToAction(nameof(Index));
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", city.CountryCode);
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

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", city.CountryCode);
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
                    await _cityRepository.UpdateAsync(city);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cityRepository.ExistsAsync(city.ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", city.CountryCode);
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

            var city = await _cityRepository.GetByIdAsync(id.Value);

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
            var city = await _cityRepository.GetByIdAsync(id);
            if (city != null)
            {
                await _cityRepository.DeleteAsync(city);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}