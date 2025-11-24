using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KosovaDoganaModerne.Modelet.Entitetet
{
    public class Perdoruesi : IdentityUser
    {
        [MaxLength(100)]
        public string? EmriPlote { get; set; }

        [MaxLength(100)]
        public string? Departamenti { get; set; }

        [MaxLength(100)]
        public string? Pozicioni { get; set; }

        public int? Dega_Id { get; set; }

        [ForeignKey("Dega_Id")]
        public virtual Dega? Dega { get; set; }

        [MaxLength(20)]
        public string? KodiZyrtarit { get; set; }

        public DateTime DataKrijimit { get; set; } = DateTime.UtcNow;

        public DateTime? HyrjaEFundit { get; set; }

        public bool EshteAktiv { get; set; } = true;
    }

    public static class RoletSistemit
    {
        public const string Admin = "Admin";

        public const string Zyrtar = "Zyrtar";

        public const string Shikues = "Shikues";

        public static readonly string[] TeGjithaRolet = { Admin, Zyrtar, Shikues };
    }
}
