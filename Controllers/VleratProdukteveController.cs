using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Sherbime;

namespace KosovaDoganaModerne.Controllers
{
    [Route("VleratProdukteve")]
    public class VleratProdukteveController : Controller
    {
        private readonly IDepoja_VleraProduktit _depoja;
        private readonly IDepoja_KomentiDeges _depojaKomenteve;
        private readonly SherbimetAuditimit _auditimi;
        private readonly SherbimetPrintimit _sherbimetPrintimit;
        private readonly IWebHostEnvironment _environment;

        public VleratProdukteveController(
            IDepoja_VleraProduktit depoja, 
            IDepoja_KomentiDeges depojaKomenteve, 
            SherbimetAuditimit auditimi,
            SherbimetPrintimit sherbimetPrintimit,
            IWebHostEnvironment environment)
        {
            _depoja = depoja;
            _depojaKomenteve = depojaKomenteve;
            _auditimi = auditimi;
            _sherbimetPrintimit = sherbimetPrintimit;
            _environment = environment;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? kerko, string? kategoria, string? llojiKerkimit)
        {
            try
            {
                IEnumerable<VleraProduktit> vlerat;

                if (!string.IsNullOrWhiteSpace(kerko) || !string.IsNullOrWhiteSpace(kategoria))
                {
                    vlerat = await _depoja.Kerko(kerko, kategoria, llojiKerkimit, vetemAktive: true);
                }
                else
                {
                    vlerat = await _depoja.MerrTeGjitha();
                }

                // Merr komentet për çdo produkt
                var komentetPerProdukt = new Dictionary<int, List<KomentiDeges>>();
                foreach (var vlera in vlerat)
                {
                    var komentet = await _depojaKomenteve.MerrSipasVleresProduktit(vlera.Id);
                    komentetPerProdukt[vlera.Id] = komentet.ToList();
                }
                ViewBag.KomentetPerProdukt = komentetPerProdukt;

                // Regjistro shikimin e listës në audit
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoListen",
                    "VleraProduktit",
                    detajet: $"Kërkimi: {kerko}, Kategoria: {kategoria}, Lloji: {llojiKerkimit}"
                );

                ViewData["Kerko"] = kerko;
                ViewData["Kategoria"] = kategoria;
                ViewData["LlojiKerkimit"] = llojiKerkimit ?? "cdoFjale";
                return View(vlerat);
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoListen",
                    "VleraProduktit",
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë ngarkimit të listës: " + ex.Message;
                return View(new List<VleraProduktit>());
            }
        }

        [HttpGet("Detajet/{id}")]
        public async Task<IActionResult> Detajet(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Merr historinë e ndryshimeve
                var historia = await _depoja.MerrHistorine(id);
                ViewBag.Historia = historia;

                // Regjistro shikimin e detajeve në audit
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoDetajet",
                    "VleraProduktit",
                    id.ToString()
                );

