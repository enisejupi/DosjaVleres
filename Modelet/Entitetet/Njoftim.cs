using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("Njoftimet")]
    public class Njoftim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Titulli")]
        public string Titulli { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Përshkrimi")]
        public string Pershkrimi { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Lloji")]
        public string Lloji { get; set; } = "Info"; // Info, Success, Warning, Error

        [MaxLength(500)]
        [Display(Name = "Linku")]
        public string? Linku { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Përdoruesi")]
        public string Perdoruesi { get; set; } = string.Empty;

        [Display(Name = "Është lexuar")]
        public bool EshteLexuar { get; set; } = false;

        [Display(Name = "Data e krijimit")]
        public DateTime DataKrijimit { get; set; } = DateTime.UtcNow;

        [Display(Name = "Data e leximit")]
        public DateTime? DataLeximit { get; set; }

        [MaxLength(50)]
        [Display(Name = "Ikona")]
        public string? Ikona { get; set; } = "fa-bell";
    }
}
