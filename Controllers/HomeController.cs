using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gestionpaises.Models;
using gestionpaises.Repositories.Interfaces;

namespace gestionpaises.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryLanguageRepository _countryLanguageRepository;

        public HomeController(
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            ICountryLanguageRepository countryLanguageRepository)
        {
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _countryLanguageRepository = countryLanguageRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCountries = (await _countryRepository.GetAllAsync()).Count();
            ViewBag.TotalCities = (await _cityRepository.GetAllAsync()).Count();
            ViewBag.TotalLanguages = (await _countryLanguageRepository.GetAllAsync()).Count();
            return View();
        }

        public async Task<IActionResult> Reports(string id = "MEX")
        {
            var country = await _countryRepository.GetByCodeAsync(id);

            if (country == null)
            {
                country = (await _countryRepository.GetAllAsync()).FirstOrDefault();
            }

            ViewBag.CountriesList = await _countryRepository.GetAllAsync();
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
