using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("HistoriaVlerave")]
    public class HistoriaVlerave
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("VleraProduktit_Id")]
        public int VleraProduktit_Id { get; set; }

        [ForeignKey("VleraProduktit_Id")]
        public virtual VleraProduktit? VleraProduktit { get; set; }

        [Column("Vlera_Mepar", TypeName = "decimal(18,2)")]
        public decimal Vlera_Mepar { get; set; }

        [Column("Vlera_Re", TypeName = "decimal(18,2)")]
        public decimal Vlera_Re { get; set; }

        [MaxLength(3)]
        [Column("Valuta_Mepar")]
        public string? Valuta_Mepar { get; set; }

        [MaxLength(3)]
        [Column("Valuta_Re")]
        public string? Valuta_Re { get; set; }

        [MaxLength(500)]
        [Column("ArsyejaE_Ndryshimit")]
        public string? ArsyejaE_Ndryshimit { get; set; }

        /// <summary>
        /// Path to the photo/image uploaded with this change
        /// </summary>
        [MaxLength(500)]
        [Column("FotoNdryshimit")]
        public string? FotoNdryshimit { get; set; }

        [Column("Ndryshuar_Me")]
        public DateTime Ndryshuar_Me { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Përdoruesi që ka bërë ndryshimin
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("Ndryshuar_Nga")]
        public string Ndryshuar_Nga { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("AdresaIP")]
        public string? AdresaIP { get; set; }

        [Column("NumriVersionit")]
        public int NumriVersionit { get; set; }

        [NotMapped]
        public DateTime DataNdryshimit
        {
            get => Ndryshuar_Me;
            set => Ndryshuar_Me = value;
        }

        [NotMapped]
        public string? FushaENdryshuar { get; set; }

        [NotMapped]
        public string? VleraVjeter => Vlera_Mepar.ToString();

        [NotMapped]
        public string? VleraRe => Vlera_Re.ToString();

        [NotMapped]
        public string? Perdoruesi => Ndryshuar_Nga;

        [NotMapped]
        public string? Arsyeja => ArsyejaE_Ndryshimit;
    }
}


