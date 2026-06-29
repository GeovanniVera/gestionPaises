using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestionpaises.Models
{
    [Table("country")]
    public class Country
    {
        [Key]
        [Column("Code")]
        [Required(ErrorMessage = "El código del país es obligatorio.")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "El código debe constar de exactamente 3 letras en mayúsculas.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del país es obligatorio.")]
        [Column("Name")]
        [StringLength(52, ErrorMessage = "El nombre del país no puede exceder los 52 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El continente es obligatorio.")]
        [Column("Continent")]
        public string Continent { get; set; } = string.Empty;

        [Required(ErrorMessage = "La región geográfica es obligatoria.")]
        [Column("Region")]
        [StringLength(26, ErrorMessage = "La región geográfica no puede exceder los 26 caracteres.")]
        public string Region { get; set; } = string.Empty;

        [Column("SurfaceArea")]
        [Required(ErrorMessage = "La superficie territorial es obligatoria.")]
        [Range(0.01, 99999999.99, ErrorMessage = "La superficie territorial debe ser mayor a 0.")]
        public decimal SurfaceArea { get; set; }

        [Column("IndepYear")]
        [Range(-5000, 2026, ErrorMessage = "El año de independencia debe ser un año histórico válido (entre -5000 y 2026).")]
        public short? IndepYear { get; set; }

        [Column("Population")]
        [Required(ErrorMessage = "La población es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La población no puede ser un número negativo.")]
        public int Population { get; set; }

        [Column("LifeExpectancy")]
        [Range(0.0, 100.0, ErrorMessage = "La expectativa de vida debe estar entre 0 y 100 años.")]
        public decimal? LifeExpectancy { get; set; }

        [Column("GNP")]
        [Range(0.00, 9999999999.99, ErrorMessage = "El Producto Nacional Bruto (GNP) debe ser mayor o igual a 0.")]
        public decimal? GNP { get; set; }

        [Column("GNPOld")]
        [Range(0.00, 9999999999.99, ErrorMessage = "El Producto Nacional Bruto (GNP) anterior debe ser mayor o igual a 0.")]
        public decimal? GNPOld { get; set; }

        [Required(ErrorMessage = "El nombre local es obligatorio.")]
        [Column("LocalName")]
        [StringLength(45, ErrorMessage = "El nombre local no puede exceder los 45 caracteres.")]
        public string LocalName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La forma de gobierno es obligatoria.")]
        [Column("GovernmentForm")]
        [StringLength(45, ErrorMessage = "La forma de gobierno no puede exceder los 45 caracteres.")]
        public string GovernmentForm { get; set; } = string.Empty;

        [Column("HeadOfState")]
        [StringLength(60, ErrorMessage = "El jefe de estado no puede exceder los 60 caracteres.")]
        public string? HeadOfState { get; set; }

        [Column("Capital")]
        public int? Capital { get; set; }

        [Required(ErrorMessage = "El código alternativo es obligatorio.")]
        [Column("Code2")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "El código alternativo debe constar de exactamente 2 letras en mayúsculas.")]
        public string Code2 { get; set; } = string.Empty;

        [Column("BanderaPath")]
        [StringLength(255)]
        public string? BanderaPath { get; set; }

        // Navigation properties
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        public virtual ICollection<CountryLanguage> CountryLanguages { get; set; } = new List<CountryLanguage>();
    }
}
