using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("ShpenzimetTransportit")]
    public class ShpenzimiTransportit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vendi i origjinës është i detyrueshëm")]
        [StringLength(100)]
        [Display(Name = "Vendi i origjinës")]
        public string VendiOrigjines { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendi i destinacionit është i detyrueshëm")]
        [StringLength(100)]
        [Display(Name = "Vendi i destinacionit")]
        public string VendiDestinacionit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lloji i transportit është i detyrueshëm")]
        [StringLength(50)]
        [Display(Name = "Lloji i transportit")]
        public string LlojiTransportit { get; set; } = string.Empty; // Ajror, Detar, Tokësor, Hekurudhor

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cmimi për Kg/Ton")]
        public decimal CmimiPerNjesi { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Valuta")]
        public string Valuta { get; set; } = "EUR";

        [Required]
        [StringLength(20)]
        [Display(Name = "Njësia matëse")]
        public string NjesiaMatese { get; set; } = "KG"; // KG, TON

        [StringLength(500)]
        [Display(Name = "Shënime")]
        public string? Shenime { get; set; }

        [Display(Name = "Është aktiv")]
        public bool Aktiv { get; set; } = true;

        [Display(Name = "Data e krijimit")]
        public DateTime DataKrijimit { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Krijuar nga")]
        public string? KrijuarNga { get; set; }

        [Display(Name = "Data e përditësimit")]
        public DateTime? DataPerditesimit { get; set; }

        [StringLength(100)]
        [Display(Name = "Përditësuar nga")]
        public string? PerditesoPrejNga { get; set; }

        public virtual ICollection<NdryshimiTransportit>? Ndryshimet { get; set; }
    }
}
