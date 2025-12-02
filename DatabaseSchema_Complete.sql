-- =====================================================================
-- KOSOVO CUSTOMS - DATABASE SCHEMA EXPORT
-- Dosja e Vlerave (Value File Application)
-- Generated: $(Get-Date)
-- Database: SQLite (KosovaDoganaModerne.db)
-- =====================================================================

-- This script can be used to recreate the database structure
-- Run this script on a clean SQLite or SQL Server database

-- =====================================================================
-- TABLE 1: AspNetRoles (User Roles)
-- =====================================================================
CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    NormalizedName TEXT UNIQUE,
    ConcurrencyStamp TEXT
);

-- =====================================================================
-- TABLE 2: Deget (Branch Offices)
-- =====================================================================
CREATE TABLE IF NOT EXISTS Deget (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    KodiDeges TEXT NOT NULL UNIQUE,
    EmriDeges TEXT NOT NULL,
    Adresa TEXT,
    NrTelefonit TEXT,
    EmailiKontaktit TEXT,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1,
    Krijuar_Me TEXT NOT NULL,
    Krijuar_Nga TEXT,
    Ndryshuar_Me TEXT,
    Ndryshuar_Nga TEXT
);

CREATE INDEX IF NOT EXISTS IX_Deget_KodiDeges ON Deget(KodiDeges);
CREATE INDEX IF NOT EXISTS IX_Deget_EmriDeges ON Deget(EmriDeges);
CREATE INDEX IF NOT EXISTS IX_Deget_Eshte_Aktiv ON Deget(Eshte_Aktiv);

-- =====================================================================
-- TABLE 3: AspNetUsers (Users)
-- =====================================================================
CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id TEXT PRIMARY KEY,
    UserName TEXT NOT NULL,
    NormalizedUserName TEXT UNIQUE,
    Email TEXT,
    NormalizedEmail TEXT,
    EmailConfirmed INTEGER NOT NULL DEFAULT 0,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber TEXT,
    PhoneNumberConfirmed INTEGER NOT NULL DEFAULT 0,
    TwoFactorEnabled INTEGER NOT NULL DEFAULT 0,
    LockoutEnd TEXT,
    LockoutEnabled INTEGER NOT NULL DEFAULT 1,
    AccessFailedCount INTEGER NOT NULL DEFAULT 0,
    -- Extended properties
    EmriPlote TEXT,
    RoliSystemit TEXT,
    Dega_Id INTEGER,
    Departamenti TEXT,
    Pozita TEXT,
    DataRegjistrit TEXT,
    DataFunditKyqjes TEXT,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (Dega_Id) REFERENCES Deget(Id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS IX_AspNetUsers_Dega_Id ON AspNetUsers(Dega_Id);
CREATE INDEX IF NOT EXISTS IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);
CREATE INDEX IF NOT EXISTS IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);

-- =====================================================================
-- TABLE 4: VleratProdukteve (Product Values - Main Table)
-- =====================================================================
CREATE TABLE IF NOT EXISTS VleratProdukteve (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Kodi TEXT NOT NULL,
    KodiProduktit TEXT NOT NULL,
    EmriProduktit TEXT NOT NULL,
    Pershkrimi TEXT,
    Origjina TEXT,
    Pariteti TEXT,
    Njesia TEXT,
    Cmimi TEXT,
    VleraDoganore REAL NOT NULL DEFAULT 0,
    Valuta TEXT DEFAULT 'EUR',
    Kategoria TEXT,
    Burimi_Informacionit TEXT,
    DataPublikimit TEXT,
    DataSkadimit TEXT,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1,
    Krijuar_Me TEXT NOT NULL,
    Krijuar_Nga TEXT,
    Ndryshuar_Me TEXT,
    Ndryshuar_Nga TEXT,
    Shenim TEXT,
    FotoPath TEXT
);

CREATE INDEX IF NOT EXISTS IX_VleratProdukteve_Kodi ON VleratProdukteve(Kodi);
CREATE INDEX IF NOT EXISTS IX_VleratProdukteve_Kategoria ON VleratProdukteve(Kategoria);
CREATE INDEX IF NOT EXISTS IX_VleratProdukteve_Eshte_Aktiv ON VleratProdukteve(Eshte_Aktiv);
CREATE INDEX IF NOT EXISTS IX_VleratProdukteve_Origjina ON VleratProdukteve(Origjina);

