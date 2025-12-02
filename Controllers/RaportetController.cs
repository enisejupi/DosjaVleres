using Microsoft.AspNetCore.Mvc;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Sherbime;

namespace KosovaDoganaModerne.Controllers
{
    [Route("Raportet")]
    public class RaportetController : Controller
    {
        private readonly IDepoja_VleraProduktit _depojaProdukteve;
        private readonly IDepoja_KomentiDeges _depojaKomenteve;
        private readonly IDepoja_ShpenzimiTransportit _depojaTransportit;
        private readonly SherbimetRaporteve _sherbimetRaporteve;
        private readonly SherbimetPrintimit _sherbimetPrintimit;

        public RaportetController(
            IDepoja_VleraProduktit depojaProdukteve, 
            IDepoja_KomentiDeges depojaKomenteve,
            IDepoja_ShpenzimiTransportit depojaTransportit,
            SherbimetRaporteve sherbimetRaporteve,
            SherbimetPrintimit sherbimetPrintimit)
        {
            _depojaProdukteve = depojaProdukteve;
            _depojaKomenteve = depojaKomenteve;
            _depojaTransportit = depojaTransportit;
            _sherbimetRaporteve = sherbimetRaporteve;
            _sherbimetPrintimit = sherbimetPrintimit;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("DosjaVlerave")]
        public async Task<IActionResult> DosjaVlerave(string? kategoria, string? origjina, bool printEditor = false)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = vlerat.Where(v => v.Kategoria == kategoria);
                }

                if (!string.IsNullOrWhiteSpace(origjina))
                {
                    vlerat = vlerat.Where(v => v.Origjina == origjina);
                }

