using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace gestionpaises.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // 1. Siempre registrar el error en los logs para que tú puedas investigarlo
            _logger.LogError(exception, "Excepción global capturada en el pipeline MVC.");

            // 2. Decidir a dónde enviar al usuario según el entorno o el tipo de error
            // Redirigimos al controlador Home y su acción Error (o una ruta personalizada de errores)
            httpContext.Response.Redirect("/Home/Error");

            // Le indicamos al framework que ya tomamos el control de la excepción
            return true;
        }
    }
}