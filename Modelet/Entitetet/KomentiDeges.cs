using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("KomentetDegeve")]
    public class KomentiDeges
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri i degës është i detyrueshëm")]
        [StringLength(100)]
        [Display(Name = "Emri i degës")]
        public string EmriDeges { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kodi tarifar është i detyrueshëm")]
        [StringLength(20)]
        [Display(Name = "Kodi tarifar")]
        public string KodiTarifar { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesazhi është i detyrueshëm")]
        [StringLength(2000)]
        [Display(Name = "Mesazhi")]
        public string Mesazhi { get; set; } = string.Empty;

        [Display(Name = "Data e dërgimit")]
        public DateTime DataDergimit { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Dërguar prej")]
        public string? DergoPrejNga { get; set; }

        [Display(Name = "Është i lexuar")]
        public bool EshteLexuar { get; set; } = false;

        [Display(Name = "Data e leximit")]
        public DateTime? DataLeximit { get; set; }

        [StringLength(100)]
        [Display(Name = "Lexuar nga")]
        public string? LexoNga { get; set; }

        [Display(Name = "Është i zgjidhur")]
        public bool EshteZgjidhur { get; set; } = false;

        [StringLength(2000)]
        [Display(Name = "Përgjigja")]
        public string? Pergjigja { get; set; }

        [Display(Name = "Data e përgjigjes")]
        public DateTime? DataPergjigjes { get; set; }

        [StringLength(100)]
        [Display(Name = "Përgjigjur nga")]
        public string? PergjigjetNga { get; set; }

        // Lidhje me produktin nëse është e mundur
        [Display(Name = "Vlera e produktit")]
        public int? VleraProduktit_Id { get; set; }

        [ForeignKey("VleraProduktit_Id")]
        public virtual VleraProduktit? VleraProduktit { get; set; }
    }
}
