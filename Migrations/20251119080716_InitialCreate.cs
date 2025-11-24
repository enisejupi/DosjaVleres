using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KosovaDoganaModerne.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KodiDeges = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EmriDeges = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Qyteti = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Adresa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Telefoni = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Eshte_Aktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    Krijuar_Me = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Krijuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KerkesatRegjistrim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmriPlote = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Departamenti = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Pozicioni = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    KodiZyrtarit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Dega_Id = table.Column<int>(type: "INTEGER", nullable: true),
                    EmriDegës = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataKerkeses = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statusi = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataShqyrtimit = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShqyrtuesId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ArsetimiRefuzimit = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ADUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EshteVerifikuarNeAD = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KerkesatRegjistrim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegjistriAuditimit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Perdoruesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LlojiVeprimit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Entiteti = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Entiteti_Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Detajet = table.Column<string>(type: "TEXT", nullable: true),
                    Vlerat_Vjetra = table.Column<string>(type: "TEXT", nullable: true),
                    Vlerat_Reja = table.Column<string>(type: "TEXT", nullable: true),
                    Koha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdresaIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Eshte_Suksesshem = table.Column<bool>(type: "INTEGER", nullable: false),
                    MesazhiGabimit = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Sesioni_Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegjistriAuditimit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SesionetPerdoruesve",
                columns: table => new
                {
                    Sesioni_Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Perdoruesi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Filluar_Me = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Perfunduar_Me = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Eshte_Aktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdresaIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Kohezgjatja_Minuta = table.Column<int>(type: "INTEGER", nullable: true),
                    AktivitetiI_Fundit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumriVeprimeve = table.Column<int>(type: "INTEGER", nullable: false),
                    ArsyejaPerfundimit = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionetPerdoruesve", x => x.Sesioni_Id);
                });

            migrationBuilder.CreateTable(
                name: "ShpenzimetTransportit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendiOrigjines = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VendiDestinacionit = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LlojiTransportit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CmimiPerNjesi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valuta = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    NjesiaMatese = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Shenime = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Aktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataKrijimit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KrijuarNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataPerditesimit = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PerditesoPrejNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShpenzimetTransportit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VleratProdukteve",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KodiProduktit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EmriProduktit = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Kodi = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Pershkrimi = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Origjina = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Pariteti = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Njesia = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Cmimi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    VleraDoganore = table.Column<decimal>(type: "TEXT", nullable: false),
                    Valuta = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Kategoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Komentet = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Eshte_Aktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    Krijuar_Me = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Krijuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Modifikuar_Me = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Modifikuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VleratProdukteve", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    EmriPlote = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Departamenti = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Pozicioni = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Dega_Id = table.Column<int>(type: "INTEGER", nullable: true),
                    KodiZyrtarit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DataKrijimit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HyrjaEFundit = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EshteAktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Deget_Dega_Id",
                        column: x => x.Dega_Id,
                        principalTable: "Deget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NdryshimetTransportit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShpenzimiTransportit_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Cmimi_Mepar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cmimi_Ri = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valuta_Mepar = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Valuta_Re = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ArsyejaE_Ndryshimit = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Ndryshuar_Me = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ndryshuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AdresaIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NumriVersionit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdryshimetTransportit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdryshimetTransportit_ShpenzimetTransportit_ShpenzimiTransportit_Id",
                        column: x => x.ShpenzimiTransportit_Id,
                        principalTable: "ShpenzimetTransportit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriaVlerave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VleraProduktit_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Vlera_Mepar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vlera_Re = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valuta_Mepar = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    Valuta_Re = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    ArsyejaE_Ndryshimit = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Ndryshuar_Me = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ndryshuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AdresaIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NumriVersionit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriaVlerave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriaVlerave_VleratProdukteve_VleraProduktit_Id",
                        column: x => x.VleraProduktit_Id,
                        principalTable: "VleratProdukteve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Komentet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VleraProduktitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Permbajtja = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Krijuar_Nga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Krijuar_Me = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komentet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Komentet_VleratProdukteve_VleraProduktitId",
                        column: x => x.VleraProduktitId,
                        principalTable: "VleratProdukteve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KomentetDegeve",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmriDeges = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    KodiTarifar = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Mesazhi = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DataDergimit = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DergoPrejNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EshteLexuar = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataLeximit = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LexoNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EshteZgjidhur = table.Column<bool>(type: "INTEGER", nullable: false),
                    Pergjigja = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DataPergjigjes = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PergjigjetNga = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    VleraProduktit_Id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KomentetDegeve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KomentetDegeve_VleratProdukteve_VleraProduktit_Id",
                        column: x => x.VleraProduktit_Id,
                        principalTable: "VleratProdukteve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "VleratProdukteve",
                columns: new[] { "Id", "Cmimi", "EmriProduktit", "Eshte_Aktiv", "Kategoria", "Kodi", "KodiProduktit", "Komentet", "Krijuar_Me", "Krijuar_Nga", "Modifikuar_Me", "Modifikuar_Nga", "Njesia", "Origjina", "Pariteti", "Pershkrimi", "Valuta", "VleraDoganore" },
                values: new object[,]
                {
                    { 1, "1.20 ; 1.45", "Tepsi alumini", true, "Tjera", "7615", "7615", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi alumini 0.8mm me dim: 30cm ; 32cm", "EUR", 1.20m },
                    { 2, "2.70", "Viça", true, "Tjera", "0102", "0102-01", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "TE NDRYSHME", "DAT", "Viça (gjedhet femra që nuk kanë pjellë, mshqerra)", "EUR", 2.70m },
                    { 3, "2.40", "Dema", true, "Tjera", "0102", "0102-02", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "TE NDRYSHME", "DAT", "Dema per therje me peshe mbi 160 kg", "EUR", 2.40m },
                    { 4, "1.50", "Lopet", true, "Tjera", "0102", "0102-03", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "TE NDRYSHME", "DAT", "Lopet per therje", "EUR", 1.50m },
                    { 5, "2.50", "Tepsi e zezë", true, "Tjera", "7323", "7323-01", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi e zezë e emajluar me dim:40 cm", "EUR", 2.50m },
                    { 6, "2.50 ; 3.20", "Tepsi e zezë me lule", true, "Tjera", "7323", "7323-02", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi e zezë e emajluar(me lule) dim: 40cm ; 42 cm", "EUR", 2.50m },
                    { 7, "1.50 ; 1.85 ; 2.30", "Tepsi e zezë emajluara", true, "Tjera", "7323", "7323-03", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi e zezë e emajluara dim :34 cm;36 cm;38cm", "EUR", 1.50m },
                    { 8, "3.00 ; 3.50", "Tepsi teflon", true, "Tjera", "7323", "7323-04", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi teflon me dim: 30cm;32cm", "EUR", 3.00m },
                    { 9, "3.80 ; 4.80", "Tepsi teflon", true, "Tjera", "7323", "7323-05", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi teflon me dim: 34cm ;36cm", "EUR", 3.80m },
                    { 10, "2.70", "Tepsi metalike", true, "Tjera", "7323", "7323-06", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Kompleti", "CN", "EXW", "Tepsi metalike 1/3 (28cm;32cm;36cm)", "EUR", 2.70m },
                    { 11, "4.50 ;5.00", "Tepsi teflon", true, "Tjera", "7323", "7323-07", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi teflon me dim: 38cm; 40cm", "EUR", 4.50m },
                    { 12, "1.50 ; 1.60 ; 1.85", "Tepsi e zezë emajluar", true, "Tjera", "7323", "7323-08", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi e zezë e emajluar me dim. 34 cm ; 36 cm ; 38cm", "EUR", 1.50m },
                    { 13, "1.20 ; 1.30 ; 1.50", "Tepsi e zezë emajluar", true, "Tjera", "7323", "7323-09", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "TR", "DAT", "Tepsi e zezë e emajluar dim;28cm ;30cm ; 32cm", "EUR", 1.20m },
                    { 14, "", "QUMESHT PLUHUR A", true, "Çmimi nga Bursa", "0402", "0402-01", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "TON", "FR", "EXW", "QUMESHT PLUHUR A / DRY-MILK-PW-NED", "EUR", 0m },
                    { 15, "", "QUMESHT PLUHUR H", true, "Çmimi nga Bursa", "0402", "0402-02", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "TON", "FR", "EXW", "QUMESHT PLUHUR H / DRY-MILK-PW-EDI", "EUR", 0m },
                    { 16, "0.45", "Patate", true, "Pemë-Perime", "0701", "0701", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "TR", "DAT", "Patate te reja", "EUR", 0.45m },
                    { 17, "1.25", "Litar-spango", true, "Tjera", "5607", "5607", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "CN", "EXW", "Litar-spango", "EUR", 1.25m },
                    { 18, "7.20", "Perde sintetike", true, "Tjera", "6005", "6005-01", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "CN", "EXW", "Perde sintetike te firmave te njohura", "EUR", 7.20m },
                    { 19, "12.00", "Perde sintetike", true, "Tjera", "6005", "6005-02", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "KG", "TR", "DAT", "Perde sintetike te firmave te njohura", "EUR", 12.00m },
                    { 20, "0.35", "Cadra shiu", true, "Tjera", "6601", "6601", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", null, null, "Copë", "CN", "EXW", "Cadra shiu", "EUR", 0.35m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Dega_Id",
                table: "AspNetUsers",
                column: "Dega_Id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deget_EmriDeges",
                table: "Deget",
                column: "EmriDeges");

            migrationBuilder.CreateIndex(
                name: "IX_Deget_Eshte_Aktiv",
                table: "Deget",
                column: "Eshte_Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_Deget_KodiDeges",
                table: "Deget",
                column: "KodiDeges",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaVlerave_Ndryshuar_Me",
                table: "HistoriaVlerave",
                column: "Ndryshuar_Me");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaVlerave_Ndryshuar_Nga",
                table: "HistoriaVlerave",
                column: "Ndryshuar_Nga");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaVlerave_VleraProduktit_Id",
                table: "HistoriaVlerave",
                column: "VleraProduktit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Komentet_Krijuar_Me",
                table: "Komentet",
                column: "Krijuar_Me");

            migrationBuilder.CreateIndex(
                name: "IX_Komentet_VleraProduktitId",
                table: "Komentet",
                column: "VleraProduktitId");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_DataDergimit",
                table: "KomentetDegeve",
                column: "DataDergimit");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_EmriDeges",
                table: "KomentetDegeve",
                column: "EmriDeges");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_EshteLexuar",
                table: "KomentetDegeve",
                column: "EshteLexuar");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_EshteZgjidhur",
                table: "KomentetDegeve",
                column: "EshteZgjidhur");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_KodiTarifar",
                table: "KomentetDegeve",
                column: "KodiTarifar");

            migrationBuilder.CreateIndex(
                name: "IX_KomentetDegeve_VleraProduktit_Id",
                table: "KomentetDegeve",
                column: "VleraProduktit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_NdryshimetTransportit_Ndryshuar_Me",
                table: "NdryshimetTransportit",
                column: "Ndryshuar_Me");

            migrationBuilder.CreateIndex(
                name: "IX_NdryshimetTransportit_ShpenzimiTransportit_Id",
                table: "NdryshimetTransportit",
                column: "ShpenzimiTransportit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RegjistriAuditimit_Entiteti",
                table: "RegjistriAuditimit",
                column: "Entiteti");

            migrationBuilder.CreateIndex(
                name: "IX_RegjistriAuditimit_Koha",
                table: "RegjistriAuditimit",
                column: "Koha");

            migrationBuilder.CreateIndex(
                name: "IX_RegjistriAuditimit_LlojiVeprimit",
                table: "RegjistriAuditimit",
                column: "LlojiVeprimit");

            migrationBuilder.CreateIndex(
                name: "IX_RegjistriAuditimit_Perdoruesi",
                table: "RegjistriAuditimit",
                column: "Perdoruesi");

            migrationBuilder.CreateIndex(
                name: "IX_RegjistriAuditimit_Sesioni_Id",
                table: "RegjistriAuditimit",
                column: "Sesioni_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SesionetPerdoruesve_Eshte_Aktiv",
                table: "SesionetPerdoruesve",
                column: "Eshte_Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_SesionetPerdoruesve_Filluar_Me",
                table: "SesionetPerdoruesve",
                column: "Filluar_Me");

            migrationBuilder.CreateIndex(
                name: "IX_SesionetPerdoruesve_Perdoruesi",
                table: "SesionetPerdoruesve",
                column: "Perdoruesi");

            migrationBuilder.CreateIndex(
                name: "IX_ShpenzimetTransportit_Aktiv",
                table: "ShpenzimetTransportit",
                column: "Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_ShpenzimetTransportit_LlojiTransportit",
                table: "ShpenzimetTransportit",
                column: "LlojiTransportit");

            migrationBuilder.CreateIndex(
                name: "IX_ShpenzimetTransportit_VendiDestinacionit",
                table: "ShpenzimetTransportit",
                column: "VendiDestinacionit");

            migrationBuilder.CreateIndex(
                name: "IX_ShpenzimetTransportit_VendiOrigjines",
                table: "ShpenzimetTransportit",
                column: "VendiOrigjines");

            migrationBuilder.CreateIndex(
                name: "IX_VleratProdukteve_Eshte_Aktiv",
                table: "VleratProdukteve",
                column: "Eshte_Aktiv");

            migrationBuilder.CreateIndex(
                name: "IX_VleratProdukteve_Kategoria",
                table: "VleratProdukteve",
                column: "Kategoria");

            migrationBuilder.CreateIndex(
                name: "IX_VleratProdukteve_Kodi",
                table: "VleratProdukteve",
                column: "Kodi");

            migrationBuilder.CreateIndex(
                name: "IX_VleratProdukteve_Origjina",
                table: "VleratProdukteve",
                column: "Origjina");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "HistoriaVlerave");

            migrationBuilder.DropTable(
                name: "KerkesatRegjistrim");

            migrationBuilder.DropTable(
                name: "Komentet");

            migrationBuilder.DropTable(
                name: "KomentetDegeve");

            migrationBuilder.DropTable(
                name: "NdryshimetTransportit");

            migrationBuilder.DropTable(
                name: "RegjistriAuditimit");

            migrationBuilder.DropTable(
                name: "SesionetPerdoruesve");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "VleratProdukteve");

            migrationBuilder.DropTable(
                name: "ShpenzimetTransportit");

            migrationBuilder.DropTable(
                name: "Deget");
        }
    }
}
