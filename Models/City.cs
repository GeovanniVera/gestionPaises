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

        [Required]
        [Column("Name")]
        [StringLength(35)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("CountryCode")]
        [StringLength(3)]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        [Column("District")]
        [StringLength(20)]
        public string District { get; set; } = string.Empty;

        [Column("Population")]
        public int Population { get; set; }

        // Navigation property to the Country entity
        [ForeignKey("CountryCode")]
        [ValidateNever]
        public virtual Country Country { get; set; } = null!;
    }
}