                ViewData["Kategoria"] = kategoria;
                ViewData["Origjina"] = origjina;
                ViewData["DataGjenerimit"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                if (printEditor)
                {
                    // Generate content for universal print editor
                    var content = GenerateDosjaVleraweContent(vlerat, kategoria, origjina);
                    ViewData["Content"] = content;
                    ViewData["ModuleType"] = "DosjaVlerave";
                    return View("~/Views/Shared/PrintEditorUniversal.cshtml");
                }

                return View(vlerat);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("DosjaVlerave/Excel")]
        public async Task<IActionResult> DosjaVlerave_Excel(string? kategoria, string? origjina)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = vlerat.Where(v => v.Kategoria == kategoria);
                }

                if (!string.IsNullOrWhiteSpace(origjina))
                {
                    vlerat = vlerat.Where(v => v.Origjina == origjina);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Vlerave" }
                };
                if (!string.IsNullOrEmpty(kategoria))
                    parametra.Add("Kategoria", kategoria);
                if (!string.IsNullOrEmpty(origjina))
                    parametra.Add("Origjina", origjina);

                var excelData = await _sherbimetPrintimit.GjeneroExcel("DosjaVlerave", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "Excel",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { kategoria, origjina },
                    shenime: "Eksportuar dosja e vlerave në Excel"
                );

                var fileName = $"DosjaVlerave_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(DosjaVlerave), new { kategoria, origjina });
            }
        }

        [HttpGet("DosjaVlerave/PDF")]
        public async Task<IActionResult> DosjaVlerave_PDF(string? kategoria, string? origjina)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = vlerat.Where(v => v.Kategoria == kategoria);
                }

                if (!string.IsNullOrWhiteSpace(origjina))
                {
                    vlerat = vlerat.Where(v => v.Origjina == origjina);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Vlerave" }
                };
                if (!string.IsNullOrEmpty(kategoria))
                    parametra.Add("Kategoria", kategoria);
                if (!string.IsNullOrEmpty(origjina))
                    parametra.Add("Origjina", origjina);

                var pdfData = await _sherbimetPrintimit.GjeneroPDF("DosjaVlerave", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "PDF",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { kategoria, origjina },
                    shenime: "Gjeneruar PDF për dosjen e vlerave"
                );

                var fileName = $"DosjaVlerave_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të PDF: " + ex.Message;
                return RedirectToAction(nameof(DosjaVlerave), new { kategoria, origjina });
            }
        }

        [HttpGet("DosjaVlerave/Print")]
        public async Task<IActionResult> DosjaVlerave_Print(string? kategoria, string? origjina)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = vlerat.Where(v => v.Kategoria == kategoria);
                }

                if (!string.IsNullOrWhiteSpace(origjina))
                {
                    vlerat = vlerat.Where(v => v.Origjina == origjina);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Vlerave" }
                };
                if (!string.IsNullOrEmpty(kategoria))
                    parametra.Add("Kategoria", kategoria);
                if (!string.IsNullOrEmpty(origjina))
                    parametra.Add("Origjina", origjina);

                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("DosjaVlerave", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "Print",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { kategoria, origjina },
                    shenime: "Hapur për printim dosja e vlerave"
                );

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaVlerave",
                    formatiEksportimit: "Print",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë përgatitjes për printim: " + ex.Message;
                return RedirectToAction(nameof(DosjaVlerave), new { kategoria, origjina });
            }
        }

        [HttpGet("DosjaTeDisponueshme")]
        public async Task<IActionResult> DosjaTeDisponueshme()
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                vlerat = vlerat.Where(v => v.Eshte_Aktiv).OrderBy(v => v.Kodi).ThenBy(v => v.KodiProduktit);

                ViewData["DataGjenerimit"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                return View(vlerat);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("DosjaTeDisponueshme/Excel")]
        public async Task<IActionResult> DosjaTeDisponueshme_Excel()
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                vlerat = vlerat.Where(v => v.Eshte_Aktiv).OrderBy(v => v.Kodi).ThenBy(v => v.KodiProduktit);

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Disponueshme" },
                    { "Filtro sipas", "Vetëm produktet aktive" }
                };

                // Gjenero Excel përmes SherbimetPrintimit
                var excelData = await _sherbimetPrintimit.GjeneroExcel("DosjaTeDisponueshme", vlerat, parametra);
                
                // Regjistro në auditim
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "Excel",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { Aktiv = true },
                    shenime: "Eksportuar dosja e disponueshme në Excel"
                );

                var fileName = $"DosjaTeDisponueshme_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(DosjaTeDisponueshme));
            }
        }

        [HttpGet("DosjaTeDisponueshme/PDF")]
        public async Task<IActionResult> DosjaTeDisponueshme_PDF()
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                vlerat = vlerat.Where(v => v.Eshte_Aktiv).OrderBy(v => v.Kodi).ThenBy(v => v.KodiProduktit);

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Disponueshme" },
                    { "Filtro sipas", "Vetëm produktet aktive" }
                };

                // Gjenero PDF përmes SherbimetPrintimit
                var pdfData = await _sherbimetPrintimit.GjeneroPDF("DosjaTeDisponueshme", vlerat, parametra);
                
                // Regjistro në auditim
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "PDF",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { Aktiv = true },
                    shenime: "Gjeneruar PDF për dosjen e disponueshme"
                );

                var fileName = $"DosjaTeDisponueshme_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të PDF: " + ex.Message;
                return RedirectToAction(nameof(DosjaTeDisponueshme));
            }
        }

        [HttpGet("DosjaTeDisponueshme/Print")]
        public async Task<IActionResult> DosjaTeDisponueshme_Print()
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                vlerat = vlerat.Where(v => v.Eshte_Aktiv).OrderBy(v => v.Kodi).ThenBy(v => v.KodiProduktit);

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Dosja e Disponueshme" },
                    { "Filtro sipas", "Vetëm produktet aktive" }
                };

                // Gjenero HTML për printim
                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("DosjaTeDisponueshme", vlerat, parametra);
                
                // Regjistro në auditim
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "Print",
                    numriRekordeve: vlerat.Count(),
                    filtrat: new { Aktiv = true },
                    shenime: "Hapur për printim dosja e disponueshme"
                );

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "DosjaTeDisponueshme",
                    formatiEksportimit: "Print",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë përgatitjes për printim: " + ex.Message;
                return RedirectToAction(nameof(DosjaTeDisponueshme));
            }
        }

        [HttpGet("ListaNdryshimeve")]
        public async Task<IActionResult> ListaNdryshimeve(DateTime? dataFillimit, DateTime? dataMbarimit, string? oficeri)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                var ndryshime = vlerat
                    .SelectMany(v => v.HistoriaVlerave ?? new List<HistoriaVlerave>())
                    .AsEnumerable();

                if (dataFillimit.HasValue)
                {
                    ndryshime = ndryshime.Where(n => n.Ndryshuar_Me >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    ndryshime = ndryshime.Where(n => n.Ndryshuar_Me <= dataMbarimit.Value);
                }

                if (!string.IsNullOrWhiteSpace(oficeri))
                {
                    ndryshime = ndryshime.Where(n => n.Ndryshuar_Nga == oficeri);
                }

                ndryshime = ndryshime.OrderByDescending(n => n.Ndryshuar_Me);

                ViewData["DataFillimit"] = dataFillimit;
                ViewData["DataMbarimit"] = dataMbarimit;
                ViewData["Oficeri"] = oficeri;
                ViewData["DataGjenerimit"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                return View(ndryshime);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("KomentetDegeve")]
        public async Task<IActionResult> KomentetDegeve(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depojaKomenteve.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depojaKomenteve.MerrTeGjitha();
                }

                if (dataFillimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit <= dataMbarimit.Value);
                }

                ViewData["Dega"] = dega;
                ViewData["DataFillimit"] = dataFillimit;
                ViewData["DataMbarimit"] = dataMbarimit;
                ViewData["DataGjenerimit"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                return View(komentet);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("KomentetDegeve/Excel")]
        public async Task<IActionResult> KomentetDegeve_Excel(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depojaKomenteve.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depojaKomenteve.MerrTeGjitha();
                }

                if (dataFillimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit <= dataMbarimit.Value);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Komentet e Degëve" }
                };
                if (!string.IsNullOrEmpty(dega))
                    parametra.Add("Dega", dega);
                if (dataFillimit.HasValue)
                    parametra.Add("Data fillimit", dataFillimit.Value.ToString("dd/MM/yyyy"));
                if (dataMbarimit.HasValue)
                    parametra.Add("Data mbarimit", dataMbarimit.Value.ToString("dd/MM/yyyy"));

                var excelData = await _sherbimetPrintimit.GjeneroExcel("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Excel",
                    numriRekordeve: komentet.Count(),
                    filtrat: new { dega, dataFillimit, dataMbarimit },
                    shenime: "Eksportuar komentet e degëve në Excel"
                );

                var fileName = $"KomentetDegeve_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(KomentetDegeve), new { dega, dataFillimit, dataMbarimit });
            }
        }

        [HttpGet("KomentetDegeve/PDF")]
        public async Task<IActionResult> KomentetDegeve_PDF(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depojaKomenteve.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depojaKomenteve.MerrTeGjitha();
                }

                if (dataFillimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit <= dataMbarimit.Value);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Komentet e Degëve" }
                };
                if (!string.IsNullOrEmpty(dega))
                    parametra.Add("Dega", dega);
                if (dataFillimit.HasValue)
                    parametra.Add("Data fillimit", dataFillimit.Value.ToString("dd/MM/yyyy"));
                if (dataMbarimit.HasValue)
                    parametra.Add("Data mbarimit", dataMbarimit.Value.ToString("dd/MM/yyyy"));

                var pdfData = await _sherbimetPrintimit.GjeneroPDF("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "PDF",
                    numriRekordeve: komentet.Count(),
                    filtrat: new { dega, dataFillimit, dataMbarimit },
                    shenime: "Gjeneruar PDF për komentet e degëve"
                );

                var fileName = $"KomentetDegeve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të PDF: " + ex.Message;
                return RedirectToAction(nameof(KomentetDegeve), new { dega, dataFillimit, dataMbarimit });
            }
        }

        [HttpGet("KomentetDegeve/Print")]
        public async Task<IActionResult> KomentetDegeve_Print(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depojaKomenteve.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depojaKomenteve.MerrTeGjitha();
                }

                if (dataFillimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit <= dataMbarimit.Value);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Komentet e Degëve" }
                };
                if (!string.IsNullOrEmpty(dega))
                    parametra.Add("Dega", dega);
                if (dataFillimit.HasValue)
                    parametra.Add("Data fillimit", dataFillimit.Value.ToString("dd/MM/yyyy"));
                if (dataMbarimit.HasValue)
                    parametra.Add("Data mbarimit", dataMbarimit.Value.ToString("dd/MM/yyyy"));

                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Print",
                    numriRekordeve: komentet.Count(),
                    filtrat: new { dega, dataFillimit, dataMbarimit },
                    shenime: "Hapur për printim komentet e degëve"
                );

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Print",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë përgatitjes për printim: " + ex.Message;
                return RedirectToAction(nameof(KomentetDegeve), new { dega, dataFillimit, dataMbarimit });
            }
        }

        [HttpGet("ShpenzimetTransportit")]
        public async Task<IActionResult> ShpenzimetTransportit(string? llojiTransportit)
        {
            try
            {
                var shpenzimet = await _depojaTransportit.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = shpenzimet.Where(s => s.LlojiTransportit == llojiTransportit);
                }

                ViewData["LlojiTransportit"] = llojiTransportit;
                ViewData["DataGjenerimit"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                return View(shpenzimet);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("ShpenzimetTransportit/Excel")]
        public async Task<IActionResult> ShpenzimetTransportit_Excel(string? llojiTransportit)
        {
            try
            {
                var shpenzimet = await _depojaTransportit.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = shpenzimet.Where(s => s.LlojiTransportit == llojiTransportit);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Shpenzimet e Transportit" }
                };
                if (!string.IsNullOrEmpty(llojiTransportit))
                    parametra.Add("Lloji i transportit", llojiTransportit);

                var excelData = await _sherbimetPrintimit.GjeneroExcel("ShpenzimetTransportit", shpenzimet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "Excel",
                    numriRekordeve: shpenzimet.Count(),
                    filtrat: new { llojiTransportit },
                    shenime: "Eksportuar shpenzimet e transportit në Excel"
                );

                var fileName = $"ShpenzimetTransportit_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(ShpenzimetTransportit), new { llojiTransportit });
            }
        }

        [HttpGet("ShpenzimetTransportit/PDF")]
        public async Task<IActionResult> ShpenzimetTransportit_PDF(string? llojiTransportit)
        {
            try
            {
                var shpenzimet = await _depojaTransportit.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = shpenzimet.Where(s => s.LlojiTransportit == llojiTransportit);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Shpenzimet e Transportit" }
                };
                if (!string.IsNullOrEmpty(llojiTransportit))
                    parametra.Add("Lloji i transportit", llojiTransportit);

                var pdfData = await _sherbimetPrintimit.GjeneroPDF("ShpenzimetTransportit", shpenzimet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "PDF",
                    numriRekordeve: shpenzimet.Count(),
                    filtrat: new { llojiTransportit },
                    shenime: "Gjeneruar PDF për shpenzimet e transportit"
                );

                var fileName = $"ShpenzimetTransportit_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të PDF: " + ex.Message;
                return RedirectToAction(nameof(ShpenzimetTransportit), new { llojiTransportit });
            }
        }

        [HttpGet("ShpenzimetTransportit/Print")]
        public async Task<IActionResult> ShpenzimetTransportit_Print(string? llojiTransportit)
        {
            try
            {
                var shpenzimet = await _depojaTransportit.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = shpenzimet.Where(s => s.LlojiTransportit == llojiTransportit);
                }

                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Shpenzimet e Transportit" }
                };
                if (!string.IsNullOrEmpty(llojiTransportit))
                    parametra.Add("Lloji i transportit", llojiTransportit);

                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("ShpenzimetTransportit", shpenzimet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "Print",
                    numriRekordeve: shpenzimet.Count(),
                    filtrat: new { llojiTransportit },
                    shenime: "Hapur për printim shpenzimet e transportit"
                );

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "ShpenzimetTransportit",
                    formatiEksportimit: "Print",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë përgatitjes për printim: " + ex.Message;
                return RedirectToAction(nameof(ShpenzimetTransportit), new { llojiTransportit });
            }
        }

        [HttpGet("DosjaVlerave/PrintEditor")]
        public async Task<IActionResult> DosjaVlerave_PrintEditor(string? kategoria, string? origjina)
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = vlerat.Where(v => v.Kategoria == kategoria);
                }

                if (!string.IsNullOrWhiteSpace(origjina))
                {
                    vlerat = vlerat.Where(v => v.Origjina == origjina);
                }

                ViewData["Kategoria"] = kategoria;
                ViewData["Origjina"] = origjina;
                ViewData["PrintTitle"] = "Dosja e Vlerave";
                ViewData["PrintType"] = "DosjaVlerave";

                // Generate HTML content
                var content = GenerateDosjaVleraweContent(vlerat, kategoria, origjina);
                ViewData["PrintContent"] = content;

                return View("~/Views/Shared/PrintEditorUniversal.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së editorit të printimit: " + ex.Message;
                return RedirectToAction(nameof(DosjaVlerave), new { kategoria, origjina });
            }
        }

        [HttpGet("DosjaTeDisponueshme/PrintEditor")]
        public async Task<IActionResult> DosjaTeDisponueshme_PrintEditor()
        {
            try
            {
                var vlerat = await _depojaProdukteve.MerrTeGjitha();
                vlerat = vlerat.Where(v => v.Eshte_Aktiv).OrderBy(v => v.Kodi).ThenBy(v => v.KodiProduktit);

                ViewData["PrintTitle"] = "Dosja e Disponueshme";
                ViewData["PrintType"] = "DosjaTeDisponueshme";

                var content = GenerateDosjaVleraweContent(vlerat, null, null);
                ViewData["PrintContent"] = content;

                return View("~/Views/Shared/PrintEditorUniversal.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së editorit të printimit: " + ex.Message;
                return RedirectToAction(nameof(DosjaTeDisponueshme));
            }
        }

        [HttpGet("KomentetDegeve/PrintEditor")]
        public async Task<IActionResult> KomentetDegeve_PrintEditor(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depojaKomenteve.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depojaKomenteve.MerrTeGjitha();
                }

                if (dataFillimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit >= dataFillimit.Value);
                }

                if (dataMbarimit.HasValue)
                {
                    komentet = komentet.Where(k => k.DataDergimit <= dataMbarimit.Value);
                }

                ViewData["Dega"] = dega;
                ViewData["DataFillimit"] = dataFillimit;
                ViewData["DataMbarimit"] = dataMbarimit;
                ViewData["PrintTitle"] = "Komentet e Degeve";
                ViewData["PrintType"] = "KomentetDegeve";

                var content = GenerateKomentetContent(komentet, dega);
                ViewData["PrintContent"] = content;

                return View("~/Views/Shared/PrintEditorUniversal.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së editorit të printimit: " + ex.Message;
                return RedirectToAction(nameof(KomentetDegeve), new { dega, dataFillimit, dataMbarimit });
            }
        }

        [HttpGet("ShpenzimetTransportit/PrintEditor")]
        public async Task<IActionResult> ShpenzimetTransportit_PrintEditor(string? llojiTransportit)
        {
            try
            {
                var shpenzimet = await _depojaTransportit.MerrTeGjitha();

                if (!string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = shpenzimet.Where(s => s.LlojiTransportit == llojiTransportit);
                }

                ViewData["LlojiTransportit"] = llojiTransportit;
                ViewData["PrintTitle"] = "Shpenzimet e Transportit";
                ViewData["PrintType"] = "ShpenzimetTransportit";

                var content = GenerateShpenzimetContent(shpenzimet, llojiTransportit);
                ViewData["PrintContent"] = content;

                return View("~/Views/Shared/PrintEditorUniversal.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së editorit të printimit: " + ex.Message;
                return RedirectToAction(nameof(ShpenzimetTransportit), new { llojiTransportit });
            }
        }

        #region Helper Methods for Universal Print Editor

        private string GenerateDosjaVleraweContent(IEnumerable<VleraProduktit> vlerat, string? kategoria, string? origjina)
        {
            var html = new System.Text.StringBuilder();
            
            // Header
            html.Append(@"
                <div class='document-header' style='text-align: center; border-bottom: 2px solid #000; padding-bottom: 20px; margin-bottom: 30px;'>
                    <img src='/images/dogana-logo.png' alt='Logo' style='max-height: 80px; margin-bottom: 10px;' class='logo' id='mainLogo' />
                    <h1 style='margin: 0; font-size: 24pt; color: #1e3c72;'>REPUBLIKA E KOSOVËS</h1>
                    <h2 style='margin: 5px 0; font-size: 18pt; color: #2a5298;'>DOGANA E KOSOVËS</h2>
                    <h3 style='margin: 10px 0; font-size: 16pt;'>DOSJA E VLERAVE DOGANORE</h3>
                </div>
            ");

            // Info section
            html.Append("<div style='margin-bottom: 20px;'>");
            if (!string.IsNullOrEmpty(kategoria))
                html.Append($"<p><strong>Kategoria:</strong> {kategoria}</p>");
            if (!string.IsNullOrEmpty(origjina))
                html.Append($"<p><strong>Origjina:</strong> {origjina}</p>");
            html.Append($"<p><strong>Data e gjenerimit:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            html.Append($"<p><strong>Totali i produkteve:</strong> {vlerat.Count()}</p>");
            html.Append("</div>");

            // Table
            html.Append(@"
                <table class='table table-striped'>
                    <thead>
                        <tr>
                            <th>Nr.</th>
                            <th>Kodi</th>
                            <th>Emri produktit</th>
                            <th>Përshkrimi</th>
                            <th>Vlera</th>
                            <th>Njësia</th>
                            <th>Origjina</th>
                            <th>Kategoria</th>
                        </tr>
                    </thead>
                    <tbody>
            ");

            int nr = 1;
            foreach (var vlera in vlerat.OrderBy(v => v.KodiProduktit))
            {
                html.Append($@"
                    <tr>
                        <td>{nr}</td>
                        <td><strong>{vlera.KodiProduktit}</strong></td>
                        <td>{vlera.EmriProduktit}</td>
                        <td>{(vlera.Pershkrimi.Length > 50 ? vlera.Pershkrimi.Substring(0, 50) + "..." : vlera.Pershkrimi)}</td>
                        <td>{vlera.VleraDoganore:N2} {vlera.Valuta}</td>
                        <td>{vlera.Njesia}</td>
                        <td>{vlera.Origjina}</td>
                        <td>{vlera.Kategoria}</td>
                    </tr>
                ");
                nr++;
            }

            html.Append(@"
                    </tbody>
                </table>
            ");

            // Footer
            html.Append(@"
                <div class='footer-info' style='margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; font-size: 10pt; color: #666;'>
                    <p><strong>Dogana e Republikës së Kosovës</strong></p>
                    <p>Rruga ""Bill Clinton"" p.n., 10000 Prishtinë, Kosovë</p>
                    <p>Tel: +383 38 200 33 000 | Email: info@dogana.rks-gov.net</p>
                </div>
            ");

            return html.ToString();
        }

        private string GenerateKomentetContent(IEnumerable<KomentiDeges> komentet, string? dega)
        {
            var html = new System.Text.StringBuilder();
            
            html.Append(@"
                <div class='document-header' style='text-align: center; border-bottom: 2px solid #000; padding-bottom: 20px; margin-bottom: 30px;'>
                    <img src='/images/dogana-logo.png' alt='Logo' style='max-height: 80px; margin-bottom: 10px;' class='logo' id='mainLogo' />
                    <h1 style='margin: 0; font-size: 24pt; color: #1e3c72;'>REPUBLIKA E KOSOVËS</h1>
                    <h2 style='margin: 5px 0; font-size: 18pt; color: #2a5298;'>DOGANA E KOSOVËS</h2>
                    <h3 style='margin: 10px 0; font-size: 16pt;'>RAPORTI I KOMENTEVE TË DEGËVE</h3>
                </div>
            ");

            html.Append("<div style='margin-bottom: 20px;'>");
            if (!string.IsNullOrEmpty(dega))
                html.Append($"<p><strong>Dega:</strong> {dega}</p>");
            html.Append($"<p><strong>Data e gjenerimit:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            html.Append($"<p><strong>Totali i komenteve:</strong> {komentet.Count()}</p>");
            html.Append("</div>");

            html.Append(@"
                <table class='table table-striped'>
                    <thead>
                        <tr>
                            <th>Nr.</th>
                            <th>Dega</th>
                            <th>Kodi Tarifar</th>
                            <th>Mesazhi</th>
                            <th>Data</th>
                            <th>Statusi</th>
                        </tr>
                    </thead>
                    <tbody>
            ");

            int nr = 1;
            foreach (var koment in komentet.OrderByDescending(k => k.DataDergimit))
            {
                var statusi = koment.EshteZgjidhur ? "Zgjidhur" : "Në pritje";
                html.Append($@"
                    <tr>
                        <td>{nr}</td>
                        <td>{koment.EmriDeges}</td>
                        <td><strong>{koment.KodiTarifar}</strong></td>
                        <td>{(koment.Mesazhi.Length > 100 ? koment.Mesazhi.Substring(0, 100) + "..." : koment.Mesazhi)}</td>
                        <td>{koment.DataDergimit:dd/MM/yyyy}</td>
                        <td>{statusi}</td>
                    </tr>
                ");
                nr++;
            }

            html.Append(@"
                    </tbody>
                </table>
                <div class='footer-info' style='margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; font-size: 10pt; color: #666;'>
                    <p><strong>Dogana e Republikës së Kosovës</strong></p>
                    <p>Rruga ""Bill Clinton"" p.n., 10000 Prishtinë, Kosovë</p>
                    <p>Tel: +383 38 200 33 000 | Email: info@dogana.rks-gov.net</p>
                </div>
            ");

            return html.ToString();
        }

        private string GenerateShpenzimetContent(IEnumerable<ShpenzimiTransportit> shpenzimet, string? llojiTransportit)
        {
            var html = new System.Text.StringBuilder();
            
            html.Append(@"
                <div class='document-header' style='text-align: center; border-bottom: 2px solid #000; padding-bottom: 20px; margin-bottom: 30px;'>
                    <img src='/images/dogana-logo.png' alt='Logo' style='max-height: 80px; margin-bottom: 10px;' class='logo' id='mainLogo' />
                    <h1 style='margin: 0; font-size: 24pt; color: #1e3c72;'>REPUBLIKA E KOSOVËS</h1>
                    <h2 style='margin: 5px 0; font-size: 18pt; color: #2a5298;'>DOGANA E KOSOVËS</h2>
                    <h3 style='margin: 10px 0; font-size: 16pt;'>RAPORTI I SHPENZIMEVE TË TRANSPORTIT</h3>
                </div>
            ");

            html.Append("<div style='margin-bottom: 20px;'>");
            if (!string.IsNullOrEmpty(llojiTransportit))
                html.Append($"<p><strong>Lloji i transportit:</strong> {llojiTransportit}</p>");
            html.Append($"<p><strong>Data e gjenerimit:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            html.Append($"<p><strong>Totali i shpenzimeve:</strong> {shpenzimet.Count()}</p>");
            html.Append("</div>");

            html.Append(@"
                <table class='table table-striped'>
                    <thead>
                        <tr>
                            <th>Nr.</th>
                            <th>Origjina</th>
                            <th>Destinacioni</th>
                            <th>Lloji</th>
                            <th>Çmimi/Njësi</th>
                            <th>Njësia</th>
                        </tr>
                    </thead>
                    <tbody>
            ");

            int nr = 1;
            foreach (var shpenzim in shpenzimet.OrderBy(s => s.VendiOrigjines))
            {
                html.Append($@"
                    <tr>
                        <td>{nr}</td>
                        <td>{shpenzim.VendiOrigjines}</td>
                        <td>{shpenzim.VendiDestinacionit}</td>
                        <td>{shpenzim.LlojiTransportit}</td>
                        <td>{shpenzim.CmimiPerNjesi:N2} {shpenzim.Valuta}</td>
                        <td>{shpenzim.NjesiaMatese}</td>
                    </tr>
                ");
                nr++;
            }

            html.Append(@"
                    </tbody>
                </table>
                <div class='footer-info' style='margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; font-size: 10pt; color: #666;'>
                    <p><strong>Dogana e Republikës së Kosovës</strong></p>
                    <p>Rruga ""Bill Clinton"" p.n., 10000 Prishtinë, Kosovë</p>
                    <p>Tel: +383 38 200 33 000 | Email: info@dogana.rks-gov.net</p>
                </div>
            ");

            return html.ToString();
        }

        #endregion
    }
}
