using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("VleratProdukteve")]
    public class VleraProduktit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kodi i produktit është i detyrueshëm")]
        [MaxLength(50)]
        [Column("KodiProduktit")]
        public string KodiProduktit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Emri i produktit është i detyrueshëm")]
        [MaxLength(200)]
        [Column("EmriProduktit")]
        public string EmriProduktit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kodi HS është i detyrueshëm")]
        [MaxLength(10)]
        [Column("Kodi")]
        public string Kodi { get; set; } = string.Empty;

        [NotMapped]
        public string KodiHS 
        { 
            get => Kodi; 
            set => Kodi = value; 
        }

        /// <summary>
        /// Përshkrimi i detajuar i produktit
        /// </summary>
        [Required(ErrorMessage = "Përshkrimi është i detyrueshëm")]
        [MaxLength(2000)]
        [Column("Pershkrimi")]
        public string Pershkrimi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Origjina është e detyrueshme")]
        [MaxLength(50)]
        [Column("Origjina")]
        public string Origjina { get; set; } = string.Empty;

        [MaxLength(10)]
        [Column("Pariteti")]
        public string? Pariteti { get; set; }

        [Required(ErrorMessage = "Njësia është e detyrueshme")]
        [MaxLength(50)]
        [Column("Njesia")]
        public string Njesia { get; set; } = string.Empty;

        [NotMapped]
        public string NjesiaMatese 
        { 
            get => Njesia; 
            set => Njesia = value; 
        }

        /// <summary>
        /// Çmimi i produktit (mund të jetë më shumë se një çmim, p.sh. "1.20 ; 1.45")
        /// </summary>
        [MaxLength(200)]
        [Column("Cmimi")]
        public string? Cmimi { get; set; }

        [Column("VleraDoganore")]
        public decimal VleraDoganore { get; set; } = 0m;

        [Required(ErrorMessage = "Valuta është e detyrueshme")]
        [MaxLength(3)]
        [Column("Valuta")]
        public string Valuta { get; set; } = "EUR";

        [Required(ErrorMessage = "Kategoria është e detyrueshme")]
        [MaxLength(100)]
        [Column("Kategoria")]
        public string Kategoria { get; set; } = "Tjera";

        [MaxLength(2000)]
        [Column("Komentet")]
        public string? Komentet { get; set; }

        /// <summary>
        /// Shtegu i bashkëngjitjes (dokumenti, imazhi, etj.)
        /// </summary>
        [MaxLength(500)]
        [Column("Bashkangjitje")]
        [Display(Name = "Bashkëngjitje")]
        public string? Bashkangjitje { get; set; }

        /// <summary>
        /// Emri origjinal i skedarit të bashkëngjitur
        /// </summary>
        [MaxLength(255)]
        [Column("EmeriBashkangjitjes")]
        [Display(Name = "Emëri i bashkëngjitjes")]
        public string? EmeriBashkangjitjes { get; set; }

        [Column("Eshte_Aktiv")]
        public bool Eshte_Aktiv { get; set; } = true;

        [Column("Krijuar_Me")]
        public DateTime Krijuar_Me { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Përdoruesi që ka krijuar rekordin
        /// </summary>
        [MaxLength(100)]
        [Column("Krijuar_Nga")]
        public string? Krijuar_Nga { get; set; }

        [Column("Modifikuar_Me")]
        public DateTime? Modifikuar_Me { get; set; }

        /// <summary>
        /// Përdoruesi që ka modifikuar rekordin
        /// </summary>
        [MaxLength(100)]
        [Column("Modifikuar_Nga")]
        public string? Modifikuar_Nga { get; set; }

        // Navigation properties
        public virtual ICollection<Komenti>? KomentList { get; set; }

        public virtual ICollection<HistoriaVlerave>? HistoriaVlerave { get; set; }

        /// <summary>
        /// Navigation property për imazhet e produktit
        /// </summary>
        public virtual ICollection<ImazhetProduktit>? Imazhet { get; set; }

        
        [NotMapped]
        public DateTime DataRegjistrimit
        {
            get => Krijuar_Me;
            set => Krijuar_Me = value;
        }

        [NotMapped]
        public string? BurimiInformacionit { get; set; }

        [NotMapped]
        public string? VendiOrigjines
        {
            get => Origjina;
            set => Origjina = value;
        }

        [NotMapped]
        public bool Aktiv
        {
            get => Eshte_Aktiv;
            set => Eshte_Aktiv = value;
        }

        [NotMapped]
        public decimal Vlera
        {
            get => VleraDoganore;
            set => VleraDoganore = value;
        }

        [NotMapped]
        public ICollection<HistoriaVlerave>? Historia => HistoriaVlerave;

        [NotMapped]
        public string? KodiTarifar
        {
            get => Kodi;
            set => Kodi = value;
        }
    }
}

