using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("Komentet")]
    public class Komenti
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("VleraProduktitId")]
        public int VleraProduktitId { get; set; }

        /// <summary>
        /// Përmbajtja e komentit
        /// </summary>
        [Required(ErrorMessage = "Komenti është i detyrueshëm")]
        [MaxLength(2000)]
        [Column("Permbajtja")]
        public string Permbajtja { get; set; } = string.Empty;

        /// <summary>
        /// Përdoruesi që ka krijuar komentin
        /// </summary>
        [MaxLength(100)]
        [Column("Krijuar_Nga")]
        public string? Krijuar_Nga { get; set; }

        [Column("Krijuar_Me")]
        public DateTime Krijuar_Me { get; set; } = DateTime.UtcNow;

        [ForeignKey("VleraProduktitId")]
        public virtual VleraProduktit? VleraProduktit { get; set; }
    }
}
