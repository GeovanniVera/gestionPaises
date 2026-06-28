namespace gestionpaises.Services
{
    public class ImageValidationService
    {
        private const long TamañoMaximoBytes = 2 * 1024 * 1024; // 2 MB

        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".gif" };

        // Firmas de bytes (magic numbers) reales de cada formato de imagen permitido.
        private static readonly Dictionary<string, List<byte[]>> Firmas = new()
        {
            { ".jpg",  new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".png",  new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".gif",  new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 } } }
        };

        public (bool EsValido, string? MensajeError) ValidarImagen(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return (false, "Debes seleccionar un archivo.");
            }

            if (archivo.Length > TamañoMaximoBytes)
            {
                return (false, "El archivo no debe superar los 2 MB.");
            }

            var extension = Path.GetExtension(archivo.FileName)?.ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrEmpty(extension) || !ExtensionesPermitidas.Contains(extension))
            {
                return (false, "Solo se permiten archivos .jpg, .jpeg, .png o .gif.");
            }

            // Defensa adicional: si la extensión pasó el filtro anterior pero
            // por algún motivo no existe en el diccionario de firmas,
            // rechazamos sin lanzar excepción.
            if (!Firmas.TryGetValue(extension, out var firmasEsperadas))
            {
                return (false, "No se pudo validar el tipo de archivo.");
            }

            try
            {
                // Leemos los primeros bytes del archivo para comparar contra la firma real.
                using var stream = archivo.OpenReadStream();
                var maxLongitudFirma = firmasEsperadas.Max(f => f.Length);
                var buffer = new byte[maxLongitudFirma];

                int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                stream.Position = 0; // Regresamos el cursor al inicio para que el archivo se pueda guardar después.

                bool firmaValida = firmasEsperadas.Any(firma =>
                    bytesLeidos >= firma.Length &&
                    buffer.Take(firma.Length).SequenceEqual(firma));

                if (!firmaValida)
                {
                    return (false, "El contenido del archivo no corresponde a una imagen válida.");
                }
            }
            catch (Exception)
            {
                // Cualquier error inesperado al leer el archivo se trata como inválido;
                // nunca se deja propagar fuera de este método.
                return (false, "No se pudo leer el archivo. Intenta con otro.");
            }

            return (true, null);
        }

        public async Task<string> GuardarImagenAsync(IFormFile archivo, string carpetaDestino)
        {
            var extension = Path.GetExtension(archivo.FileName)?.ToLowerInvariant() ?? string.Empty;
            var nombreNuevo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpetaDestino, nombreNuevo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return nombreNuevo;
        }
    }
}