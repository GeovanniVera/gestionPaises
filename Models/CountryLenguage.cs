using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace gestionpaises.Models
{
    [Table("countrylanguage")]
    [PrimaryKey(nameof(CountryCode), nameof(Language))]
    public class CountryLanguage
    {
        [Column("CountryCode")]
        [StringLength(3)]
        [Required(ErrorMessage = "El país es obligatorio.")]
        public string CountryCode { get; set; } = string.Empty;

        [Column("Language")]
        [StringLength(30, ErrorMessage = "El nombre del idioma no puede exceder los 30 caracteres.")]
        [Required(ErrorMessage = "El idioma es obligatorio.")]
        public string Language { get; set; } = string.Empty;

        [Column("IsOfficial")]
        public bool IsOfficial { get; set; }

        [Column("Percentage")]
        [Required(ErrorMessage = "El porcentaje es obligatorio.")]
        [Range(0.0, 100.0, ErrorMessage = "El porcentaje de hablantes debe estar entre 0.0% y 100.0%.")]
        public decimal Percentage { get; set; }

        [ForeignKey("CountryCode")]
        [ValidateNever]
        public virtual Country Country { get; set; } = null!;
    }
}