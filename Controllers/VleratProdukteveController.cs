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

        public VleratProdukteveController(IDepoja_VleraProduktit depoja, IDepoja_KomentiDeges depojaKomenteve, SherbimetAuditimit auditimi)
        {
            _depoja = depoja;
            _depojaKomenteve = depojaKomenteve;
            _auditimi = auditimi;
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
        public async Task<IActionResult> Krijo(VleraProduktit vlera)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        public async Task<IActionResult> Perditeso(int id, VleraProduktit vlera)
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

                    // Kontrollo nëse vlera doganore është ndryshuar
                    if (vleraVjeter.VleraDoganore != vlera.VleraDoganore)
                    {
                        // Shto në historinë e ndryshimeve
                        var historia = new HistoriaVlerave
                        {
                            VleraProduktit_Id = id,
                            Vlera_Mepar = vleraVjeter.VleraDoganore,
                            Vlera_Re = vlera.VleraDoganore,
                            Valuta_Mepar = vleraVjeter.Valuta,
                            Valuta_Re = vlera.Valuta,
                            ArsyejaE_Ndryshimit = "Përditësim manual",
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

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Printo",
                    "VleraProduktit",
                    id.ToString(),
                    detajet: $"Printuar dokumenti për produktin ID: {id}"
                );

                return View(vlera);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë printimit: " + ex.Message;
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
    }
}
