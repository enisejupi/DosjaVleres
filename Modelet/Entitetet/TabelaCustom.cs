using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("TabelatCustom")]
    public class TabelaCustom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Emri i tabelës")]
        public string EmriTabeles { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Përshkrimi")]
        public string? Pershkrimi { get; set; }

        [Required]
        [Display(Name = "Skema (JSON)")]
        public string Skema { get; set; } = "{}"; // JSON që përshkruan kolonat

        [Required]
        [StringLength(100)]
        [Display(Name = "Krijuar nga")]
        public string KrijuarNga { get; set; } = string.Empty;

        [Display(Name = "Data e krijimit")]
        public DateTime KrijuarMe { get; set; } = DateTime.UtcNow;

        [Display(Name = "Është aktive")]
        public bool EshteAktive { get; set; } = true;

        [Display(Name = "Data e përditësimit")]
        public DateTime? PerditesomMe { get; set; }
    }
}
