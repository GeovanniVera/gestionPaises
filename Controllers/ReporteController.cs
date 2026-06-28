using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;
using Rotativa.AspNetCore;

namespace gestionpaises.Controllers
{
    [Authorize(Roles = "Consulta,Editor,Administrador")]
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ruta sugerida por el profesor: /Reporte/Pais/MEX o por nombre /Reporte/Pais/Mexico
        [Route("Reporte/Pais/{id}")]
        public async Task<IActionResult> Pais(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Code == id || c.Name == id);

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

            var country = await _context.Countries
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Code == id || c.Name == id);

            if (country == null)
            {
                return NotFound();
            }

            // Rotativa renderizará la vista "Pais" del subdirectorio Reporte y la convertirá a PDF
            return new ViewAsPdf("Pais", country)
            {
                FileName = $"Reporte_{country.Code}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--enable-local-file-access"
            };
        }
    }
}