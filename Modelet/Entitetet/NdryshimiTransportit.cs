using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("NdryshimetTransportit")]
    public class NdryshimiTransportit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Shpenzimi i transportit")]
        public int ShpenzimiTransportit_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Çmimi i mëparshëm")]
        public decimal Cmimi_Mepar { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Çmimi i ri")]
        public decimal Cmimi_Ri { get; set; }

        [StringLength(10)]
        [Display(Name = "Valuta e mëparshme")]
        public string? Valuta_Mepar { get; set; }

        [StringLength(10)]
        [Display(Name = "Valuta e re")]
        public string? Valuta_Re { get; set; }

        [StringLength(1000)]
        [Display(Name = "Arsyeja e ndryshimit")]
        public string? ArsyejaE_Ndryshimit { get; set; }

        [Display(Name = "Data e ndryshimit")]
        public DateTime Ndryshuar_Me { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Ndryshuar nga")]
        public string? Ndryshuar_Nga { get; set; }

        [StringLength(50)]
        [Display(Name = "Adresa IP")]
        public string? AdresaIP { get; set; }

        [Display(Name = "Numri i versionit")]
        public int NumriVersionit { get; set; } = 1;

        [NotMapped]
        public DateTime DataNdryshimit
        {
            get => Ndryshuar_Me;
            set => Ndryshuar_Me = value;
        }

        [NotMapped]
        public string? Perdoruesi => Ndryshuar_Nga;

        [NotMapped]
        public string FushaENdryshuar
        {
            get
            {
                var fushat = new List<string>();
                if (Cmimi_Mepar != Cmimi_Ri) fushat.Add($"Cmimi: {Cmimi_Mepar} \u2192 {Cmimi_Ri}");
                if (Valuta_Mepar != Valuta_Re) fushat.Add($"Valuta: {Valuta_Mepar} \u2192 {Valuta_Re}");
                return string.Join(", ", fushat);
            }
        }

        [NotMapped]
        public string VleraVjeter => $"{Cmimi_Mepar:N2} {Valuta_Mepar}";

        [NotMapped]
        public string VleraRe => $"{Cmimi_Ri:N2} {Valuta_Re}";

        [ForeignKey("ShpenzimiTransportit_Id")]
        public virtual ShpenzimiTransportit? ShpenzimiTransportit { get; set; }
    }
}
