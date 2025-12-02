using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    /// <summary>
    /// Përfaqëson imazhet e lidhura me një produkt doganor
    /// </summary>
    [Table("ImazhetProduktit")]
    public class ImazhetProduktit
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID e produktit për të cilin është ky imazh
        /// </summary>
        [Required]
        [Column("VleraProduktit_Id")]
        public int VleraProduktit_Id { get; set; }

        /// <summary>
        /// Shtegu i imazhit në server
        /// </summary>
        [Required]
        [MaxLength(500)]
        [Column("ShtegimaImazhit")]
        public string ShtegimaImazhit { get; set; } = string.Empty;

        /// <summary>
        /// Emri origjinal i skedarit
        /// </summary>
        [Required]
        [MaxLength(255)]
        [Column("EmriOrigjinal")]
        public string EmriOrigjinal { get; set; } = string.Empty;

        /// <summary>
        /// Lloji i imazhit: "Produkt", "Historia", "Dokument"
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column("LlojiImazhit")]
        public string LlojiImazhit { get; set; } = "Produkt";

        /// <summary>
        /// Përshkrimi opsional i imazhit
        /// </summary>
        [MaxLength(500)]
        [Column("Pershkrimi")]
        public string? Pershkrimi { get; set; }

        /// <summary>
        /// Rradhë shfaqjes (për sorting)
        /// </summary>
        [Column("RradhaShfaqjes")]
        public int RradhaShfaqjes { get; set; } = 0;

        /// <summary>
        /// A është ky imazhi kryesor për produktin?
        /// </summary>
        [Column("EshteImazhKryesor")]
        public bool EshteImazhKryesor { get; set; } = false;

        /// <summary>
        /// Madhësia e skedarit në bytes
        /// </summary>
        [Column("MadhesiaBytes")]
        public long MadhesiaBytes { get; set; }

        /// <summary>
        /// Përdoruesi që ka ngarkuar imazhin
        /// </summary>
        [MaxLength(100)]
        [Column("NgarkuarNga")]
        public string? NgarkuarNga { get; set; }

        /// <summary>
        /// Data e ngarkimit
        /// </summary>
        [Column("NgarkuarMe")]
        public DateTime NgarkuarMe { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property për produktin
        /// </summary>
        [ForeignKey("VleraProduktit_Id")]
        public virtual VleraProduktit? VleraProduktit { get; set; }
    }
}
