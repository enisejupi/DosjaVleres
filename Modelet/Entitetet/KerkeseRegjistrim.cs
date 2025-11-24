using System.ComponentModel.DataAnnotations;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    public class KerkeseRegjistrim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? EmriPlote { get; set; }

        [MaxLength(100)]
        public string? Departamenti { get; set; }

        [MaxLength(100)]
        public string? Pozicioni { get; set; }

        [MaxLength(20)]
        public string? KodiZyrtarit { get; set; }

        public int? Dega_Id { get; set; }

        [MaxLength(100)]
        public string? EmriDegës { get; set; }

        public DateTime DataKerkeses { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Statusi { get; set; } = "NePritje"; // NePritje, Aprovuar, Refuzuar

        public DateTime? DataShqyrtimit { get; set; }

        [MaxLength(450)]
        public string? ShqyrtuesId { get; set; }

        [MaxLength(500)]
        public string? ArsetimiRefuzimit { get; set; }

        [MaxLength(100)]
        public string? ADUsername { get; set; }

        public bool EshteVerifikuarNeAD { get; set; } = false;
    }

    public static class StatusiKerkeses
    {
        public const string NePritje = "NePritje";
        public const string Aprovuar = "Aprovuar";
        public const string Refuzuar = "Refuzuar";
    }
}
