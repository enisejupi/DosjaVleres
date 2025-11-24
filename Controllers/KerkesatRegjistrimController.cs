using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/KerkesatRegjistrim")]
    public class KerkesatRegjistrimController : Controller
    {
        private readonly IDepoja_KerkeseRegjistrim _depojaKerkesave;
        private readonly UserManager<Perdoruesi> _userManager;
        private readonly ILogger<KerkesatRegjistrimController> _logger;

        public KerkesatRegjistrimController(
            IDepoja_KerkeseRegjistrim depojaKerkesave,
            UserManager<Perdoruesi> userManager,
            ILogger<KerkesatRegjistrimController> logger)
        {
            _depojaKerkesave = depojaKerkesave;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var kerkesatNePritje = await _depojaKerkesave.MerrSipasStatusit(StatusiKerkeses.NePritje);
            return View(kerkesatNePritje);
        }

        [HttpGet("TeGjitha")]
        public async Task<IActionResult> TeGjitha()
        {
            var kerkesatTeGjitha = await _depojaKerkesave.MerrTeGjitha();
            return View(kerkesatTeGjitha);
        }

        [HttpGet("Detajet/{id}")]
        public async Task<IActionResult> Detajet(int id)
        {
            var kerkese = await _depojaKerkesave.MerrSipasId(id);
            if (kerkese == null)
            {
                TempData["Gabim"] = "Kërkesa nuk u gjet.";
                return RedirectToAction(nameof(Index));
            }

            return View(kerkese);
        }

        [HttpPost("Aprovo/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprovo(int id)
        {
            try
            {
                var kerkese = await _depojaKerkesave.MerrSipasId(id);
                if (kerkese == null)
                {
                    TempData["Gabim"] = "Kërkesa nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                // Krijo përdoruesin
                var perdoruesi = new Perdoruesi
                {
                    UserName = kerkese.ADUsername ?? kerkese.Email,
                    Email = kerkese.Email,
                    EmriPlote = kerkese.EmriPlote,
                    Departamenti = kerkese.Departamenti,
                    Pozicioni = kerkese.Pozicioni,
                    KodiZyrtarit = kerkese.KodiZyrtarit,
                    Dega_Id = kerkese.Dega_Id,
                    EmailConfirmed = true,
                    EshteAktiv = true,
                    DataKrijimit = DateTime.UtcNow
                };

                // Krijo përdoruesin pa password (do të autentifikohet përmes AD)
                var result = await _userManager.CreateAsync(perdoruesi);

                if (result.Succeeded)
                {
                    // Shto rolin Zyrtar si default
                    await _userManager.AddToRoleAsync(perdoruesi, RoletSistemit.Zyrtar);

                    // Përditëso kërkesën
                    kerkese.Statusi = StatusiKerkeses.Aprovuar;
                    kerkese.DataShqyrtimit = DateTime.UtcNow;
                    kerkese.ShqyrtuesId = User.Identity?.Name;
                    await _depojaKerkesave.Perditeso(kerkese);

                    TempData["Sukses"] = $"Përdoruesi {perdoruesi.Email} u krijua me sukses!";
                    _logger.LogInformation("Admin {Admin} aprovoi kërkesën e regjistrimit për {Email}", User.Identity?.Name, kerkese.Email);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["Gabim"] = $"Gabim gjatë krijimit të përdoruesit: {errors}";
                    _logger.LogError("Gabim gjatë aprovimit të kërkesës {Id}: {Errors}", id, errors);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë aprovimit të kërkesës.";
                _logger.LogError(ex, "Gabim gjatë aprovimit të kërkesës {Id}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Refuzo/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuzo(int id, string arsyeRefuzimi)
        {
            try
            {
                var kerkese = await _depojaKerkesave.MerrSipasId(id);
                if (kerkese == null)
                {
                    TempData["Gabim"] = "Kërkesa nuk u gjet.";
                    return RedirectToAction(nameof(Index));
                }

                kerkese.Statusi = StatusiKerkeses.Refuzuar;
                kerkese.DataShqyrtimit = DateTime.UtcNow;
                kerkese.ShqyrtuesId = User.Identity?.Name;
                kerkese.ArsetimiRefuzimit = arsyeRefuzimi;

                await _depojaKerkesave.Perditeso(kerkese);

                TempData["Sukses"] = "Kërkesa u refuzua.";
                _logger.LogInformation("Admin {Admin} refuzoi kërkesën e regjistrimit për {Email}", User.Identity?.Name, kerkese.Email);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë refuzimit të kërkesës.";
                _logger.LogError(ex, "Gabim gjatë refuzimit të kërkesës {Id}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Fshi/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fshi(int id)
        {
            try
            {
                await _depojaKerkesave.Fshi(id);
                TempData["Sukses"] = "Kërkesa u fshi me sukses.";
                return RedirectToAction(nameof(TeGjitha));
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë fshirjes së kërkesës.";
                _logger.LogError(ex, "Gabim gjatë fshirjes së kërkesës {Id}", id);
                return RedirectToAction(nameof(TeGjitha));
            }
        }
    }
}
