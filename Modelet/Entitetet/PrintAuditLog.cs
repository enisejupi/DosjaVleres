using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    /// <summary>
    /// Entiteti për ruajtjen e auditimit të të gjitha operacioneve të printimit, PDF dhe Excel.
    /// Ky është një entitet kritik i sigurisë për të monitoruar se kush printon çfarë dhe kur.
    /// </summary>
    [Table("PrintAuditLogs")]
    public class PrintAuditLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Emri i përdoruesit që ka kryer printimin/eksportimin
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Përdoruesi")]
        public string Perdoruesi { get; set; } = string.Empty;

        /// <summary>
        /// Lloji i raportit: VleratProdukteve, KomentetDegeve, ShpenzimiTransportit, DosjaTeDisponueshme
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Lloji i raportit")]
        public string LlojiRaportit { get; set; } = string.Empty;

        /// <summary>
        /// Formati i eksportimit: Print, PDF, Excel
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Formati i eksportimit")]
        public string FormatiEksportimit { get; set; } = string.Empty;

        /// <summary>
        /// Numri i rekordeve që janë printuar/eksportuar
        /// </summary>
        [Display(Name = "Numri i rekordeve")]
        public int NumriRekordeve { get; set; }

        /// <summary>
        /// Filtrat e aplikuar gjatë printimit (ruhet si JSON)
        /// </summary>
        [Display(Name = "Filtrat")]
        public string? Filtrat { get; set; }

        /// <summary>
        /// ID e formatit të printimit që është përdorur
        /// </summary>
        [Display(Name = "ID e formatit të printimit")]
        public int? FormatPrintimiId { get; set; }

        /// <summary>
        /// Lidhja me formatin e printimit
        /// </summary>
        [ForeignKey("FormatPrintimiId")]
        public FormatPrintimi? FormatPrintimi { get; set; }

        /// <summary>
        /// Data dhe ora e printimit
        /// </summary>
        [Required]
        [Display(Name = "Data e printimit")]
        public DateTime DataPrintimit { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Adresa IP e përdoruesit
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Adresa IP")]
        public string? AdresaIP { get; set; }

        /// <summary>
        /// User Agent (informacion i browser-it)
        /// </summary>
        [StringLength(500)]
        [Display(Name = "User Agent")]
        public string? UserAgent { get; set; }

        /// <summary>
        /// ID e sesionit (nëse është i disponueshëm)
        /// </summary>
        [StringLength(100)]
        [Display(Name = "ID e sesionit")]
        public string? SesioniId { get; set; }

        /// <summary>
        /// Shënime shtesë (për raste të veçanta)
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Shënime")]
        public string? Shenime { get; set; }

        /// <summary>
        /// A është printimi i suksesshëm
        /// </summary>
        [Display(Name = "Është i suksesshëm")]
        public bool EshteSuksesshem { get; set; } = true;

        /// <summary>
        /// Mesazhi i gabimit (nëse ka pasur gabim)
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Mesazhi i gabimit")]
        public string? MesazhiGabimit { get; set; }
    }
}
