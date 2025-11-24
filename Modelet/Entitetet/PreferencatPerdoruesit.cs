using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("PreferencatPerdoruesit")]
    public class PreferencatPerdoruesit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Perdoruesi { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LlojiPreferences { get; set; } = string.Empty; // PrintFormat, ModulCustomization, ColorTheme, etc.

        [Required]
        public string Vlera { get; set; } = "{}"; // JSON data

        public DateTime KrijuarMe { get; set; } = DateTime.UtcNow;

        public DateTime? PerditesomMe { get; set; }
    }
}
