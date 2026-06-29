using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using gestionpaises.Repositories.Interfaces;
using Rotativa.AspNetCore;

namespace gestionpaises.Controllers
{
    [Authorize(Roles = "Consulta,Editor,Administrador")]
    public class ReporteController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public ReporteController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        // Ruta sugerida por el profesor: /Reporte/Pais/MEX o por nombre /Reporte/Pais/Mexico
        [Route("Reporte/Pais/{id}")]
        public async Task<IActionResult> Pais(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryReportByCodeAsync(id)
                          ?? await _countryRepository.GetCountryReportByNameAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            // Retorna la vista limpia en HTML (Views/Reporte/Pais.cshtml)
            return View(country);
        }

        // Acción para descargar el PDF utilizando la misma vista "Pais"
        public async Task<IActionResult> DownloadPdf(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryReportByCodeAsync(id)
                          ?? await _countryRepository.GetCountryReportByNameAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            // Rotativa renderizará la vista "Pais" del subdirectorio Reporte y la convertirá a PDF
            var pdfResult = new ViewAsPdf("Pais", country)
            {
                FileName = $"Reporte_{country.Code}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--enable-local-file-access"
            };
            pdfResult.ViewData["IsPdf"] = true;
            return pdfResult;
        }
    }
}