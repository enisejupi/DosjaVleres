using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Sherbime;

namespace KosovaDoganaModerne.Controllers
{
    [Route("KomentetDegeve")]
    public class KomentetDegeveController : Controller
    {
        private readonly IDepoja_KomentiDeges _depoja;
        private readonly IDepoja_VleraProduktit _depojaProdukteve;
        private readonly SherbimetAuditimit _auditimi;
        private readonly SherbimetPrintimit _sherbimetPrintimit;

        public KomentetDegeveController(
            IDepoja_KomentiDeges depoja, 
            IDepoja_VleraProduktit depojaProdukteve, 
            SherbimetAuditimit auditimi,
            SherbimetPrintimit sherbimetPrintimit)
        {
            _depoja = depoja;
            _depojaProdukteve = depojaProdukteve;
            _auditimi = auditimi;
            _sherbimetPrintimit = sherbimetPrintimit;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? dega, string? kodiTarifar, string? statusi)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                
                bool vetemEpalexuara = statusi == "PaLexuar";
                bool vetemEpazgjidhura = statusi == "Pazgjidhur";

                if (vetemEpalexuara)
                {
                    komentet = await _depoja.MerrKomentetEpalexuara();
                }
                else if (vetemEpazgjidhura)
                {
                    komentet = await _depoja.MerrKomentetEpazgjidhura();
                }
                else if (statusi == "Zgjidhur")
                {
                    komentet = await _depoja.MerrTeGjitha();
                    komentet = komentet.Where(k => k.EshteZgjidhur);
                }
                else if (statusi == "Lexuar")
                {
                    komentet = await _depoja.MerrTeGjitha();
                    komentet = komentet.Where(k => k.EshteLexuar);
                }
                else if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depoja.MerrSipasDegës(dega);
                }
                else if (!string.IsNullOrWhiteSpace(kodiTarifar))
                {
                    komentet = await _depoja.MerrSipasKoditTarifor(kodiTarifar);
                }
                else
                {
                    komentet = await _depoja.MerrTeGjitha();
                }

