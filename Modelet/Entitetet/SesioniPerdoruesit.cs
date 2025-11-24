using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("SesionetPerdoruesve")]
    public class SesioniPerdoruesit
    {
        [Key]
        [MaxLength(100)]
        [Column("Sesioni_Id")]
        public string Sesioni_Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Emri i përdoruesit është i detyrueshëm")]
        [MaxLength(100)]
        [Column("Perdoruesi")]
        public string Perdoruesi { get; set; } = string.Empty;

        [Column("Filluar_Me")]
        public DateTime Filluar_Me { get; set; } = DateTime.UtcNow;

        [Column("Perfunduar_Me")]
        public DateTime? Perfunduar_Me { get; set; }

        [Column("Eshte_Aktiv")]
        public bool Eshte_Aktiv { get; set; } = true;

        [MaxLength(50)]
        [Column("AdresaIP")]
        public string? AdresaIP { get; set; }

        [MaxLength(500)]
        [Column("UserAgent")]
        public string? UserAgent { get; set; }

        [Column("Kohezgjatja_Minuta")]
        public int? Kohezgjatja_Minuta { get; set; }

        [Column("AktivitetiI_Fundit")]
        public DateTime AktivitetiI_Fundit { get; set; } = DateTime.UtcNow;

        [Column("NumriVeprimeve")]
        public int NumriVeprimeve { get; set; } = 0;

        [MaxLength(100)]
        [Column("ArsyejaPerfundimit")]
        public string? ArsyejaPerfundimit { get; set; }
    }
}
