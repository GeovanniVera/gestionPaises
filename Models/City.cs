using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace gestionpaises.Models
{
    [Table("city")]
    public class City
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio.")]
        [Column("Name")]
        [StringLength(35, ErrorMessage = "El nombre de la ciudad no puede exceder los 35 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es obligatorio.")]
        [Column("CountryCode")]
        [StringLength(3, ErrorMessage = "El código del país debe tener exactamente 3 caracteres.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El distrito o estado es obligatorio.")]
        [Column("District")]
        [StringLength(20, ErrorMessage = "El distrito o estado no puede exceder los 20 caracteres.")]
        public string District { get; set; } = string.Empty;

        [Column("Population")]
        [Required(ErrorMessage = "La población es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La población no puede ser un número negativo.")]
        public int Population { get; set; }

        // Navigation property to the Country entity
        [ForeignKey("CountryCode")]
        [ValidateNever]
        public virtual Country Country { get; set; } = null!;
    }
}