                ViewData["Dega"] = dega;
                ViewData["KodiTarifar"] = kodiTarifar;
                ViewData["Statusi"] = statusi;
                return View(komentet);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të komenteve: " + ex.Message;
                return View(new List<KomentiDeges>());
            }
        }

        [HttpGet("Detajet/{id}")]
        public async Task<IActionResult> Detajet(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Shëno si të lexuar
                if (!komenti.EshteLexuar)
                {
                    await _depoja.ShënosiLexuar(id);
                }

                return View(komenti);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të komentit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Krijo një koment të ri. Kjo metodë duhet të aksesohet vetëm nga tabela e produkteve.
        /// </summary>
        [HttpGet("Krijo")]
        public IActionResult Krijo(int? vleraProduktitId, string? kodiTarifar)
        {
            // Nëse nuk vjen nga tabela e produkteve, ridrejto te lista
            if (!vleraProduktitId.HasValue && string.IsNullOrWhiteSpace(kodiTarifar))
            {
                TempData["Gabim"] = "Komentet mund të shtohen vetëm nga tabela e produkteve.";
                return RedirectToAction("Index", "VleratProdukteve");
            }

            var model = new KomentiDeges();
            if (vleraProduktitId.HasValue)
            {
                model.VleraProduktit_Id = vleraProduktitId.Value;
            }
            if (!string.IsNullOrWhiteSpace(kodiTarifar))
            {
                model.KodiTarifar = kodiTarifar;
            }

            return View(model);
        }

        [HttpPost("Krijo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Krijo(KomentiDeges komenti)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    komenti.DergoPrejNga = User.Identity?.Name ?? "Anonim";
                    komenti.DataDergimit = DateTime.UtcNow;

                    var komentiKrijuar = await _depoja.Krijo(komenti);

                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Krijo",
                        "KomentiDeges",
                        komentiKrijuar.Id.ToString(),
                        vlerat_reja: komenti
                    );

                    TempData["Sukses"] = "Komenti u dërgua me sukses!";
                    
                    // Ridrejto mbrapa te produkti nëse ka ID
                    if (komenti.VleraProduktit_Id.HasValue)
                    {
                        return RedirectToAction("Detajet", "VleratProdukteve", new { id = komenti.VleraProduktit_Id.Value });
                    }
                    
                    return RedirectToAction(nameof(Index));
                }

                return View(komenti);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë krijimit të komentit: " + ex.Message;
                return View(komenti);
            }
        }

        [HttpGet("Pergjigju/{id}")]
        public async Task<IActionResult> Pergjigju(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                return View(komenti);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të komentit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Pergjigju/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pergjigju(int id, KomentiDeges model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Pergjigja))
                {
                    TempData["Gabim"] = "Përgjigjja është e detyrueshme.";
                    var komenti = await _depoja.MerrSipasID(id);
                    return View(komenti);
                }

                // Merr koment ekzistues
                var komentiEkzistues = await _depoja.MerrSipasID(id);
                if (komentiEkzistues == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Update the comment with the reply
                komentiEkzistues.Pergjigja = model.Pergjigja;
                komentiEkzistues.PergjigjetNga = User.Identity?.Name ?? "Anonim";
                komentiEkzistues.DataPergjigjes = DateTime.UtcNow;
                komentiEkzistues.EshteLexuar = true; // Always mark as read when replied
                
                // Only mark as solved if the checkbox was checked
                if (model.EshteZgjidhur)
                {
                    komentiEkzistues.EshteZgjidhur = true;
                }

                await _depoja.Perditeso(komentiEkzistues);

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Pergjigju",
                    "KomentiDeges",
                    id.ToString(),
                    detajet: $"Komenti u përgjigjur. Zgjidhur: {model.EshteZgjidhur}"
                );

                TempData["Sukses"] = model.EshteZgjidhur 
                    ? "Përgjigjja u dërgua me sukses dhe komenti u shënua si i zgjidhur!"
                    : "Përgjigjja u dërgua me sukses! Diskutimi mund të vazhdojë.";

                return RedirectToAction(nameof(Detajet), new { id });
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë dërgimit të përgjigjes: " + ex.Message;
                return RedirectToAction(nameof(Detajet), new { id });
            }
        }

        [HttpPost("Fshi/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fshi(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                await _depoja.Fshi(id);

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Fshi",
                    "KomentiDeges",
                    id.ToString()
                );

                TempData["Sukses"] = "Komenti u fshi me sukses!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë fshirjes së komentit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Print/{id}")]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var komentet = new List<KomentiDeges> { komenti };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Komentit" },
                    { "Dega", komenti.EmriDeges },
                    { "Kodi Tarifar", komenti.KodiTarifar }
                };

                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Print",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Printuar detajet për komentin ID: {id}"
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

                TempData["Gabim"] = "Gabim gjatë përgatitjes së printimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("PDF/{id}")]
        public async Task<IActionResult> PDF(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var komentet = new List<KomentiDeges> { komenti };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Komentit" },
                    { "Dega", komenti.EmriDeges },
                    { "Kodi Tarifar", komenti.KodiTarifar }
                };

                var pdfData = await _sherbimetPrintimit.GjeneroPDF("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Gjeneruar PDF për komentin ID: {id}"
                );

                var fileName = $"Komenti_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Excel/{id}")]
        public async Task<IActionResult> Excel(int id)
        {
            try
            {
                var komenti = await _depoja.MerrSipasID(id);
                if (komenti == null)
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var komentet = new List<KomentiDeges> { komenti };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Komentit" },
                    { "Dega", komenti.EmriDeges },
                    { "Kodi Tarifar", komenti.KodiTarifar }
                };

                var excelData = await _sherbimetPrintimit.GjeneroExcel("KomentetDegeve", komentet, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "KomentetDegeve",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Eksportuar në Excel komentin ID: {id}"
                );

                var fileName = $"Komenti_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("PrintTeGjitha")]
        public async Task<IActionResult> PrintTeGjitha(string? dega, DateTime? dataFillimit, DateTime? dataMbarimit)
        {
            try
            {
                IEnumerable<KomentiDeges> komentet;

                if (!string.IsNullOrWhiteSpace(dega))
                {
                    komentet = await _depoja.MerrSipasDegës(dega);
                }
                else
                {
                    komentet = await _depoja.MerrTeGjitha();
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
                
                return View(komentet);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së raportit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
