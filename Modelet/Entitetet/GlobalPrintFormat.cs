using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    /// <summary>
    /// Entiteti për ruajtjen e formatit global të printimit që aplikohet për të gjithë përdoruesit.
    /// Vetëm administratorët mund ta modifikojnë këtë format.
    /// </summary>
    [Table("GlobalPrintFormats")]
    public class GlobalPrintFormat
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Lloji i modulit: VleratProdukteve, KomentetDegeve, ShpenzimiTransportit, DosjaTeDisponueshme
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Lloji i modulit")]
        public string LlojiModulit { get; set; } = string.Empty;

        /// <summary>
        /// ID e formatit të printimit që është aktiv për këtë modul
        /// </summary>
        [Required]
        [Display(Name = "ID e formatit të printimit")]
        public int FormatPrintimiId { get; set; }

        /// <summary>
        /// Lidhja me formatin e printimit
        /// </summary>
        [ForeignKey("FormatPrintimiId")]
        public FormatPrintimi? FormatPrintimi { get; set; }

        /// <summary>
        /// Emri i administratorit që ka vendosur këtë format
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Vendosur nga")]
        public string VendosurNga { get; set; } = string.Empty;

        /// <summary>
        /// Data kur është vendosur ky format
        /// </summary>
        [Required]
        [Display(Name = "Data e vendosjes")]
        public DateTime DataVendosjes { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Përshkrim i shkurtër i formatit
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Përshkrimi")]
        public string? Pershkrimi { get; set; }

        /// <summary>
        /// A është ky format aktiv
        /// </summary>
        [Display(Name = "Është aktiv")]
        public bool EshteAktiv { get; set; } = true;

        /// <summary>
        /// Data e fundit e modifikimit
        /// </summary>
        [Display(Name = "Modifikuar më")]
        public DateTime? ModifikuarMe { get; set; }

        /// <summary>
        /// Emri i administratorit që ka modifikuar për herë të fundit
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Modifikuar nga")]
        public string? ModifikuarNga { get; set; }
    }
}
