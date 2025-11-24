using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("FormatetiPrintimit")]
    public class FormatPrintimi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Emri i formatit")]
        public string EmriFormatit { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Lloji i modulit")]
        public string LlojiModulit { get; set; } = string.Empty; // VleratProdukteve, KomentetDegeve, etc.

        [Display(Name = "HTML Template")]
        public string HtmlTemplate { get; set; } = string.Empty;

        [Display(Name = "CSS Stilet")]
        public string CssStyle { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }

        [StringLength(50)]
        [Display(Name = "Pozicioni i logos")]
        public string? LogoPosition { get; set; } // center, left, right

        [Display(Name = "Madhësia e letrës")]
        [StringLength(20)]
        public string PaperSize { get; set; } = "A4";

        [Required]
        [StringLength(100)]
        [Display(Name = "Krijuar nga")]
        public string KrijuarNga { get; set; } = string.Empty;

        [Display(Name = "Data e krijimit")]
        public DateTime KrijuarMe { get; set; } = DateTime.UtcNow;

        [Display(Name = "Është default")]
        public bool EshteDefault { get; set; } = false;

        [Display(Name = "Data e përditësimit")]
        public DateTime? PerditesomMe { get; set; }
    }
}
