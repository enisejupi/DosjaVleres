using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;
using Microsoft.EntityFrameworkCore;

namespace KosovaDoganaModerne.Te_dhenat
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AplikacioniDbKonteksti context)
        {
            if (!await context.Deget.AnyAsync())
            {
                var deget = new List<Dega>
                {
                    new Dega { KodiDeges = "PR-01", EmriDeges = "PRISHTINA - KRYESORJA", Qyteti = "Prishtinë", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "PR-02", EmriDeges = "PRISHTINA - TERMINALI", Qyteti = "Prishtinë", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "MT-01", EmriDeges = "MITROVICA - TERMINALI", Qyteti = "Mitrovicë", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "PZ-01", EmriDeges = "PRIZREN", Qyteti = "Prizren", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "GJ-01", EmriDeges = "GJAKOVA", Qyteti = "Gjakovë", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "PE-01", EmriDeges = "PEJA", Qyteti = "Pejë", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "GI-01", EmriDeges = "GJILAN", Qyteti = "Gjilan", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" },
                    new Dega { KodiDeges = "FE-01", EmriDeges = "FERIZAJ", Qyteti = "Ferizaj", Eshte_Aktiv = true, Krijuar_Me = DateTime.UtcNow, Krijuar_Nga = "System" }
                };
                await context.Deget.AddRangeAsync(deget);
                await context.SaveChangesAsync();
                Console.WriteLine($" ✓ U shtuan {deget.Count} degë");
            }

            if (!await context.ShpenzimetTransportit.AnyAsync())
            {
                var shpenzimet = new List<ShpenzimiTransportit>
                {
                    new ShpenzimiTransportit { VendiOrigjines = "Shqipëri", VendiDestinacionit = "Kosovë", LlojiTransportit = "Tokësor", CmimiPerNjesi = 0.50m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Turqi", VendiDestinacionit = "Kosovë", LlojiTransportit = "Ajror", CmimiPerNjesi = 2.50m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Gjermani", VendiDestinacionit = "Kosovë", LlojiTransportit = "Hekurudhor", CmimiPerNjesi = 0.80m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Itali", VendiDestinacionit = "Kosovë", LlojiTransportit = "Hekurudhor", CmimiPerNjesi = 0.75m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Slloveni", VendiDestinacionit = "Kosovë", LlojiTransportit = "Hekurudhor", CmimiPerNjesi = 0.65m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Kroaci", VendiDestinacionit = "Kosovë", LlojiTransportit = "Detar", CmimiPerNjesi = 0.40m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Maqedoni", VendiDestinacionit = "Kosovë", LlojiTransportit = "Tokësor", CmimiPerNjesi = 0.35m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" },
                    new ShpenzimiTransportit { VendiOrigjines = "Serbi", VendiDestinacionit = "Kosovë", LlojiTransportit = "Tokësor", CmimiPerNjesi = 0.30m, Valuta = "EUR", NjesiaMatese = "kg", Aktiv = true, DataKrijimit = DateTime.UtcNow, KrijuarNga = "Sistem" }
                };
                await context.ShpenzimetTransportit.AddRangeAsync(shpenzimet);
                await context.SaveChangesAsync();
                Console.WriteLine($" ✓ U shtuan {shpenzimet.Count} shpenzime transporti");
            }
        }
    }
}
