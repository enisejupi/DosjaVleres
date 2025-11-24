using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Te_dhenat
{
    public class AplikacioniDbKonteksti : IdentityDbContext<Perdoruesi>
    {
        public AplikacioniDbKonteksti(DbContextOptions<AplikacioniDbKonteksti> options)
            : base(options)
        {
        }

        public DbSet<VleraProduktit> VleratProdukteve { get; set; }

        public DbSet<HistoriaVlerave> HistoriaVlerave { get; set; }

        public DbSet<RegjistriAuditimit> RegjistriAuditimit { get; set; }

        public DbSet<SesioniPerdoruesit> SesionetPerdoruesve { get; set; }

        public DbSet<Komenti> Komentet { get; set; }

        public DbSet<Dega> Deget { get; set; }

        public DbSet<ShpenzimiTransportit> ShpenzimetTransportit { get; set; }

        public DbSet<NdryshimiTransportit> NdryshimetTransportit { get; set; }

        public DbSet<KomentiDeges> KomentetDegeve { get; set; }

        public DbSet<KerkeseRegjistrim> KerkesatRegjistrim { get; set; }

        public DbSet<PreferencatPerdoruesit> PreferencatPerdoruesve { get; set; }

        public DbSet<TabelaCustom> TabelatCustom { get; set; }

        public DbSet<FormatPrintimi> FormatetiPrintimit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurimi i VleraProduktit
            modelBuilder.Entity<VleraProduktit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Kodi);
                entity.HasIndex(e => e.Kategoria);
                entity.HasIndex(e => e.Eshte_Aktiv);
                entity.HasIndex(e => e.Origjina);

                // Marrëdhënia një-me-shumë me HistoriaVlerave
                entity.HasMany(p => p.HistoriaVlerave)
                      .WithOne(h => h.VleraProduktit)
                      .HasForeignKey(h => h.VleraProduktit_Id)
                      .OnDelete(DeleteBehavior.Cascade);

                // Marrëdhënia një-me-shumë me Komentet
                entity.HasMany(p => p.KomentList)
                      .WithOne(k => k.VleraProduktit)
                      .HasForeignKey(k => k.VleraProduktitId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Konfigurimi i Komenti
            modelBuilder.Entity<Komenti>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.VleraProduktitId);
                entity.HasIndex(e => e.Krijuar_Me);
            });

            // Konfigurimi i HistoriaVlerave
            modelBuilder.Entity<HistoriaVlerave>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.VleraProduktit_Id);
                entity.HasIndex(e => e.Ndryshuar_Me);
                entity.HasIndex(e => e.Ndryshuar_Nga);
            });

            // Konfigurimi i RegjistriAuditimit
            modelBuilder.Entity<RegjistriAuditimit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Perdoruesi);
                entity.HasIndex(e => e.LlojiVeprimit);
                entity.HasIndex(e => e.Entiteti);
                entity.HasIndex(e => e.Koha);
                entity.HasIndex(e => e.Sesioni_Id);
            });

            // Konfigurimi i SesioniPerdoruesit
            modelBuilder.Entity<SesioniPerdoruesit>(entity =>
            {
                entity.HasKey(e => e.Sesioni_Id);
                entity.HasIndex(e => e.Perdoruesi);
                entity.HasIndex(e => e.Filluar_Me);
                entity.HasIndex(e => e.Eshte_Aktiv);
            });

            // Konfigurimi i Dega
            modelBuilder.Entity<Dega>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.KodiDeges).IsUnique();
                entity.HasIndex(e => e.EmriDeges);
                entity.HasIndex(e => e.Eshte_Aktiv);

                entity.HasMany(d => d.Perdoruesit)
                      .WithOne(p => p.Dega)
                      .HasForeignKey(p => p.Dega_Id)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Konfigurimi i ShpenzimiTransportit
            modelBuilder.Entity<ShpenzimiTransportit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.VendiOrigjines);
                entity.HasIndex(e => e.VendiDestinacionit);
                entity.HasIndex(e => e.LlojiTransportit);
                entity.HasIndex(e => e.Aktiv);

                entity.HasMany(s => s.Ndryshimet)
                      .WithOne(n => n.ShpenzimiTransportit)
                      .HasForeignKey(n => n.ShpenzimiTransportit_Id)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Konfigurimi i NdryshimiTransportit
            modelBuilder.Entity<NdryshimiTransportit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ShpenzimiTransportit_Id);
                entity.HasIndex(e => e.Ndryshuar_Me);
            });

            // Konfigurimi i KomentiDeges
            modelBuilder.Entity<KomentiDeges>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EmriDeges);
                entity.HasIndex(e => e.KodiTarifar);
                entity.HasIndex(e => e.DataDergimit);
                entity.HasIndex(e => e.EshteLexuar);
                entity.HasIndex(e => e.EshteZgjidhur);

                entity.HasOne(k => k.VleraProduktit)
                      .WithMany()
                      .HasForeignKey(k => k.VleraProduktit_Id)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Të dhënat fillestare për testim
            ShtoniTeDhenatFillestare(modelBuilder);
        }

        private void ShtoniTeDhenatFillestare(ModelBuilder modelBuilder)
        {
            // Produktet reale të Doganës së Kosovës
            var dataKrijimit = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            modelBuilder.Entity<VleraProduktit>().HasData(
                new VleraProduktit
                {
                    Id = 1,
                    Kodi = "7615",
                    KodiProduktit = "7615",
                    EmriProduktit = "Tepsi alumini",
                    Pershkrimi = "Tepsi alumini 0.8mm me dim: 30cm ; 32cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "1.20 ; 1.45",
                    VleraDoganore = 1.20m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 2,
                    Kodi = "0102",
                    KodiProduktit = "0102-01",
                    EmriProduktit = "Viça",
                    Pershkrimi = "Viça (gjedhet femra që nuk kanë pjellë, mshqerra)",
                    Origjina = "TE NDRYSHME",
                    Pariteti = "DAT",
                    Njesia = "KG",
                    Cmimi = "2.70",
                    VleraDoganore = 2.70m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 3,
                    Kodi = "0102",
                    KodiProduktit = "0102-02",
                    EmriProduktit = "Dema",
                    Pershkrimi = "Dema per therje me peshe mbi 160 kg",
                    Origjina = "TE NDRYSHME",
                    Pariteti = "DAT",
                    Njesia = "KG",
                    Cmimi = "2.40",
                    VleraDoganore = 2.40m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 4,
                    Kodi = "0102",
                    KodiProduktit = "0102-03",
                    EmriProduktit = "Lopet",
                    Pershkrimi = "Lopet per therje",
                    Origjina = "TE NDRYSHME",
                    Pariteti = "DAT",
                    Njesia = "KG",
                    Cmimi = "1.50",
                    VleraDoganore = 1.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 5,
                    Kodi = "7323",
                    KodiProduktit = "7323-01",
                    EmriProduktit = "Tepsi e zezë",
                    Pershkrimi = "Tepsi e zezë e emajluar me dim:40 cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "2.50",
                    VleraDoganore = 2.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 6,
                    Kodi = "7323",
                    KodiProduktit = "7323-02",
                    EmriProduktit = "Tepsi e zezë me lule",
                    Pershkrimi = "Tepsi e zezë e emajluar(me lule) dim: 40cm ; 42 cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "2.50 ; 3.20",
                    VleraDoganore = 2.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 7,
                    Kodi = "7323",
                    KodiProduktit = "7323-03",
                    EmriProduktit = "Tepsi e zezë emajluara",
                    Pershkrimi = "Tepsi e zezë e emajluara dim :34 cm;36 cm;38cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "1.50 ; 1.85 ; 2.30",
                    VleraDoganore = 1.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 8,
                    Kodi = "7323",
                    KodiProduktit = "7323-04",
                    EmriProduktit = "Tepsi teflon",
                    Pershkrimi = "Tepsi teflon me dim: 30cm;32cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "3.00 ; 3.50",
                    VleraDoganore = 3.00m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 9,
                    Kodi = "7323",
                    KodiProduktit = "7323-05",
                    EmriProduktit = "Tepsi teflon",
                    Pershkrimi = "Tepsi teflon me dim: 34cm ;36cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "3.80 ; 4.80",
                    VleraDoganore = 3.80m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 10,
                    Kodi = "7323",
                    KodiProduktit = "7323-06",
                    EmriProduktit = "Tepsi metalike",
                    Pershkrimi = "Tepsi metalike 1/3 (28cm;32cm;36cm)",
                    Origjina = "CN",
                    Pariteti = "EXW",
                    Njesia = "Kompleti",
                    Cmimi = "2.70",
                    VleraDoganore = 2.70m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 11,
                    Kodi = "7323",
                    KodiProduktit = "7323-07",
                    EmriProduktit = "Tepsi teflon",
                    Pershkrimi = "Tepsi teflon me dim: 38cm; 40cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "4.50 ;5.00",
                    VleraDoganore = 4.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 12,
                    Kodi = "7323",
                    KodiProduktit = "7323-08",
                    EmriProduktit = "Tepsi e zezë emajluar",
                    Pershkrimi = "Tepsi e zezë e emajluar me dim. 34 cm ; 36 cm ; 38cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "1.50 ; 1.60 ; 1.85",
                    VleraDoganore = 1.50m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 13,
                    Kodi = "7323",
                    KodiProduktit = "7323-09",
                    EmriProduktit = "Tepsi e zezë emajluar",
                    Pershkrimi = "Tepsi e zezë e emajluar dim;28cm ;30cm ; 32cm",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "Copë",
                    Cmimi = "1.20 ; 1.30 ; 1.50",
                    VleraDoganore = 1.20m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 14,
                    Kodi = "0402",
                    KodiProduktit = "0402-01",
                    EmriProduktit = "QUMESHT PLUHUR A",
                    Pershkrimi = "QUMESHT PLUHUR A / DRY-MILK-PW-NED",
                    Origjina = "FR",
                    Pariteti = "EXW",
                    Njesia = "TON",
                    Cmimi = "",
                    VleraDoganore = 0m,
                    Valuta = "EUR",
                    Kategoria = "Çmimi nga Bursa",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 15,
                    Kodi = "0402",
                    KodiProduktit = "0402-02",
                    EmriProduktit = "QUMESHT PLUHUR H",
                    Pershkrimi = "QUMESHT PLUHUR H / DRY-MILK-PW-EDI",
                    Origjina = "FR",
                    Pariteti = "EXW",
                    Njesia = "TON",
                    Cmimi = "",
                    VleraDoganore = 0m,
                    Valuta = "EUR",
                    Kategoria = "Çmimi nga Bursa",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 16,
                    Kodi = "0701",
                    KodiProduktit = "0701",
                    EmriProduktit = "Patate",
                    Pershkrimi = "Patate te reja",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "KG",
                    Cmimi = "0.45",
                    VleraDoganore = 0.45m,
                    Valuta = "EUR",
                    Kategoria = "Pemë-Perime",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 17,
                    Kodi = "5607",
                    KodiProduktit = "5607",
                    EmriProduktit = "Litar-spango",
                    Pershkrimi = "Litar-spango",
                    Origjina = "CN",
                    Pariteti = "EXW",
                    Njesia = "KG",
                    Cmimi = "1.25",
                    VleraDoganore = 1.25m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 18,
                    Kodi = "6005",
                    KodiProduktit = "6005-01",
                    EmriProduktit = "Perde sintetike",
                    Pershkrimi = "Perde sintetike te firmave te njohura",
                    Origjina = "CN",
                    Pariteti = "EXW",
                    Njesia = "KG",
                    Cmimi = "7.20",
                    VleraDoganore = 7.20m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 19,
                    Kodi = "6005",
                    KodiProduktit = "6005-02",
                    EmriProduktit = "Perde sintetike",
                    Pershkrimi = "Perde sintetike te firmave te njohura",
                    Origjina = "TR",
                    Pariteti = "DAT",
                    Njesia = "KG",
                    Cmimi = "12.00",
                    VleraDoganore = 12.00m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                },
                new VleraProduktit
                {
                    Id = 20,
                    Kodi = "6601",
                    KodiProduktit = "6601",
                    EmriProduktit = "Cadra shiu",
                    Pershkrimi = "Cadra shiu",
                    Origjina = "CN",
                    Pariteti = "EXW",
                    Njesia = "Copë",
                    Cmimi = "0.35",
                    VleraDoganore = 0.35m,
                    Valuta = "EUR",
                    Kategoria = "Tjera",
                    Eshte_Aktiv = true,
                    Krijuar_Me = dataKrijimit,
                    Krijuar_Nga = "Admin"
                }
            );
        }
    }
}
