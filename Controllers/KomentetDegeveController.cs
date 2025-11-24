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

        public KomentetDegeveController(IDepoja_KomentiDeges depoja, IDepoja_VleraProduktit depojaProdukteve, SherbimetAuditimit auditimi)
        {
            _depoja = depoja;
            _depojaProdukteve = depojaProdukteve;
            _auditimi = auditimi;
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

        [HttpGet("Krijo")]
        public IActionResult Krijo()
        {
            return View();
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

                var sukses = await _depoja.ShënosiZgjidhur(id, model.Pergjigja, User.Identity?.Name ?? "Anonim");
                
                if (sukses)
                {
                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Pergjigju",
                        "KomentiDeges",
                        id.ToString(),
                        detajet: "Komenti u përgjigjur"
                    );

                    TempData["Sukses"] = "Përgjigjja u dërgua me sukses!";
                }
                else
                {
                    TempData["Gabim"] = "Komenti nuk u gjet.";
                }

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

                return View(komenti);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përgatitjes së printimit: " + ex.Message;
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
