using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestionpaises.Models
{
    [Table("countrylanguage")]
    [PrimaryKey(nameof(CountryCode), nameof(Language))]
    public class CountryLanguage
    {
        [Column("CountryCode")]
        [StringLength(3)]
        public string CountryCode { get; set; } = string.Empty;

        [Column("Language")]
        [StringLength(30)]
        public string Language { get; set; } = string.Empty;

        [Column("IsOfficial")]
        public bool IsOfficial { get; set; }

        [Column("Percentage")]
        public decimal Percentage { get; set; }

        // Navigation property to the Country entity
        [ForeignKey("CountryCode")]
        public virtual Country Country { get; set; } = null!;
    }
}