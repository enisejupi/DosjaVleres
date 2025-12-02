using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Te_dhenat;
using System.Text.Json;

namespace KosovaDoganaModerne.Controllers
{
    [Authorize]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public AdminController(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("DatabaseSchema")]
        public IActionResult DatabaseSchema()
        {
            try
            {
                var model = new DatabaseSchemaViewModel
                {
                    Tables = new List<TableInfo>
                    {
                        new TableInfo
                        {
                            Name = "VleratProdukteve",
                            Description = "Vlerat e produkteve doganore",
                            RowCount = _konteksti.VleratProdukteve.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "Kodi", Type = "string(50)", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "Pershkrimi", Type = "string(500)", IsRequired = true },
                                new ColumnInfo { Name = "Vlera", Type = "decimal", IsRequired = true },
                                new ColumnInfo { Name = "Kategoria", Type = "string(100)", IsIndexed = true },
                                new ColumnInfo { Name = "Origjina", Type = "string(100)", IsIndexed = true },
                                new ColumnInfo { Name = "Eshte_Aktiv", Type = "bool", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "Krijuar_Me", Type = "DateTime", IsRequired = true },
                                new ColumnInfo { Name = "Krijuar_Nga", Type = "string(100)" },
                                new ColumnInfo { Name = "Perditesuar_Me", Type = "DateTime?" },
                                new ColumnInfo { Name = "Perditesuar_Nga", Type = "string(100)" }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string> { "HistoriaVlerave", "Komentet", "KomentetDegeve" }
                        },
                        new TableInfo
                        {
                            Name = "KomentetDegeve",
                            Description = "Komentet nga degët e doganës",
                            RowCount = _konteksti.KomentetDegeve.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "EmriDeges", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "KodiTarifar", Type = "string(20)", IsRequired = true },
                                new ColumnInfo { Name = "Mesazhi", Type = "string(2000)", IsRequired = true },
                                new ColumnInfo { Name = "DataDergimit", Type = "DateTime", IsRequired = true },
                                new ColumnInfo { Name = "DergoPrejNga", Type = "string(100)" },
                                new ColumnInfo { Name = "EshteLexuar", Type = "bool", IsRequired = true },
                                new ColumnInfo { Name = "EshteZgjidhur", Type = "bool", IsRequired = true },
                                new ColumnInfo { Name = "Pergjigja", Type = "string(2000)" },
                                new ColumnInfo { Name = "VleraProduktit_Id", Type = "int?", IsForeignKey = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>
                            {
                                new ForeignKeyInfo { Column = "VleraProduktit_Id", ReferencedTable = "VleratProdukteve", ReferencedColumn = "Id" }
                            },
                            RelatedTables = new List<string> { "VleratProdukteve" }
                        },
                        new TableInfo
                        {
                            Name = "ShpenzimetTransportit",
                            Description = "Shpenzimet e transportit",
                            RowCount = _konteksti.ShpenzimetTransportit.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "Origjina", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "Destinacioni", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "Cmimi", Type = "decimal", IsRequired = true },
                                new ColumnInfo { Name = "Monedha", Type = "string(10)", IsRequired = true },
                                new ColumnInfo { Name = "LlojiTransportit", Type = "string(50)", IsRequired = true },
                                new ColumnInfo { Name = "DataKrijimit", Type = "DateTime", IsRequired = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string> { "NdryshimetTransportit" }
                        },
                        new TableInfo
                        {
                            Name = "KerkesatRegjistrim",
                            Description = "Kërkesat për regjistrim",
                            RowCount = _konteksti.KerkesatRegjistrim.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "Emri", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "Mbiemri", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "Email", Type = "string(150)", IsRequired = true },
                                new ColumnInfo { Name = "Telefoni", Type = "string(20)" },
                                new ColumnInfo { Name = "Arsyeja", Type = "string(1000)", IsRequired = true },
                                new ColumnInfo { Name = "Statusi", Type = "string(50)", IsRequired = true },
                                new ColumnInfo { Name = "DataKerkeses", Type = "DateTime", IsRequired = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string>()
                        },
                        new TableInfo
                        {
                            Name = "RegjistriAuditimit",
                            Description = "Regjistri i auditimit të veprimeve",
                            RowCount = _konteksti.RegjistriAuditimit.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "Perdoruesi", Type = "string(100)", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "LlojiVeprimit", Type = "string(50)", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "Entiteti", Type = "string(100)", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "EntitetiId", Type = "string(50)" },
                                new ColumnInfo { Name = "Koha", Type = "DateTime", IsRequired = true, IsIndexed = true },
                                new ColumnInfo { Name = "Detajet", Type = "string(2000)" }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string>()
                        },
                        new TableInfo
                        {
                            Name = "PreferencatPerdoruesve",
                            Description = "Preferencat e përdoruesve",
                            RowCount = _konteksti.PreferencatPerdoruesve.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "Perdoruesi", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "LlojiPreferences", Type = "string(50)", IsRequired = true },
                                new ColumnInfo { Name = "Vlera", Type = "string (JSON)", IsRequired = true },
                                new ColumnInfo { Name = "KrijuarMe", Type = "DateTime", IsRequired = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string>()
                        },
                        new TableInfo
                        {
                            Name = "TabelatCustom",
                            Description = "Tabelat e krijuara nga përdoruesit",
                            RowCount = _konteksti.TabelatCustom.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "EmriTabeles", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "Pershkrimi", Type = "string(500)" },
                                new ColumnInfo { Name = "Skema", Type = "string (JSON)", IsRequired = true },
                                new ColumnInfo { Name = "KrijuarNga", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "KrijuarMe", Type = "DateTime", IsRequired = true },
                                new ColumnInfo { Name = "EshteAktive", Type = "bool", IsRequired = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string>()
                        },
                        new TableInfo
                        {
                            Name = "FormatetiPrintimit",
                            Description = "Formatet e printimit",
                            RowCount = _konteksti.FormatetiPrintimit.Count(),
                            Columns = new List<ColumnInfo>
                            {
                                new ColumnInfo { Name = "Id", Type = "int", IsPrimaryKey = true, IsRequired = true },
                                new ColumnInfo { Name = "EmriFormatit", Type = "string(100)", IsRequired = true },
                                new ColumnInfo { Name = "LlojiModulit", Type = "string(50)", IsRequired = true },
                                new ColumnInfo { Name = "HtmlTemplate", Type = "text", IsRequired = true },
                                new ColumnInfo { Name = "CssStyle", Type = "text" },
                                new ColumnInfo { Name = "LogoUrl", Type = "string(200)" },
                                new ColumnInfo { Name = "PaperSize", Type = "string(20)", IsRequired = true },
                                new ColumnInfo { Name = "EshteDefault", Type = "bool", IsRequired = true }
                            },
                            ForeignKeys = new List<ForeignKeyInfo>(),
                            RelatedTables = new List<string>()
                        }
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të skemës: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Statistics")]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var model = new StatisticsViewModel
                {
                    TotalVleratProdukteve = await _konteksti.VleratProdukteve.CountAsync(),
                    VleratAktive = await _konteksti.VleratProdukteve.CountAsync(v => v.Eshte_Aktiv),
                    TotalKomentet = await _konteksti.KomentetDegeve.CountAsync(),
                    KomentetPalexuara = await _konteksti.KomentetDegeve.CountAsync(k => !k.EshteLexuar),
                    KomentetPazgjidhura = await _konteksti.KomentetDegeve.CountAsync(k => !k.EshteZgjidhur),
                    TotalShpenzimet = await _konteksti.ShpenzimetTransportit.CountAsync(),
                    TotalKerkesa = await _konteksti.KerkesatRegjistrim.CountAsync(),
                    KerkesaPritje = await _konteksti.KerkesatRegjistrim.CountAsync(k => k.Statusi == "Në pritje"),
                    TotalDege = await _konteksti.Deget.CountAsync()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të statistikave: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("CustomTables")]
        public async Task<IActionResult> CustomTables()
        {
            var tabela = await _konteksti.TabelatCustom
                .OrderByDescending(t => t.KrijuarMe)
                .ToListAsync();
            return View(tabela);
        }

        [HttpGet("PrintFormats")]
        public async Task<IActionResult> PrintFormats()
        {
            var formate = await _konteksti.FormatetiPrintimit
                .OrderByDescending(f => f.KrijuarMe)
                .ToListAsync();
            return View(formate);
        }

        [HttpGet("PrintFormatsEditor")]
        public IActionResult PrintFormatsEditor()
        {
            return View();
        }

        [HttpGet("GlobalPrintSettings")]
        public async Task<IActionResult> GlobalPrintSettings()
        {
            try
            {
                // Merr të gjitha format globale
                var globalFormats = await _konteksti.GlobalPrintFormats
                    .Include(g => g.FormatPrintimi)
                    .Where(g => g.EshteAktiv)
                    .ToListAsync();

                // Merr të gjitha formatet e disponueshme
                var formateDisponueshme = await _konteksti.FormatetiPrintimit
                    .OrderBy(f => f.LlojiModulit)
                    .ThenBy(f => f.EmriFormatit)
                    .ToListAsync();

                var model = new GlobalPrintSettingsViewModel
                {
                    GlobalFormats = globalFormats,
                    FormateDisponueshme = formateDisponueshme,
                    ModuletDisponueshme = new List<string> 
                    { 
                        "VleratProdukteve", 
                        "KomentetDegeve", 
                        "ShpenzimiTransportit", 
                        "DosjaTeDisponueshme",
                        "RaportiKomentetDegeve"
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të formateve: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("SetGlobalPrintFormat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetGlobalPrintFormat(string llojiModulit, int formatPrintimiId)
        {
            try
            {
                var perdoruesi = User.Identity?.Name ?? "Anonim";

                // Kontrollo nëse ekziston një format global për këtë modul
                var ekzistues = await _konteksti.GlobalPrintFormats
                    .FirstOrDefaultAsync(g => g.LlojiModulit == llojiModulit && g.EshteAktiv);

                if (ekzistues != null)
                {
                    // Përditëso formatin ekzistues
                    ekzistues.FormatPrintimiId = formatPrintimiId;
                    ekzistues.ModifikuarNga = perdoruesi;
                    ekzistues.ModifikuarMe = DateTime.UtcNow;
                }
                else
                {
                    // Krijo një format të ri global
                    var formatGlobal = new Modelet.Entitetet.GlobalPrintFormat
                    {
                        LlojiModulit = llojiModulit,
                        FormatPrintimiId = formatPrintimiId,
                        VendosurNga = perdoruesi,
                        DataVendosjes = DateTime.UtcNow,
                        Pershkrimi = $"Format global për modulin {llojiModulit}",
                        EshteAktiv = true
                    };
                    _konteksti.GlobalPrintFormats.Add(formatGlobal);
                }

                await _konteksti.SaveChangesAsync();
                TempData["Sukses"] = $"Formati global për modulin '{llojiModulit}' u vendos me sukses!";
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë vendosjes së formatit: " + ex.Message;
            }

            return RedirectToAction(nameof(GlobalPrintSettings));
        }

        [HttpPost("RemoveGlobalPrintFormat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveGlobalPrintFormat(int id)
        {
            try
            {
                var formatGlobal = await _konteksti.GlobalPrintFormats.FindAsync(id);
                if (formatGlobal != null)
                {
                    formatGlobal.EshteAktiv = false;
                    formatGlobal.ModifikuarNga = User.Identity?.Name ?? "Anonim";
                    formatGlobal.ModifikuarMe = DateTime.UtcNow;
                    await _konteksti.SaveChangesAsync();
                    TempData["Sukses"] = "Formati global u çaktivizua me sukses!";
                }
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë çaktivizimit të formatit: " + ex.Message;
            }

            return RedirectToAction(nameof(GlobalPrintSettings));
        }

        [HttpGet("PrintAuditLogs")]
        public async Task<IActionResult> PrintAuditLogs(string? perdoruesi = null, string? llojiRaportit = null, 
            DateTime? ngaData = null, DateTime? deriData = null, int faqja = 1, int madhesiaFaqes = 50)
        {
            try
            {
                var query = _konteksti.PrintAuditLogs
                    .Include(p => p.FormatPrintimi)
                    .AsQueryable();

                // Filtrat
                if (!string.IsNullOrWhiteSpace(perdoruesi))
                    query = query.Where(p => p.Perdoruesi.Contains(perdoruesi));

                if (!string.IsNullOrWhiteSpace(llojiRaportit))
                    query = query.Where(p => p.LlojiRaportit == llojiRaportit);

                if (ngaData.HasValue)
                    query = query.Where(p => p.DataPrintimit >= ngaData.Value);

                if (deriData.HasValue)
                    query = query.Where(p => p.DataPrintimit <= deriData.Value.AddDays(1));

                // Totali para paginimit
                var totali = await query.CountAsync();

                // Paginimi
                var logs = await query
                    .OrderByDescending(p => p.DataPrintimit)
                    .Skip((faqja - 1) * madhesiaFaqes)
                    .Take(madhesiaFaqes)
                    .ToListAsync();

                var model = new PrintAuditLogsViewModel
                {
                    Logs = logs,
                    FaqjaAktuale = faqja,
                    MadhesiaFaqes = madhesiaFaqes,
                    TotaliFaqeve = (int)Math.Ceiling(totali / (double)madhesiaFaqes),
                    TotaliRekordet = totali,
                    Perdoruesi = perdoruesi,
                    LlojiRaportit = llojiRaportit,
                    NgaData = ngaData,
                    DeriData = deriData
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të log-eve: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("SavePrintFormat")]
        public async Task<IActionResult> SavePrintFormat([FromBody] Modelet.Entitetet.FormatPrintimi format)
        {
            try
            {
                var perdoruesi = User.Identity?.Name ?? "Anonim";
                format.KrijuarNga = perdoruesi;
                format.KrijuarMe = DateTime.UtcNow;

                // If this format is set as default, unset other defaults for this module
                if (format.EshteDefault)
                {
                    var existingDefaults = await _konteksti.FormatetiPrintimit
                        .Where(f => f.LlojiModulit == format.LlojiModulit && f.EshteDefault)
                        .ToListAsync();
                    
                    foreach (var existing in existingDefaults)
                    {
                        existing.EshteDefault = false;
                    }
                }

                _konteksti.FormatetiPrintimit.Add(format);
                await _konteksti.SaveChangesAsync();

                return Json(new { success = true, message = "Formati u ruajt me sukses!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("ColorTheme")]
        public async Task<IActionResult> ColorTheme()
        {
            var perdoruesi = User.Identity?.Name ?? "Anonim";
            var preferenca = await _konteksti.PreferencatPerdoruesve
                .FirstOrDefaultAsync(p => p.Perdoruesi == perdoruesi && p.LlojiPreferences == "ColorTheme");

            var model = new ColorThemeViewModel();
            if (preferenca != null)
            {
                model = JsonSerializer.Deserialize<ColorThemeViewModel>(preferenca.Vlera) 
                    ?? new ColorThemeViewModel();
            }

            return View(model);
        }

        [HttpPost("ColorTheme")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ColorTheme(ColorThemeViewModel model)
        {
            try
            {
                var perdoruesi = User.Identity?.Name ?? "Anonim";
                var preferenca = await _konteksti.PreferencatPerdoruesve
                    .FirstOrDefaultAsync(p => p.Perdoruesi == perdoruesi && p.LlojiPreferences == "ColorTheme");

                var json = JsonSerializer.Serialize(model);

                if (preferenca == null)
                {
                    preferenca = new Modelet.Entitetet.PreferencatPerdoruesit
                    {
                        Perdoruesi = perdoruesi,
                        LlojiPreferences = "ColorTheme",
                        Vlera = json,
                        KrijuarMe = DateTime.UtcNow
                    };
                    _konteksti.PreferencatPerdoruesve.Add(preferenca);
                }
                else
                {
                    preferenca.Vlera = json;
                    preferenca.PerditesomMe = DateTime.UtcNow;
                }

                await _konteksti.SaveChangesAsync();
                TempData["Sukses"] = "Thema e ngjyrave u ruajt me sukses!";
                return RedirectToAction(nameof(ColorTheme));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ruajtjes: " + ex.Message;
                return View(model);
            }
        }

        /// <summary>
        /// Shikues i databazës SQLite
        /// </summary>
        [HttpGet("DatabaseViewer")]
        public IActionResult DatabaseViewer()
        {
            return View();
        }
    }

    // View Models
    public class DatabaseSchemaViewModel
    {
        public List<TableInfo> Tables { get; set; } = new();
    }

    public class TableInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RowCount { get; set; }
        public List<ColumnInfo> Columns { get; set; } = new();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
        public List<string> RelatedTables { get; set; } = new();
    }

    public class ColumnInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsRequired { get; set; }
        public bool IsIndexed { get; set; }
    }

    public class ForeignKeyInfo
    {
        public string Column { get; set; } = string.Empty;
        public string ReferencedTable { get; set; } = string.Empty;
        public string ReferencedColumn { get; set; } = string.Empty;
    }

    public class ColorThemeViewModel
    {
        public string PrimaryColor { get; set; } = "#007bff";
        public string SecondaryColor { get; set; } = "#6c757d";
        public string SuccessColor { get; set; } = "#28a745";
        public string DangerColor { get; set; } = "#dc3545";
        public string WarningColor { get; set; } = "#ffc107";
        public string InfoColor { get; set; } = "#17a2b8";
        public string ButtonColor { get; set; } = "#007bff";
        public string MenuColor { get; set; } = "#343a40";
    }

    public class StatisticsViewModel
    {
        public int TotalVleratProdukteve { get; set; }
        public int VleratAktive { get; set; }
        public int TotalKomentet { get; set; }
        public int KomentetPalexuara { get; set; }
        public int KomentetPazgjidhura { get; set; }
        public int TotalShpenzimet { get; set; }
        public int TotalKerkesa { get; set; }
        public int KerkesaPritje { get; set; }
        public int TotalDege { get; set; }
    }

    public class GlobalPrintSettingsViewModel
    {
        public List<Modelet.Entitetet.GlobalPrintFormat> GlobalFormats { get; set; } = new();
        public List<Modelet.Entitetet.FormatPrintimi> FormateDisponueshme { get; set; } = new();
        public List<string> ModuletDisponueshme { get; set; } = new();
    }

    public class PrintAuditLogsViewModel
    {
        public List<Modelet.Entitetet.PrintAuditLog> Logs { get; set; } = new();
        public int FaqjaAktuale { get; set; }
        public int MadhesiaFaqes { get; set; }
        public int TotaliFaqeve { get; set; }
        public int TotaliRekordet { get; set; }
        public string? Perdoruesi { get; set; }
        public string? LlojiRaportit { get; set; }
        public DateTime? NgaData { get; set; }
        public DateTime? DeriData { get; set; }
    }
}
