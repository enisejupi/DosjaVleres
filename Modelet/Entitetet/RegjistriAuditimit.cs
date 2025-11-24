using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("RegjistriAuditimit")]
    public class RegjistriAuditimit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri i përdoruesit është i detyrueshëm")]
        [MaxLength(100)]
        [Column("Perdoruesi")]
        public string Perdoruesi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lloji i veprimit është i detyrueshëm")]
        [MaxLength(50)]
        [Column("LlojiVeprimit")]
        public string LlojiVeprimit { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("Entiteti")]
        public string Entiteti { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("Entiteti_Id")]
        public string? Entiteti_Id { get; set; }

        [Column("Detajet")]
        public string? Detajet { get; set; }

        [Column("Vlerat_Vjetra")]
        public string? Vlerat_Vjetra { get; set; }

        [Column("Vlerat_Reja")]
        public string? Vlerat_Reja { get; set; }

        [Column("Koha")]
        public DateTime Koha { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        [Column("AdresaIP")]
        public string? AdresaIP { get; set; }

        [MaxLength(500)]
        [Column("UserAgent")]
        public string? UserAgent { get; set; }

        [Column("Eshte_Suksesshem")]
        public bool Eshte_Suksesshem { get; set; } = true;

        [MaxLength(1000)]
        [Column("MesazhiGabimit")]
        public string? MesazhiGabimit { get; set; }

        [MaxLength(100)]
        [Column("Sesioni_Id")]
        public string? Sesioni_Id { get; set; }
    }
}