                return View(vlera);
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoDetajet",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë ngarkimit të detajeve: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Krijo")]
        public IActionResult Krijo()
        {
            return View();
        }

        [HttpPost("Krijo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Krijo(VleraProduktit vlera, IFormFile? bashkangjitja)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle file upload
                    if (bashkangjitja != null && bashkangjitja.Length > 0)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(bashkangjitja.FileName).ToLowerInvariant();
                        
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Create uploads directory if it doesn't exist
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "bashkangjitje");
                            Directory.CreateDirectory(uploadsFolder);

                            // Generate unique filename
                            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Save file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await bashkangjitja.CopyToAsync(fileStream);
                            }

                            vlera.Bashkangjitje = $"/uploads/bashkangjitje/{uniqueFileName}";
                            vlera.EmeriBashkangjitjes = bashkangjitja.FileName;
                        }
                    }

                    // Vendos të dhënat e krijimit
                    vlera.Krijuar_Nga = User.Identity?.Name ?? "Anonim";
                    vlera.Krijuar_Me = DateTime.UtcNow;

                    var vleraEKrijuar = await _depoja.Krijo(vlera);

                    // Regjistro krijimin në audit
                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Krijo",
                        "VleraProduktit",
                        vleraEKrijuar.Id.ToString(),
                        vlerat_reja: vlera,
                        detajet: $"U krijua vlera e produktit: {vlera.Pershkrimi}"
                    );

                    TempData["Sukses"] = $"Vlera e produktit '{vlera.Pershkrimi}' u krijua me sukses!";
                    return RedirectToAction(nameof(Detajet), new { id = vleraEKrijuar.Id });
                }

                return View(vlera);
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Krijo",
                    "VleraProduktit",
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë krijimit të produktit: " + ex.Message;
                return View(vlera);
            }
        }

        [HttpGet("Perditeso/{id}")]
        public async Task<IActionResult> Perditeso(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                return View(vlera);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të produktit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Përditëso një produkt ekzistues
        /// </summary>
        [HttpPost("Perditeso/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perditeso(int id, VleraProduktit vlera, IFormFile? fotoNdryshimit, string? arsyejaNdryshimit, IFormFile? bashkangjitja)
        {
            if (id != vlera.Id)
            {
                TempData["Gabim"] = "ID e produktit nuk përputhet.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Merr vlerën e vjetër për historinë (pa tracking)
                    var vleraVjeter = await _depoja.MerrSipasID_PaTracking(id);
                    if (vleraVjeter == null)
                    {
                        TempData["Gabim"] = "Produkti nuk u gjet.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Handle attachment upload if provided
                    if (bashkangjitja != null && bashkangjitja.Length > 0)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(bashkangjitja.FileName).ToLowerInvariant();
                        
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Create uploads directory if it doesn't exist
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "bashkangjitje");
                            Directory.CreateDirectory(uploadsFolder);

                            // Generate unique filename
                            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Save file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await bashkangjitja.CopyToAsync(fileStream);
                            }

                            vlera.Bashkangjitje = $"/uploads/bashkangjitje/{uniqueFileName}";
                            vlera.EmeriBashkangjitjes = bashkangjitja.FileName;
                        }
                    }
                    else
                    {
                        // Keep existing attachment if no new file uploaded
                        vlera.Bashkangjitje = vleraVjeter.Bashkangjitje;
                        vlera.EmeriBashkangjitjes = vleraVjeter.EmeriBashkangjitjes;
                    }

                    // Kontrollo nëse vlera doganore është ndryshuar
                    if (vleraVjeter.VleraDoganore != vlera.VleraDoganore)
                    {
                        string? fotoPath = null;

                        // Handle photo upload if provided
                        if (fotoNdryshimit != null && fotoNdryshimit.Length > 0)
                        {
                            // Validate file type
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                            var fileExtension = Path.GetExtension(fotoNdryshimit.FileName).ToLowerInvariant();
                            
                            if (allowedExtensions.Contains(fileExtension))
                            {
                                // Create uploads directory if it doesn't exist
                                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "historia");
                                Directory.CreateDirectory(uploadsFolder);

                                // Generate unique filename
                                var uniqueFileName = $"{id}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{fileExtension}";
                                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                                // Save file
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await fotoNdryshimit.CopyToAsync(fileStream);
                                }

                                fotoPath = $"/uploads/historia/{uniqueFileName}";
                            }
                        }

                        // Shto në historinë e ndryshimeve
                        var historia = new HistoriaVlerave
                        {
                            VleraProduktit_Id = id,
                            Vlera_Mepar = vleraVjeter.VleraDoganore,
                            Vlera_Re = vlera.VleraDoganore,
                            Valuta_Mepar = vleraVjeter.Valuta,
                            Valuta_Re = vlera.Valuta,
                            ArsyejaE_Ndryshimit = !string.IsNullOrWhiteSpace(arsyejaNdryshimit) ? arsyejaNdryshimit : "Përditësim manual",
                            FotoNdryshimit = fotoPath,
                            Ndryshuar_Nga = User.Identity?.Name ?? "Anonim",
                            AdresaIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            NumriVersionit = (vleraVjeter.HistoriaVlerave?.Count() ?? 0) + 1
                        };
                        await _depoja.ShtoNeHistori(historia);
                    }

                    // Përditëso produktin
                    vlera.Modifikuar_Nga = User.Identity?.Name ?? "Anonim";
                    vlera.Modifikuar_Me = DateTime.UtcNow;
                    vlera.Krijuar_Me = vleraVjeter.Krijuar_Me;
                    vlera.Krijuar_Nga = vleraVjeter.Krijuar_Nga;

                    await _depoja.Perditeso(vlera);

                    // Regjistro përditësimin në audit
                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Perditeso",
                        "VleraProduktit",
                        id.ToString(),
                        vlerat_vjetra: vleraVjeter,
                        vlerat_reja: vlera,
                        detajet: $"U përditësua produkti: {vlera.EmriProduktit}"
                    );

                    TempData["Sukses"] = $"Produkti '{vlera.EmriProduktit}' u përditësua me sukses!";
                    return RedirectToAction(nameof(Detajet), new { id = vlera.Id });
                }

                return View(vlera);
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Perditeso",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë përditësimit të produktit: " + ex.Message;
                return View(vlera);
            }
        }

        [HttpGet("Fshi/{id}")]
        public async Task<IActionResult> Fshi(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                return View(vlera);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të produktit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Fshi/{id}")]
        [ActionName("Fshi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KonfirmoFshirjen(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var emriProduktit = vlera.EmriProduktit;
                var sukses = await _depoja.Fshi(id);

                if (sukses)
                {
                    // Regjistro fshirjen në audit
                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Fshi",
                        "VleraProduktit",
                        id.ToString(),
                        vlerat_vjetra: vlera,
                        detajet: $"U fshi produkti: {emriProduktit}"
                    );

                    TempData["Sukses"] = $"Produkti '{emriProduktit}' u fshi me sukses!";
                }
                else
                {
                    TempData["Gabim"] = "Nuk u arrit të fshihet produkti.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Fshi",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë fshirjes së produktit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Eksporto")]
        public async Task<IActionResult> Eksporto()
        {
            try
            {
                var vlerat = await _depoja.MerrTeGjitha();
                var csv = "ID,Kodi,Përshkrimi,Origjina,Pariteti,Njësia,Çmimi,Valuta,Kategoria,Komentet\n";

                foreach (var vlera in vlerat)
                {
                    csv += $"{vlera.Id},\"{vlera.Kodi}\",\"{vlera.Pershkrimi}\",\"{vlera.Origjina}\",";
                    csv += $"\"{vlera.Pariteti}\",\"{vlera.Njesia}\",\"{vlera.Cmimi}\",";
                    csv += $"\"{vlera.Valuta}\",\"{vlera.Kategoria}\",\"{vlera.Komentet}\"\n";
                }

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Eksporto",
                    "VleraProduktit",
                    detajet: $"Eksportuar {vlerat.Count()} produkte"
                );

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"VleratProdukteve_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë eksportimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Print/{id}")]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var vlerat = new List<VleraProduktit> { vlera };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Produktit" },
                    { "Kodi i produktit", vlera.KodiProduktit },
                    { "Emri i produktit", vlera.EmriProduktit }
                };

                var htmlContent = await _sherbimetPrintimit.GjeneroPërmbajtjeHTML("VleratProdukteve", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
                    formatiEksportimit: "Print",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Printuar detajet për produktin: {vlera.EmriProduktit}"
                );

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
                    formatiEksportimit: "Print",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë printimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("PDF/{id}")]
        public async Task<IActionResult> PDF(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var vlerat = new List<VleraProduktit> { vlera };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Produktit" },
                    { "Kodi i produktit", vlera.KodiProduktit },
                    { "Emri i produktit", vlera.EmriProduktit }
                };

                var pdfData = await _sherbimetPrintimit.GjeneroPDF("VleratProdukteve", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
                    formatiEksportimit: "PDF",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Gjeneruar PDF për produktin: {vlera.EmriProduktit}"
                );

                var fileName = $"Produkti_{vlera.KodiProduktit}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
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
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var vlerat = new List<VleraProduktit> { vlera };
                var parametra = new Dictionary<string, string>
                {
                    { "Lloji i raportit", "Detajet e Produktit" },
                    { "Kodi i produktit", vlera.KodiProduktit },
                    { "Emri i produktit", vlera.EmriProduktit }
                };

                var excelData = await _sherbimetPrintimit.GjeneroExcel("VleratProdukteve", vlerat, parametra);
                
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 1,
                    filtrat: new { Id = id },
                    shenime: $"Eksportuar në Excel produktin: {vlera.EmriProduktit}"
                );

                var fileName = $"Produkti_{vlera.KodiProduktit}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                await _sherbimetPrintimit.RegjistroAuditimPrintimi(
                    llojiRaportit: "VleratProdukteve",
                    formatiEksportimit: "Excel",
                    numriRekordeve: 0,
                    eshteSuksesshem: false,
                    mesazhiGabimit: ex.Message
                );

                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("ShtoKoment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShtoKoment(int id, string komentPerdoruesit, string emriDeges)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Krijo një koment të ri në sistemin e komenteve të degëve
                var komentiDeges = new KomentiDeges
                {
                    EmriDeges = emriDeges,
                    KodiTarifar = vlera.Kodi,
                    Mesazhi = komentPerdoruesit,
                    DataDergimit = DateTime.UtcNow,
                    DergoPrejNga = User.Identity?.Name ?? "Anonim",
                    EshteLexuar = false,
                    EshteZgjidhur = false,
                    VleraProduktit_Id = id
                };

                await _depojaKomenteve.Krijo(komentiDeges);

                // Regjistro në audit
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShtoKoment",
                    "VleraProduktit",
                    id.ToString(),
                    detajet: $"U krijua koment i lidhur me produktin ID: {id}, Kodi: {vlera.Kodi}"
                );

                TempData["Sukses"] = "Komenti u krijua me sukses dhe u dërgua në sistemin e komenteve të degëve!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShtoKoment",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë krijimit të komentit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// API endpoint për të marrë komentet e një produkti (për AJAX)
        /// </summary>
        [HttpGet("/api/komentet/{produktId}")]
        public async Task<IActionResult> MerrKomentet(int produktId)
        {
            try
            {
                var komentet = await _depojaKomenteve.MerrSipasVleresProduktit(produktId);
                
                var result = komentet.Select(k => new
                {
                    id = k.Id,
                    emriDeges = k.EmriDeges,
                    mesazhi = k.Mesazhi,
                    dataDergimit = k.DataDergimit,
                    dergoPrejNga = k.DergoPrejNga,
                    eshteLexuar = k.EshteLexuar,
                    eshteZgjidhur = k.EshteZgjidhur,
                    pergjigja = k.Pergjigja,
                    dataPergjigjes = k.DataPergjigjes,
                    pergjigjetNga = k.PergjigjetNga
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Shfaq historinë e ndryshimeve për një produkt të caktuar
        /// </summary>
        [HttpGet("Historia/{id}")]
        public async Task<IActionResult> Historia(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    TempData["Gabim"] = "Produkti nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Merr historinë e ndryshimeve
                var historia = await _depoja.MerrHistorine(id);

                ViewBag.Produkti = vlera;
                ViewData["EmriProduktit"] = vlera.EmriProduktit;
                ViewData["KodiProduktit"] = vlera.KodiProduktit;

                // Regjistro shikimin e historisë në audit
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoHistorine",
                    "VleraProduktit",
                    id.ToString(),
                    detajet: $"Shikuar historia për produktin: {vlera.EmriProduktit}"
                );

                return View(historia);
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoHistorine",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                TempData["Gabim"] = "Gabim gjatë ngarkimit të historisë: " + ex.Message;
                return RedirectToAction(nameof(Detajet), new { id });
            }
        }

        /// <summary>
        /// Upload product images
        /// </summary>
        [HttpPost("NgarkoImazhe/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NgarkoImazhe(int id, List<IFormFile> imazhet, string? pershkrimi)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                if (vlera == null)
                {
                    return Json(new { success = false, message = "Produkti nuk u gjet." });
                }

                if (imazhet == null || !imazhet.Any())
                {
                    return Json(new { success = false, message = "Nuk ka imazhe për tu ngarkuar." });
                }

                var uploadedImages = new List<ImazhetProduktit>();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var maxFileSize = 5 * 1024 * 1024; // 5MB

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "produkte");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var imazh in imazhet)
                {
                    if (imazh.Length == 0) continue;

                    var fileExtension = Path.GetExtension(imazh.FileName).ToLowerInvariant();
                    
                    // Validate file type
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = $"Formati i skedarit '{imazh.FileName}' nuk është i lejuar. Lejohen vetëm: {string.Join(", ", allowedExtensions)}" });
                    }

                    // Validate file size
                    if (imazh.Length > maxFileSize)
                    {
                        return Json(new { success = false, message = $"Skedari '{imazh.FileName}' është shumë i madh. Maksimumi: 5MB" });
                    }

                    // Generate unique filename
                    var uniqueFileName = $"{id}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imazh.CopyToAsync(fileStream);
                    }

                    // Create image entity
                    var imazhEntity = new ImazhetProduktit
                    {
                        VleraProduktit_Id = id,
                        ShtegimaImazhit = $"/uploads/produkte/{uniqueFileName}",
                        EmriOrigjinal = imazh.FileName,
                        LlojiImazhit = "Produkt",
                        Pershkrimi = pershkrimi,
                        MadhesiaBytes = imazh.Length,
                        NgarkuarNga = User.Identity?.Name ?? "Anonim",
                        NgarkuarMe = DateTime.UtcNow,
                        RradhaShfaqjes = 0,
                        EshteImazhKryesor = false
                    };

                    uploadedImages.Add(imazhEntity);
                }

                // Save to database
                await _depoja.ShtoImazhe(uploadedImages);

                // Audit log
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "NgarkoImazhe",
                    "VleraProduktit",
                    id.ToString(),
                    detajet: $"Ngarkuar {uploadedImages.Count} imazhe për produktin ID: {id}"
                );

                return Json(new { 
                    success = true, 
                    message = $"{uploadedImages.Count} imazhe u ngarkuan me sukses!",
                    images = uploadedImages.Select(i => new { 
                        id = i.Id,
                        url = i.ShtegimaImazhit,
                        emri = i.EmriOrigjinal
                    })
                });
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "NgarkoImazhe",
                    "VleraProduktit",
                    id.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                return Json(new { success = false, message = "Gabim gjatë ngarkimit të imazheve: " + ex.Message });
            }
        }

        /// <summary>
        /// Download/View product image
        /// </summary>
        [HttpGet("Imazhi/{imazhId}")]
        public async Task<IActionResult> Imazhi(int imazhId)
        {
            try
            {
                var imazh = await _depoja.MerrImazhSipasID(imazhId);
                if (imazh == null)
                {
                    return NotFound("Imazhi nuk u gjet.");
                }

                // Get physical file path
                var filePath = Path.Combine(_environment.WebRootPath, imazh.ShtegimaImazhit.TrimStart('/'));

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Skedari i imazhit nuk ekziston në server.");
                }

                // Determine content type
                var contentType = imazh.ShtegimaImazhit.ToLowerInvariant() switch
                {
                    var s when s.EndsWith(".jpg") || s.EndsWith(".jpeg") => "image/jpeg",
                    var s when s.EndsWith(".png") => "image/png",
                    var s when s.EndsWith(".gif") => "image/gif",
                    var s when s.EndsWith(".bmp") => "image/bmp",
                    _ => "application/octet-stream"
                };

                // Return file
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, contentType, imazh.EmriOrigjinal);
            }
            catch (Exception ex)
            {
                return BadRequest($"Gabim gjatë shkarkimit të imazhit: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete product image
        /// </summary>
        [HttpPost("FshiImazhin/{imazhId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FshiImazhin(int imazhId)
        {
            try
            {
                var imazh = await _depoja.MerrImazhSipasID(imazhId);
                if (imazh == null)
                {
                    return Json(new { success = false, message = "Imazhi nuk u gjet." });
                }

                // Delete physical file
                var filePath = Path.Combine(_environment.WebRootPath, imazh.ShtegimaImazhit.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Delete from database
                await _depoja.FshiImazhin(imazhId);

                // Audit log
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "FshiImazhin",
                    "ImazhetProduktit",
                    imazhId.ToString(),
                    detajet: $"Fshirë imazhi për produktin ID: {imazh.VleraProduktit_Id}"
                );

                return Json(new { success = true, message = "Imazhi u fshi me sukses!" });
            }
            catch (Exception ex)
            {
                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "FshiImazhin",
                    "ImazhetProduktit",
                    imazhId.ToString(),
                    eshte_suksesshem: false,
                    mesazhiGabimit: ex.Message
                );
                return Json(new { success = false, message = "Gabim gjatë fshirjes së imazhit: " + ex.Message });
            }
        }

        /// <summary>
        /// Get all images for a product (API endpoint for AJAX)
        /// </summary>
        [HttpGet("/api/imazhet/{produktId}")]
        public async Task<IActionResult> MerrImazhet(int produktId)
        {
            try
            {
                var imazhet = await _depoja.MerrImazhetProduktit(produktId);
                
                var result = imazhet.Select(i => new
                {
                    id = i.Id,
                    url = i.ShtegimaImazhit,
                    emriOrigjinal = i.EmriOrigjinal,
                    llojiImazhit = i.LlojiImazhit,
                    pershkrimi = i.Pershkrimi,
                    madhesiaBytes = i.MadhesiaBytes,
                    eshteKryesor = i.EshteImazhKryesor,
                    ngarkuarNga = i.NgarkuarNga,
                    ngarkuarMe = i.NgarkuarMe
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Set an image as primary for a product
        /// </summary>
        [HttpPost("VendosImazhKryesor/{imazhId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VendosImazhKryesor(int imazhId)
        {
            try
            {
                var imazh = await _depoja.MerrImazhSipasID(imazhId);
                if (imazh == null)
                {
                    return Json(new { success = false, message = "Imazhi nuk u gjet." });
                }

                await _depoja.VendosImazhKryesor(imazhId, imazh.VleraProduktit_Id);

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "VendosImazhKryesor",
                    "ImazhetProduktit",
                    imazhId.ToString(),
                    detajet: $"Vendosur si imazh kryesor për produktin ID: {imazh.VleraProduktit_Id}"
                );

                return Json(new { success = true, message = "Imazhi u vendos si kryesor me sukses!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gabim: " + ex.Message });
            }
        }
    }
}
