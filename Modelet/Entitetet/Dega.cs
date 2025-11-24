using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    [Table("Deget")]
    public class Dega
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kodi i degës është i detyrueshëm")]
        [StringLength(20)]
        [Display(Name = "Kodi i degës")]
        public string KodiDeges { get; set; } = string.Empty;

        [Required(ErrorMessage = "Emri i degës është i detyrueshëm")]
        [StringLength(100)]
        [Display(Name = "Emri i degës")]
        public string EmriDeges { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Qyteti")]
        public string? Qyteti { get; set; }

        [StringLength(200)]
        [Display(Name = "Adresa")]
        public string? Adresa { get; set; }

        [StringLength(50)]
        [Display(Name = "Telefoni")]
        public string? Telefoni { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Është aktiv")]
        public bool Eshte_Aktiv { get; set; } = true;

        [Display(Name = "Data e krijimit")]
        public DateTime Krijuar_Me { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Krijuar nga")]
        public string? Krijuar_Nga { get; set; }

        public virtual ICollection<Perdoruesi>? Perdoruesit { get; set; }
    }
}
