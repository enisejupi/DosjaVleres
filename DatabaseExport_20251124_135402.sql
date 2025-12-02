PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "AspNetRoles" (

    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,

    "Name" TEXT NULL,

    "NormalizedName" TEXT NULL,

    "ConcurrencyStamp" TEXT NULL

);
INSERT INTO AspNetRoles VALUES('bbe8b6cd-7107-4459-b2b7-3319fee8aced','Admin','ADMIN','a57eacfd-93ea-4339-95d4-ba9932e6ccfe');
INSERT INTO AspNetRoles VALUES('9aa2dff9-461e-49c9-8f23-21b0a4e98ef8','Zyrtar','ZYRTAR','e909c9eb-6506-4983-b6e8-26a16417968f');
INSERT INTO AspNetRoles VALUES('4ca15562-62d5-42b3-b6ab-0cdc30ea0171','Shikues','SHIKUES','5b6b3743-3b69-4e38-81bb-4b9256d5bc7a');
CREATE TABLE IF NOT EXISTS "Deget" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_Deget" PRIMARY KEY AUTOINCREMENT,

    "KodiDeges" TEXT NOT NULL,

    "EmriDeges" TEXT NOT NULL,

    "Qyteti" TEXT NULL,

    "Adresa" TEXT NULL,

    "Telefoni" TEXT NULL,

    "Email" TEXT NULL,

    "Eshte_Aktiv" INTEGER NOT NULL,

    "Krijuar_Me" TEXT NOT NULL,

    "Krijuar_Nga" TEXT NULL

);
INSERT INTO Deget VALUES(1,'PR-01','PRISHTINA - KRYESORJA','Prishtin├½',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136238','System');
INSERT INTO Deget VALUES(2,'PR-02','PRISHTINA - TERMINALI','Prishtin├½',NULL,NULL,NULL,1,'2025-11-24 12:23:43.513675','System');
INSERT INTO Deget VALUES(3,'MT-01','MITROVICA - TERMINALI','Mitrovic├½',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136783','System');
INSERT INTO Deget VALUES(4,'PZ-01','PRIZREN','Prizren',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136784','System');
INSERT INTO Deget VALUES(5,'GJ-01','GJAKOVA','Gjakov├½',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136786','System');
INSERT INTO Deget VALUES(6,'PE-01','PEJA','Pej├½',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136791','System');
INSERT INTO Deget VALUES(7,'GI-01','GJILAN','Gjilan',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136792','System');
INSERT INTO Deget VALUES(8,'FE-01','FERIZAJ','Ferizaj',NULL,NULL,NULL,1,'2025-11-24 12:23:43.5136793','System');
CREATE TABLE IF NOT EXISTS "FormatetiPrintimit" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_FormatetiPrintimit" PRIMARY KEY AUTOINCREMENT,

    "EmriFormatit" TEXT NOT NULL,

    "LlojiModulit" TEXT NOT NULL,

    "HtmlTemplate" TEXT NOT NULL,

    "CssStyle" TEXT NOT NULL,

    "LogoUrl" TEXT NULL,

    "LogoPosition" TEXT NULL,

    "PaperSize" TEXT NOT NULL,

    "KrijuarNga" TEXT NOT NULL,

    "KrijuarMe" TEXT NOT NULL,

    "EshteDefault" INTEGER NOT NULL,

    "PerditesomMe" TEXT NULL

);
CREATE TABLE IF NOT EXISTS "KerkesatRegjistrim" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_KerkesatRegjistrim" PRIMARY KEY AUTOINCREMENT,

    "Email" TEXT NOT NULL,

    "EmriPlote" TEXT NULL,

    "Departamenti" TEXT NULL,

    "Pozicioni" TEXT NULL,

    "KodiZyrtarit" TEXT NULL,

    "Dega_Id" INTEGER NULL,

    "EmriDeg├½s" TEXT NULL,

    "DataKerkeses" TEXT NOT NULL,

    "Statusi" TEXT NOT NULL,

    "DataShqyrtimit" TEXT NULL,

    "ShqyrtuesId" TEXT NULL,

    "ArsetimiRefuzimit" TEXT NULL,

    "ADUsername" TEXT NULL,

    "EshteVerifikuarNeAD" INTEGER NOT NULL

);
CREATE TABLE IF NOT EXISTS "PreferencatPerdoruesit" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_PreferencatPerdoruesit" PRIMARY KEY AUTOINCREMENT,

    "Perdoruesi" TEXT NOT NULL,

    "LlojiPreferences" TEXT NOT NULL,

    "Vlera" TEXT NOT NULL,

    "KrijuarMe" TEXT NOT NULL,

    "PerditesomMe" TEXT NULL

);
CREATE TABLE IF NOT EXISTS "RegjistriAuditimit" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegjistriAuditimit" PRIMARY KEY AUTOINCREMENT,

    "Perdoruesi" TEXT NOT NULL,

    "LlojiVeprimit" TEXT NOT NULL,

    "Entiteti" TEXT NOT NULL,

    "Entiteti_Id" TEXT NULL,

    "Detajet" TEXT NULL,

    "Vlerat_Vjetra" TEXT NULL,

    "Vlerat_Reja" TEXT NULL,

    "Koha" TEXT NOT NULL,

    "AdresaIP" TEXT NULL,

    "UserAgent" TEXT NULL,

    "Eshte_Suksesshem" INTEGER NOT NULL,

    "MesazhiGabimit" TEXT NULL,

    "Sesioni_Id" TEXT NULL

);
INSERT INTO RegjistriAuditimit VALUES(1,'Anonim','ShikoListen','VleraProduktit',NULL,'K├½rkimi: , Kategoria: , Lloji: ',NULL,NULL,'2025-11-24 12:24:10.0407051','::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36',1,NULL,'fd9b6d7a-8855-6075-8340-a53cf285a268');
INSERT INTO RegjistriAuditimit VALUES(2,'Anonim','ShikoListen','ShpenzimiTransportit',NULL,'Origjina: , Destinacioni: , Lloji: ',NULL,NULL,'2025-11-24 12:24:21.4377126','::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36',1,NULL,'31b6573d-fee6-bc86-bf03-fedb617d9a41');
INSERT INTO RegjistriAuditimit VALUES(3,'shaban.ejupi@dogana-rks.org','ShikoListen','VleraProduktit',NULL,'K├½rkimi: , Kategoria: , Lloji: ',NULL,NULL,'2025-11-24 12:24:53.1396714','::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36',1,NULL,'fc8b6e15-c138-7c87-0341-6892c0340d44');
INSERT INTO RegjistriAuditimit VALUES(4,'shaban.ejupi@dogana-rks.org','ShikoListen','VleraProduktit',NULL,'K├½rkimi: , Kategoria: , Lloji: ',NULL,NULL,'2025-11-24 12:38:21.5060935','::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36',1,NULL,'66eec79b-5f2d-e699-6e22-8f4a306794c6');
INSERT INTO RegjistriAuditimit VALUES(5,'shaban.ejupi@dogana-rks.org','ShikoListen','VleraProduktit',NULL,'K├½rkimi: , Kategoria: , Lloji: ',NULL,NULL,'2025-11-24 12:38:25.3982753','::1','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36',1,NULL,'95247587-a7e7-cdbf-423f-b8f3490dcddb');
CREATE TABLE IF NOT EXISTS "SesionetPerdoruesve" (

    "Sesioni_Id" TEXT NOT NULL CONSTRAINT "PK_SesionetPerdoruesve" PRIMARY KEY,

    "Perdoruesi" TEXT NOT NULL,

    "Filluar_Me" TEXT NOT NULL,

    "Perfunduar_Me" TEXT NULL,

    "Eshte_Aktiv" INTEGER NOT NULL,

    "AdresaIP" TEXT NULL,

    "UserAgent" TEXT NULL,

    "Kohezgjatja_Minuta" INTEGER NULL,

    "AktivitetiI_Fundit" TEXT NOT NULL,

    "NumriVeprimeve" INTEGER NOT NULL,

    "ArsyejaPerfundimit" TEXT NULL

);
CREATE TABLE IF NOT EXISTS "ShpenzimetTransportit" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_ShpenzimetTransportit" PRIMARY KEY AUTOINCREMENT,

    "VendiOrigjines" TEXT NOT NULL,

    "VendiDestinacionit" TEXT NOT NULL,

    "LlojiTransportit" TEXT NOT NULL,

    "CmimiPerNjesi" decimal(18,2) NOT NULL,

    "Valuta" TEXT NOT NULL,

    "NjesiaMatese" TEXT NOT NULL,

    "Shenime" TEXT NULL,

    "Aktiv" INTEGER NOT NULL,

    "DataKrijimit" TEXT NOT NULL,

    "KrijuarNga" TEXT NULL,

    "DataPerditesimit" TEXT NULL,

    "PerditesoPrejNga" TEXT NULL

);
INSERT INTO ShpenzimetTransportit VALUES(1,'Shqip├½ri','Kosov├½','Tok├½sor',0.5,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5644647','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(2,'Turqi','Kosov├½','Ajror',2.5,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645138','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(3,'Gjermani','Kosov├½','Hekurudhor',0.800000000000000044,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645141','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(4,'Itali','Kosov├½','Hekurudhor',0.75,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645144','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(5,'Slloveni','Kosov├½','Hekurudhor',0.650000000000000022,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645146','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(6,'Kroaci','Kosov├½','Detar',0.400000000000000022,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645152','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(7,'Maqedoni','Kosov├½','Tok├½sor',0.349999999999999977,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645153','Sistem',NULL,NULL);
INSERT INTO ShpenzimetTransportit VALUES(8,'Serbi','Kosov├½','Tok├½sor',0.299999999999999988,'EUR','kg',NULL,1,'2025-11-24 12:23:43.5645155','Sistem',NULL,NULL);
CREATE TABLE IF NOT EXISTS "TabelatCustom" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_TabelatCustom" PRIMARY KEY AUTOINCREMENT,

    "EmriTabeles" TEXT NOT NULL,

    "Pershkrimi" TEXT NULL,

    "Skema" TEXT NOT NULL,

    "KrijuarNga" TEXT NOT NULL,

    "KrijuarMe" TEXT NOT NULL,

    "EshteAktive" INTEGER NOT NULL,

    "PerditesomMe" TEXT NULL

);
CREATE TABLE IF NOT EXISTS "VleratProdukteve" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_VleratProdukteve" PRIMARY KEY AUTOINCREMENT,

    "KodiProduktit" TEXT NOT NULL,

    "EmriProduktit" TEXT NOT NULL,

    "Kodi" TEXT NOT NULL,

    "Pershkrimi" TEXT NOT NULL,

    "Origjina" TEXT NOT NULL,

    "Pariteti" TEXT NULL,

    "Njesia" TEXT NOT NULL,

    "Cmimi" TEXT NULL,

    "VleraDoganore" TEXT NOT NULL,

    "Valuta" TEXT NOT NULL,

    "Kategoria" TEXT NOT NULL,

    "Komentet" TEXT NULL,

    "Eshte_Aktiv" INTEGER NOT NULL,

    "Krijuar_Me" TEXT NOT NULL,

    "Krijuar_Nga" TEXT NULL,

    "Modifikuar_Me" TEXT NULL,

    "Modifikuar_Nga" TEXT NULL

);
INSERT INTO VleratProdukteve VALUES(1,'7615','Tepsi alumini','7615','Tepsi alumini 0.8mm me dim: 30cm ; 32cm','TR','DAT','Cop├½','1.20 ; 1.45','1.2','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(2,'0102-01','Vi├ºa','0102','Vi├ºa (gjedhet femra q├½ nuk kan├½ pjell├½, mshqerra)','TE NDRYSHME','DAT','KG','2.70','2.7','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(3,'0102-02','Dema','0102','Dema per therje me peshe mbi 160 kg','TE NDRYSHME','DAT','KG','2.40','2.4','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(4,'0102-03','Lopet','0102','Lopet per therje','TE NDRYSHME','DAT','KG','1.50','1.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(5,'7323-01','Tepsi e zez├½','7323','Tepsi e zez├½ e emajluar me dim:40 cm','TR','DAT','Cop├½','2.50','2.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(6,'7323-02','Tepsi e zez├½ me lule','7323','Tepsi e zez├½ e emajluar(me lule) dim: 40cm ; 42 cm','TR','DAT','Cop├½','2.50 ; 3.20','2.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(7,'7323-03','Tepsi e zez├½ emajluara','7323','Tepsi e zez├½ e emajluara dim :34 cm;36 cm;38cm','TR','DAT','Cop├½','1.50 ; 1.85 ; 2.30','1.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(8,'7323-04','Tepsi teflon','7323','Tepsi teflon me dim: 30cm;32cm','TR','DAT','Cop├½','3.00 ; 3.50','3.0','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(9,'7323-05','Tepsi teflon','7323','Tepsi teflon me dim: 34cm ;36cm','TR','DAT','Cop├½','3.80 ; 4.80','3.8','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(10,'7323-06','Tepsi metalike','7323','Tepsi metalike 1/3 (28cm;32cm;36cm)','CN','EXW','Kompleti','2.70','2.7','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(11,'7323-07','Tepsi teflon','7323','Tepsi teflon me dim: 38cm; 40cm','TR','DAT','Cop├½','4.50 ;5.00','4.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(12,'7323-08','Tepsi e zez├½ emajluar','7323','Tepsi e zez├½ e emajluar me dim. 34 cm ; 36 cm ; 38cm','TR','DAT','Cop├½','1.50 ; 1.60 ; 1.85','1.5','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(13,'7323-09','Tepsi e zez├½ emajluar','7323','Tepsi e zez├½ e emajluar dim;28cm ;30cm ; 32cm','TR','DAT','Cop├½','1.20 ; 1.30 ; 1.50','1.2','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(14,'0402-01','QUMESHT PLUHUR A','0402','QUMESHT PLUHUR A / DRY-MILK-PW-NED','FR','EXW','TON','','0.0','EUR','├çmimi nga Bursa',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(15,'0402-02','QUMESHT PLUHUR H','0402','QUMESHT PLUHUR H / DRY-MILK-PW-EDI','FR','EXW','TON','','0.0','EUR','├çmimi nga Bursa',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(16,'0701','Patate','0701','Patate te reja','TR','DAT','KG','0.45','0.45','EUR','Pem├½-Perime',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(17,'5607','Litar-spango','5607','Litar-spango','CN','EXW','KG','1.25','1.25','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(18,'6005-01','Perde sintetike','6005','Perde sintetike te firmave te njohura','CN','EXW','KG','7.20','7.2','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(19,'6005-02','Perde sintetike','6005','Perde sintetike te firmave te njohura','TR','DAT','KG','12.00','12.0','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
INSERT INTO VleratProdukteve VALUES(20,'6601','Cadra shiu','6601','Cadra shiu','CN','EXW','Cop├½','0.35','0.35','EUR','Tjera',NULL,1,'2024-01-01 00:00:00','Admin',NULL,NULL);
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,

    "RoleId" TEXT NOT NULL,

    "ClaimType" TEXT NULL,

    "ClaimValue" TEXT NULL,

    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUsers" (

    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,

    "EmriPlote" TEXT NULL,

    "Departamenti" TEXT NULL,

    "Pozicioni" TEXT NULL,

    "Dega_Id" INTEGER NULL,

    "KodiZyrtarit" TEXT NULL,

    "DataKrijimit" TEXT NOT NULL,

    "HyrjaEFundit" TEXT NULL,

    "EshteAktiv" INTEGER NOT NULL,

    "UserName" TEXT NULL,

    "NormalizedUserName" TEXT NULL,

    "Email" TEXT NULL,

    "NormalizedEmail" TEXT NULL,

    "EmailConfirmed" INTEGER NOT NULL,

    "PasswordHash" TEXT NULL,

    "SecurityStamp" TEXT NULL,

    "ConcurrencyStamp" TEXT NULL,

    "PhoneNumber" TEXT NULL,

    "PhoneNumberConfirmed" INTEGER NOT NULL,

    "TwoFactorEnabled" INTEGER NOT NULL,

    "LockoutEnd" TEXT NULL,

    "LockoutEnabled" INTEGER NOT NULL,

    "AccessFailedCount" INTEGER NOT NULL,

    CONSTRAINT "FK_AspNetUsers_Deget_Dega_Id" FOREIGN KEY ("Dega_Id") REFERENCES "Deget" ("Id") ON DELETE SET NULL

);
INSERT INTO AspNetUsers VALUES('6661fa68-09ca-46cf-a7f5-e6e26703476b','shaban.ejupi@dogana-rks.org','IT','Administrator',NULL,'CS-0001','2025-11-24 12:23:43.3479045','2025-11-24 12:24:45.5523883',1,'shaban.ejupi@dogana-rks.org','SHABAN.EJUPI@DOGANA-RKS.ORG','shaban.ejupi@dogana-rks.org','SHABAN.EJUPI@DOGANA-RKS.ORG',1,'AQAAAAIAAYagAAAAEOjO2AgxdLmhRWJiR/PhQQGiXw4mRYad0A+K1RFGA150t3riWnvtXcktkKJqseYznw==','ZMVJ6QLW3KIP7RM3LVFPY3XVEQNH7JI5','f97ad3a8-38a0-4b61-b7e1-add35699f74a',NULL,0,0,NULL,1,0);
CREATE TABLE IF NOT EXISTS "NdryshimetTransportit" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_NdryshimetTransportit" PRIMARY KEY AUTOINCREMENT,

    "ShpenzimiTransportit_Id" INTEGER NOT NULL,

    "Cmimi_Mepar" decimal(18,2) NOT NULL,

    "Cmimi_Ri" decimal(18,2) NOT NULL,

    "Valuta_Mepar" TEXT NULL,

    "Valuta_Re" TEXT NULL,

    "ArsyejaE_Ndryshimit" TEXT NULL,

    "Ndryshuar_Me" TEXT NOT NULL,

    "Ndryshuar_Nga" TEXT NULL,

    "AdresaIP" TEXT NULL,

    "NumriVersionit" INTEGER NOT NULL,

    CONSTRAINT "FK_NdryshimetTransportit_ShpenzimetTransportit_ShpenzimiTransportit_Id" FOREIGN KEY ("ShpenzimiTransportit_Id") REFERENCES "ShpenzimetTransportit" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "HistoriaVlerave" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_HistoriaVlerave" PRIMARY KEY AUTOINCREMENT,

    "VleraProduktit_Id" INTEGER NOT NULL,

    "Vlera_Mepar" decimal(18,2) NOT NULL,

    "Vlera_Re" decimal(18,2) NOT NULL,

    "Valuta_Mepar" TEXT NULL,

    "Valuta_Re" TEXT NULL,

    "ArsyejaE_Ndryshimit" TEXT NULL,

    "FotoNdryshimit" TEXT NULL,

    "Ndryshuar_Me" TEXT NOT NULL,

    "Ndryshuar_Nga" TEXT NOT NULL,

    "AdresaIP" TEXT NULL,

    "NumriVersionit" INTEGER NOT NULL,

    CONSTRAINT "FK_HistoriaVlerave_VleratProdukteve_VleraProduktit_Id" FOREIGN KEY ("VleraProduktit_Id") REFERENCES "VleratProdukteve" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "Komentet" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_Komentet" PRIMARY KEY AUTOINCREMENT,

    "VleraProduktitId" INTEGER NOT NULL,

    "Permbajtja" TEXT NOT NULL,

    "Krijuar_Nga" TEXT NULL,

    "Krijuar_Me" TEXT NOT NULL,

    CONSTRAINT "FK_Komentet_VleratProdukteve_VleraProduktitId" FOREIGN KEY ("VleraProduktitId") REFERENCES "VleratProdukteve" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "KomentetDegeve" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_KomentetDegeve" PRIMARY KEY AUTOINCREMENT,

    "EmriDeges" TEXT NOT NULL,

    "KodiTarifar" TEXT NOT NULL,

    "Mesazhi" TEXT NOT NULL,

    "DataDergimit" TEXT NOT NULL,

    "DergoPrejNga" TEXT NULL,

    "EshteLexuar" INTEGER NOT NULL,

    "DataLeximit" TEXT NULL,

    "LexoNga" TEXT NULL,

    "EshteZgjidhur" INTEGER NOT NULL,

    "Pergjigja" TEXT NULL,

    "DataPergjigjes" TEXT NULL,

    "PergjigjetNga" TEXT NULL,

    "VleraProduktit_Id" INTEGER NULL,

    CONSTRAINT "FK_KomentetDegeve_VleratProdukteve_VleraProduktit_Id" FOREIGN KEY ("VleraProduktit_Id") REFERENCES "VleratProdukteve" ("Id") ON DELETE SET NULL

);
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (

    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,

    "UserId" TEXT NOT NULL,

    "ClaimType" TEXT NULL,

    "ClaimValue" TEXT NULL,

    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (

    "LoginProvider" TEXT NOT NULL,

    "ProviderKey" TEXT NOT NULL,

    "ProviderDisplayName" TEXT NULL,

    "UserId" TEXT NOT NULL,

    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),

    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (

    "UserId" TEXT NOT NULL,

    "RoleId" TEXT NOT NULL,

    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),

    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,

    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
INSERT INTO AspNetUserRoles VALUES('6661fa68-09ca-46cf-a7f5-e6e26703476b','bbe8b6cd-7107-4459-b2b7-3319fee8aced');
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (

    "UserId" TEXT NOT NULL,

    "LoginProvider" TEXT NOT NULL,

    "Name" TEXT NOT NULL,

    "Value" TEXT NULL,

    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),

    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE

);
DELETE FROM sqlite_sequence;
INSERT INTO sqlite_sequence VALUES('VleratProdukteve',20);
INSERT INTO sqlite_sequence VALUES('Deget',8);
INSERT INTO sqlite_sequence VALUES('ShpenzimetTransportit',8);
INSERT INTO sqlite_sequence VALUES('RegjistriAuditimit',5);
CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE INDEX "IX_AspNetUsers_Dega_Id" ON "AspNetUsers" ("Dega_Id");
CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");
CREATE INDEX "IX_Deget_EmriDeges" ON "Deget" ("EmriDeges");
CREATE INDEX "IX_Deget_Eshte_Aktiv" ON "Deget" ("Eshte_Aktiv");
CREATE UNIQUE INDEX "IX_Deget_KodiDeges" ON "Deget" ("KodiDeges");
CREATE INDEX "IX_HistoriaVlerave_Ndryshuar_Me" ON "HistoriaVlerave" ("Ndryshuar_Me");
CREATE INDEX "IX_HistoriaVlerave_Ndryshuar_Nga" ON "HistoriaVlerave" ("Ndryshuar_Nga");
CREATE INDEX "IX_HistoriaVlerave_VleraProduktit_Id" ON "HistoriaVlerave" ("VleraProduktit_Id");
CREATE INDEX "IX_Komentet_Krijuar_Me" ON "Komentet" ("Krijuar_Me");
CREATE INDEX "IX_Komentet_VleraProduktitId" ON "Komentet" ("VleraProduktitId");
CREATE INDEX "IX_KomentetDegeve_DataDergimit" ON "KomentetDegeve" ("DataDergimit");
CREATE INDEX "IX_KomentetDegeve_EmriDeges" ON "KomentetDegeve" ("EmriDeges");
CREATE INDEX "IX_KomentetDegeve_EshteLexuar" ON "KomentetDegeve" ("EshteLexuar");
CREATE INDEX "IX_KomentetDegeve_EshteZgjidhur" ON "KomentetDegeve" ("EshteZgjidhur");
CREATE INDEX "IX_KomentetDegeve_KodiTarifar" ON "KomentetDegeve" ("KodiTarifar");
CREATE INDEX "IX_KomentetDegeve_VleraProduktit_Id" ON "KomentetDegeve" ("VleraProduktit_Id");
CREATE INDEX "IX_NdryshimetTransportit_Ndryshuar_Me" ON "NdryshimetTransportit" ("Ndryshuar_Me");
CREATE INDEX "IX_NdryshimetTransportit_ShpenzimiTransportit_Id" ON "NdryshimetTransportit" ("ShpenzimiTransportit_Id");
CREATE INDEX "IX_RegjistriAuditimit_Entiteti" ON "RegjistriAuditimit" ("Entiteti");
CREATE INDEX "IX_RegjistriAuditimit_Koha" ON "RegjistriAuditimit" ("Koha");
CREATE INDEX "IX_RegjistriAuditimit_LlojiVeprimit" ON "RegjistriAuditimit" ("LlojiVeprimit");
CREATE INDEX "IX_RegjistriAuditimit_Perdoruesi" ON "RegjistriAuditimit" ("Perdoruesi");
CREATE INDEX "IX_RegjistriAuditimit_Sesioni_Id" ON "RegjistriAuditimit" ("Sesioni_Id");
CREATE INDEX "IX_SesionetPerdoruesve_Eshte_Aktiv" ON "SesionetPerdoruesve" ("Eshte_Aktiv");
CREATE INDEX "IX_SesionetPerdoruesve_Filluar_Me" ON "SesionetPerdoruesve" ("Filluar_Me");
CREATE INDEX "IX_SesionetPerdoruesve_Perdoruesi" ON "SesionetPerdoruesve" ("Perdoruesi");
CREATE INDEX "IX_ShpenzimetTransportit_Aktiv" ON "ShpenzimetTransportit" ("Aktiv");
CREATE INDEX "IX_ShpenzimetTransportit_LlojiTransportit" ON "ShpenzimetTransportit" ("LlojiTransportit");
CREATE INDEX "IX_ShpenzimetTransportit_VendiDestinacionit" ON "ShpenzimetTransportit" ("VendiDestinacionit");
CREATE INDEX "IX_ShpenzimetTransportit_VendiOrigjines" ON "ShpenzimetTransportit" ("VendiOrigjines");
CREATE INDEX "IX_VleratProdukteve_Eshte_Aktiv" ON "VleratProdukteve" ("Eshte_Aktiv");
CREATE INDEX "IX_VleratProdukteve_Kategoria" ON "VleratProdukteve" ("Kategoria");
CREATE INDEX "IX_VleratProdukteve_Kodi" ON "VleratProdukteve" ("Kodi");
CREATE INDEX "IX_VleratProdukteve_Origjina" ON "VleratProdukteve" ("Origjina");
COMMIT;