-- =====================================================================
-- TABLE 5: HistoriaVlerave (Value History - Version Control)
-- =====================================================================
CREATE TABLE IF NOT EXISTS HistoriaVlerave (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    VleraProduktit_Id INTEGER NOT NULL,
    Vlera_Para TEXT,
    Vlera_Pas TEXT,
    Fusha_Ndryshuar TEXT,
    Arsyeja_Ndryshimit TEXT,
    Ndryshuar_Me TEXT NOT NULL,
    Ndryshuar_Nga TEXT NOT NULL,
    Sesioni_Id TEXT,
    AdresaIP TEXT,
    Photo TEXT,
    FOREIGN KEY (VleraProduktit_Id) REFERENCES VleratProdukteve(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_HistoriaVlerave_VleraProduktit_Id ON HistoriaVlerave(VleraProduktit_Id);
CREATE INDEX IF NOT EXISTS IX_HistoriaVlerave_Ndryshuar_Me ON HistoriaVlerave(Ndryshuar_Me);
CREATE INDEX IF NOT EXISTS IX_HistoriaVlerave_Ndryshuar_Nga ON HistoriaVlerave(Ndryshuar_Nga);

-- =====================================================================
-- TABLE 6: Komentet (Product Comments)
-- =====================================================================
CREATE TABLE IF NOT EXISTS Komentet (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    VleraProduktitId INTEGER NOT NULL,
    Teksti TEXT NOT NULL,
    EmriKomentuesit TEXT,
    EmailiKomentuesit TEXT,
    Krijuar_Me TEXT NOT NULL,
    Eshte_Miratuar INTEGER NOT NULL DEFAULT 0,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (VleraProduktitId) REFERENCES VleratProdukteve(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Komentet_VleraProduktitId ON Komentet(VleraProduktitId);
CREATE INDEX IF NOT EXISTS IX_Komentet_Krijuar_Me ON Komentet(Krijuar_Me);

-- =====================================================================
-- TABLE 7: ShpenzimetTransportit (Transport Costs)
-- =====================================================================
CREATE TABLE IF NOT EXISTS ShpenzimetTransportit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    VendiOrigjines TEXT NOT NULL,
    VendiDestinacionit TEXT NOT NULL,
    LlojiTransportit TEXT,
    Distanca REAL,
    NjesiaDistances TEXT,
    Cmimi REAL NOT NULL,
    Valuta TEXT DEFAULT 'EUR',
    DataFillimit TEXT,
    DataMbarimit TEXT,
    Aktiv INTEGER NOT NULL DEFAULT 1,
    Krijuar_Me TEXT NOT NULL,
    Krijuar_Nga TEXT,
    Ndryshuar_Me TEXT,
    Ndryshuar_Nga TEXT,
    Shenim TEXT
);

CREATE INDEX IF NOT EXISTS IX_ShpenzimetTransportit_VendiOrigjines ON ShpenzimetTransportit(VendiOrigjines);
CREATE INDEX IF NOT EXISTS IX_ShpenzimetTransportit_VendiDestinacionit ON ShpenzimetTransportit(VendiDestinacionit);
CREATE INDEX IF NOT EXISTS IX_ShpenzimetTransportit_LlojiTransportit ON ShpenzimetTransportit(LlojiTransportit);
CREATE INDEX IF NOT EXISTS IX_ShpenzimetTransportit_Aktiv ON ShpenzimetTransportit(Aktiv);

-- =====================================================================
-- TABLE 8: NdryshimetTransportit (Transport Change History)
-- =====================================================================
CREATE TABLE IF NOT EXISTS NdryshimetTransportit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShpenzimiTransportit_Id INTEGER NOT NULL,
    Vlera_Para TEXT,
    Vlera_Pas TEXT,
    Fusha_Ndryshuar TEXT,
    Arsyeja_Ndryshimit TEXT,
    Ndryshuar_Me TEXT NOT NULL,
    Ndryshuar_Nga TEXT NOT NULL,
    FOREIGN KEY (ShpenzimiTransportit_Id) REFERENCES ShpenzimetTransportit(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_NdryshimetTransportit_ShpenzimiTransportit_Id ON NdryshimetTransportit(ShpenzimiTransportit_Id);
CREATE INDEX IF NOT EXISTS IX_NdryshimetTransportit_Ndryshuar_Me ON NdryshimetTransportit(Ndryshuar_Me);

-- =====================================================================
-- TABLE 9: KomentetDegeve (Branch Office Comments)
-- =====================================================================
CREATE TABLE IF NOT EXISTS KomentetDegeve (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmriDeges TEXT NOT NULL,
    KodiTarifar TEXT NOT NULL,
    Teksti TEXT NOT NULL,
    VleraProduktit_Id INTEGER,
    DataDergimit TEXT NOT NULL,
    DerguesEmri TEXT,
    DerguesEmail TEXT,
    DerguesTelefoni TEXT,
    EshteLexuar INTEGER NOT NULL DEFAULT 0,
    EshteZgjidhur INTEGER NOT NULL DEFAULT 0,
    PergjigjaAdministratorit TEXT,
    DataPergjigjes TEXT,
    FOREIGN KEY (VleraProduktit_Id) REFERENCES VleratProdukteve(Id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS IX_KomentetDegeve_EmriDeges ON KomentetDegeve(EmriDeges);
CREATE INDEX IF NOT EXISTS IX_KomentetDegeve_KodiTarifar ON KomentetDegeve(KodiTarifar);
CREATE INDEX IF NOT EXISTS IX_KomentetDegeve_DataDergimit ON KomentetDegeve(DataDergimit);
CREATE INDEX IF NOT EXISTS IX_KomentetDegeve_EshteLexuar ON KomentetDegeve(EshteLexuar);
CREATE INDEX IF NOT EXISTS IX_KomentetDegeve_EshteZgjidhur ON KomentetDegeve(EshteZgjidhur);

-- =====================================================================
-- TABLE 10: RegjistriAuditimit (Audit Log)
-- =====================================================================
CREATE TABLE IF NOT EXISTS RegjistriAuditimit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Perdoruesi TEXT NOT NULL,
    LlojiVeprimit TEXT NOT NULL,
    Entiteti TEXT NOT NULL,
    EntitetiId TEXT,
    Perpara TEXT,
    Pas TEXT,
    Koha TEXT NOT NULL,
    AdresaIP TEXT,
    UserAgent TEXT,
    Sesioni_Id TEXT,
    Pershkrimi TEXT
);

CREATE INDEX IF NOT EXISTS IX_RegjistriAuditimit_Perdoruesi ON RegjistriAuditimit(Perdoruesi);
CREATE INDEX IF NOT EXISTS IX_RegjistriAuditimit_LlojiVeprimit ON RegjistriAuditimit(LlojiVeprimit);
CREATE INDEX IF NOT EXISTS IX_RegjistriAuditimit_Entiteti ON RegjistriAuditimit(Entiteti);
CREATE INDEX IF NOT EXISTS IX_RegjistriAuditimit_Koha ON RegjistriAuditimit(Koha);
CREATE INDEX IF NOT EXISTS IX_RegjistriAuditimit_Sesioni_Id ON RegjistriAuditimit(Sesioni_Id);

-- =====================================================================
-- TABLE 11: SesionetPerdoruesve (User Sessions)
-- =====================================================================
CREATE TABLE IF NOT EXISTS SesionetPerdoruesve (
    Sesioni_Id TEXT PRIMARY KEY,
    Perdoruesi TEXT NOT NULL,
    Filluar_Me TEXT NOT NULL,
    Mbaruar_Me TEXT,
    AdresaIP TEXT,
    UserAgent TEXT,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1,
    Token TEXT
);

CREATE INDEX IF NOT EXISTS IX_SesionetPerdoruesve_Perdoruesi ON SesionetPerdoruesve(Perdoruesi);
CREATE INDEX IF NOT EXISTS IX_SesionetPerdoruesve_Filluar_Me ON SesionetPerdoruesve(Filluar_Me);
CREATE INDEX IF NOT EXISTS IX_SesionetPerdoruesve_Eshte_Aktiv ON SesionetPerdoruesve(Eshte_Aktiv);

-- =====================================================================
-- TABLE 12: KerkesatRegjistrim (Registration Requests)
-- =====================================================================
CREATE TABLE IF NOT EXISTS KerkesatRegjistrim (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmriPlote TEXT NOT NULL,
    Email TEXT NOT NULL,
    NrTelefonit TEXT,
    Dega_Id INTEGER,
    Pozita TEXT,
    Arsyeja TEXT,
    Statusi TEXT NOT NULL DEFAULT 'Ne Pritje',
    DataKerkeses TEXT NOT NULL,
    DataShqyrtimit TEXT,
    Shqyrtuar_Nga TEXT,
    Komente TEXT,
    FOREIGN KEY (Dega_Id) REFERENCES Deget(Id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS IX_KerkesatRegjistrim_Email ON KerkesatRegjistrim(Email);
CREATE INDEX IF NOT EXISTS IX_KerkesatRegjistrim_Statusi ON KerkesatRegjistrim(Statusi);
CREATE INDEX IF NOT EXISTS IX_KerkesatRegjistrim_DataKerkeses ON KerkesatRegjistrim(DataKerkeses);

-- =====================================================================
-- TABLE 13: PreferencatPerdoruesve (User Preferences)
-- =====================================================================
CREATE TABLE IF NOT EXISTS PreferencatPerdoruesve (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Perdoruesi_Id TEXT NOT NULL,
    Gjuha TEXT DEFAULT 'sq-AL',
    Tema TEXT DEFAULT 'light',
    Faqosje_PerFaqe INTEGER DEFAULT 25,
    Formati_Raportit TEXT DEFAULT 'PDF',
    Njoftime_Email INTEGER NOT NULL DEFAULT 1,
    Te_dhena_JSON TEXT,
    FOREIGN KEY (Perdoruesi_Id) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_PreferencatPerdoruesve_Perdoruesi_Id ON PreferencatPerdoruesve(Perdoruesi_Id);

-- =====================================================================
-- TABLE 14: TabelatCustom (Custom Tables)
-- =====================================================================
CREATE TABLE IF NOT EXISTS TabelatCustom (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmriTabeles TEXT NOT NULL UNIQUE,
    Pershkrimi TEXT,
    Struktura_JSON TEXT NOT NULL,
    Krijuar_Me TEXT NOT NULL,
    Krijuar_Nga TEXT,
    Eshte_Aktiv INTEGER NOT NULL DEFAULT 1
);

-- =====================================================================
-- TABLE 15: FormatetiPrintimit (Print Formats)
-- =====================================================================
CREATE TABLE IF NOT EXISTS FormatetiPrintimit (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmriFormatit TEXT NOT NULL,
    LlojiRaportit TEXT NOT NULL,
    Template_HTML TEXT,
    CSS_Shtese TEXT,
    HeaderPath TEXT,
    FooterPath TEXT,
    Eshte_Default INTEGER NOT NULL DEFAULT 0,
    Krijuar_Me TEXT NOT NULL,
    Krijuar_Nga TEXT
);

-- =====================================================================
-- OTHER IDENTITY TABLES
-- =====================================================================

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId TEXT NOT NULL,
    RoleId TEXT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider TEXT NOT NULL,
    ProviderKey TEXT NOT NULL,
    ProviderDisplayName TEXT,
    UserId TEXT NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId TEXT NOT NULL,
    LoginProvider TEXT NOT NULL,
    Name TEXT NOT NULL,
    Value TEXT,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- =====================================================================
-- SAMPLE DATA INSERT (20 Products from Kosovo Customs)
-- =====================================================================

INSERT OR IGNORE INTO VleratProdukteve (Id, Kodi, KodiProduktit, EmriProduktit, Pershkrimi, Origjina, Pariteti, Njesia, Cmimi, VleraDoganore, Valuta, Kategoria, Eshte_Aktiv, Krijuar_Me, Krijuar_Nga) VALUES
(1, '7615', '7615', 'Tepsi alumini', 'Tepsi alumini 0.8mm me dim: 30cm ; 32cm', 'TR', 'DAT', 'Copë', '1.20 ; 1.45', 1.20, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(2, '0102', '0102-01', 'Viça', 'Viça (gjedhet femra që nuk kanë pjellë, mshqerra)', 'TE NDRYSHME', 'DAT', 'KG', '2.70', 2.70, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(3, '0102', '0102-02', 'Dema', 'Dema per therje me peshe mbi 160 kg', 'TE NDRYSHME', 'DAT', 'KG', '2.40', 2.40, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(4, '0102', '0102-03', 'Lopet', 'Lopet per therje', 'TE NDRYSHME', 'DAT', 'KG', '1.50', 1.50, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(5, '7323', '7323-01', 'Tepsi e zezë', 'Tepsi e zezë e emajluar me dim:40 cm', 'TR', 'DAT', 'Copë', '2.50', 2.50, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(6, '7323', '7323-02', 'Tepsi e zezë me lule', 'Tepsi e zezë e emajluar(me lule) dim: 40cm ; 42 cm', 'TR', 'DAT', 'Copë', '2.50 ; 3.20', 2.50, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin'),
(7, '0402', '0402-01', 'QUMESHT PLUHUR A', 'QUMESHT PLUHUR A / DRY-MILK-PW-NED', 'FR', 'EXW', 'TON', '', 0, 'EUR', 'Çmimi nga Bursa', 1, '2024-01-01 00:00:00', 'Admin'),
(8, '0402', '0402-02', 'QUMESHT PLUHUR H', 'QUMESHT PLUHUR H / DRY-MILK-PW-EDI', 'FR', 'EXW', 'TON', '', 0, 'EUR', 'Çmimi nga Bursa', 1, '2024-01-01 00:00:00', 'Admin'),
(9, '0701', '0701', 'Patate', 'Patate te reja', 'TR', 'DAT', 'KG', '0.45', 0.45, 'EUR', 'Pemë-Perime', 1, '2024-01-01 00:00:00', 'Admin'),
(10, '5607', '5607', 'Litar-spango', 'Litar-spango', 'CN', 'EXW', 'KG', '1.25', 1.25, 'EUR', 'Tjera', 1, '2024-01-01 00:00:00', 'Admin');

-- =====================================================================
-- END OF DATABASE SCHEMA
-- =====================================================================

-- To view current data, use:
-- SELECT * FROM VleratProdukteve;
-- SELECT * FROM HistoriaVlerave;
-- SELECT * FROM ShpenzimetTransportit;
-- SELECT * FROM KomentetDegeve;
-- SELECT * FROM RegjistriAuditimit;
