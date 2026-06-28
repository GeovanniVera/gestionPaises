using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestionpaises.Models
{
    [Table("country")]
    public class Country
    {
        [Key]
        [Column("Code")]
        [StringLength(3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Column("Name")]
        [StringLength(52)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("Continent")]
        public string Continent { get; set; } = string.Empty;

        [Required]
        [Column("Region")]
        [StringLength(26)]
        public string Region { get; set; } = string.Empty;

        [Column("SurfaceArea")]
        public decimal SurfaceArea { get; set; }

        [Column("IndepYear")]
        public short? IndepYear { get; set; }

        [Column("Population")]
        public int Population { get; set; }

        [Column("LifeExpectancy")]
        public decimal? LifeExpectancy { get; set; }

        [Column("GNP")]
        public decimal? GNP { get; set; }

        [Column("GNPOld")]
        public decimal? GNPOld { get; set; }

        [Required]
        [Column("LocalName")]
        [StringLength(45)]
        public string LocalName { get; set; } = string.Empty;

        [Required]
        [Column("GovernmentForm")]
        [StringLength(45)]
        public string GovernmentForm { get; set; } = string.Empty;

        [Column("HeadOfState")]
        [StringLength(60)]
        public string? HeadOfState { get; set; }

        [Column("Capital")]
        public int? Capital { get; set; }

        [Required]
        [Column("Code2")]
        [StringLength(2)]
        public string Code2 { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        public virtual ICollection<CountryLanguage> CountryLanguages { get; set; } = new List<CountryLanguage>();
    }
}
