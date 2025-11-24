using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Sherbime;

namespace KosovaDoganaModerne.Controllers
{
    [Route("ShpenzimetTransportit")]
    public class ShpenzimetTransportitController : Controller
    {
        private readonly IDepoja_ShpenzimiTransportit _depoja;
        private readonly SherbimetAuditimit _auditimi;

        public ShpenzimetTransportitController(IDepoja_ShpenzimiTransportit depoja, SherbimetAuditimit auditimi)
        {
            _depoja = depoja;
            _auditimi = auditimi;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? vendiOrigjines, string? vendiDestinacionit, string? llojiTransportit)
        {
            try
            {
                IEnumerable<ShpenzimiTransportit> shpenzimet;

                if (!string.IsNullOrWhiteSpace(vendiOrigjines) || !string.IsNullOrWhiteSpace(vendiDestinacionit) || !string.IsNullOrWhiteSpace(llojiTransportit))
                {
                    shpenzimet = await _depoja.Kerko(vendiOrigjines, vendiDestinacionit, llojiTransportit);
                }
                else
                {
                    shpenzimet = await _depoja.MerrTeGjitha();
                }

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "ShikoListen",
                    "ShpenzimiTransportit",
                    detajet: $"Origjina: {vendiOrigjines}, Destinacioni: {vendiDestinacionit}, Lloji: {llojiTransportit}"
                );

                ViewData["VendiOrigjines"] = vendiOrigjines;
                ViewData["VendiDestinacionit"] = vendiDestinacionit;
                ViewData["LlojiTransportit"] = llojiTransportit;
                return View(shpenzimet);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të listës: " + ex.Message;
                return View(new List<ShpenzimiTransportit>());
            }
        }

        [HttpGet("Detajet/{id}")]
        public async Task<IActionResult> Detajet(int id)
        {
            try
            {
                var shpenzimi = await _depoja.MerrSipasID(id);
                if (shpenzimi == null)
                {
                    TempData["Gabim"] = "Shpenzimi nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                var historia = await _depoja.MerrHistorine(id);
                ViewBag.Historia = historia;

                return View(shpenzimi);
            }
            catch (Exception ex)
            {
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
        public async Task<IActionResult> Krijo(ShpenzimiTransportit shpenzimi)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    shpenzimi.KrijuarNga = User.Identity?.Name ?? "Sistem";
                    shpenzimi.DataKrijimit = DateTime.UtcNow;

                    var shpenzimiIKrijuar = await _depoja.Krijo(shpenzimi);

                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Krijo",
                        "ShpenzimiTransportit",
                        shpenzimiIKrijuar.Id.ToString(),
                        vlerat_reja: shpenzimi
                    );

                    TempData["Sukses"] = $"Shpenzimi i transportit u krijua me sukses!";
                    return RedirectToAction(nameof(Detajet), new { id = shpenzimiIKrijuar.Id });
                }

                return View(shpenzimi);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë krijimit të shpenzimit: " + ex.Message;
                return View(shpenzimi);
            }
        }

        [HttpGet("Perditeso/{id}")]
        public async Task<IActionResult> Perditeso(int id)
        {
            try
            {
                var shpenzimi = await _depoja.MerrSipasID(id);
                if (shpenzimi == null)
                {
                    TempData["Gabim"] = "Shpenzimi nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                return View(shpenzimi);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të shpenzimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Perditeso/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perditeso(int id, ShpenzimiTransportit shpenzimi)
        {
            if (id != shpenzimi.Id)
            {
                TempData["Gabim"] = "ID e shpenzimit nuk përputhet.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var shpenzimiVjeter = await _depoja.MerrSipasID_PaTracking(id);
                    if (shpenzimiVjeter == null)
                    {
                        TempData["Gabim"] = "Shpenzimi nuk u gjet.";
                        return RedirectToAction(nameof(Index));
                    }

                    if (shpenzimiVjeter.CmimiPerNjesi != shpenzimi.CmimiPerNjesi)
                    {
                        var historia = new NdryshimiTransportit
                        {
                            ShpenzimiTransportit_Id = id,
                            Cmimi_Mepar = shpenzimiVjeter.CmimiPerNjesi,
                            Cmimi_Ri = shpenzimi.CmimiPerNjesi,
                            Valuta_Mepar = shpenzimiVjeter.Valuta,
                            Valuta_Re = shpenzimi.Valuta,
                            ArsyejaE_Ndryshimit = "Përditësim manual",
                            Ndryshuar_Nga = User.Identity?.Name ?? "Anonim",
                            AdresaIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            NumriVersionit = (shpenzimiVjeter.Ndryshimet?.Count() ?? 0) + 1
                        };
                        await _depoja.ShtoNeHistori(historia);
                    }

                    shpenzimi.PerditesoPrejNga = User.Identity?.Name ?? "Anonim";
                    shpenzimi.DataPerditesimit = DateTime.UtcNow;
                    shpenzimi.DataKrijimit = shpenzimiVjeter.DataKrijimit;
                    shpenzimi.KrijuarNga = shpenzimiVjeter.KrijuarNga;

                    await _depoja.Perditeso(shpenzimi);

                    await _auditimi.RegjistroVeprim(
                        User.Identity?.Name ?? "Anonim",
                        "Perditeso",
                        "ShpenzimiTransportit",
                        id.ToString(),
                        vlerat_vjetra: shpenzimiVjeter,
                        vlerat_reja: shpenzimi
                    );

                    TempData["Sukses"] = $"Shpenzimi i transportit u përditësua me sukses!";
                    return RedirectToAction(nameof(Detajet), new { id = shpenzimi.Id });
                }

                return View(shpenzimi);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë përditësimit të shpenzimit: " + ex.Message;
                return View(shpenzimi);
            }
        }

        [HttpGet("Fshi/{id}")]
        public async Task<IActionResult> Fshi(int id)
        {
            try
            {
                var shpenzimi = await _depoja.MerrSipasID(id);
                if (shpenzimi == null)
                {
                    TempData["Gabim"] = "Shpenzimi nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                return View(shpenzimi);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë ngarkimit të shpenzimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Fshi/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KonfirmoFshirjen(int id)
        {
            try
            {
                var shpenzimi = await _depoja.MerrSipasID(id);
                if (shpenzimi == null)
                {
                    TempData["Gabim"] = "Shpenzimi nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                await _depoja.Fshi(id);

                await _auditimi.RegjistroVeprim(
                    User.Identity?.Name ?? "Anonim",
                    "Fshi",
                    "ShpenzimiTransportit",
                    id.ToString(),
                    detajet: $"U fshi shpenzimi: {shpenzimi.VendiOrigjines} - {shpenzimi.VendiDestinacionit}"
                );

                TempData["Sukses"] = "Shpenzimi u fshi me sukses!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë fshirjes së shpenzimit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
