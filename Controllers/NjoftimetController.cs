using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Te_dhenat;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Controllers
{
    [Route("Njoftimet")]
    public class NjoftimetController : Controller
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public NjoftimetController(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var perdoruesi = User.Identity?.Name;
            var njoftimet = await _konteksti.Njoftimet
                .Where(n => n.Perdoruesi == null || n.Perdoruesi == perdoruesi)
                .OrderByDescending(n => n.DataKrijimit)
                .ToListAsync();

            return View(njoftimet);
        }

        [HttpGet("GetNjoftimetEfundit")]
        public async Task<IActionResult> GetNjoftimetEfundit()
        {
            var perdoruesi = User.Identity?.Name;
            var njoftimet = await _konteksti.Njoftimet
                .Where(n => n.Perdoruesi == null || n.Perdoruesi == perdoruesi)
                .OrderByDescending(n => n.DataKrijimit)
                .Take(5)
                .Select(n => new
                {
                    id = n.Id,
                    titulli = n.Titulli,
                    pershkrimi = n.Pershkrimi,
                    lloji = n.Lloji,
                    eshteLexuar = n.EshteLexuar,
                    dataKrijimit = n.DataKrijimit,
                    linku = n.Linku,
                    ikona = n.Ikona
                })
                .ToListAsync();

            return Json(njoftimet);
        }

        [HttpPost("Shenjosillexuar/{id}")]
        public async Task<IActionResult> Shenjosillexuar(int id)
        {
            var njoftim = await _konteksti.Njoftimet.FindAsync(id);
            if (njoftim != null)
            {
                njoftim.EshteLexuar = true;
                njoftim.DataLeximit = DateTime.UtcNow;
                await _konteksti.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("ShenjositegjitheNjoftimet")]
        public async Task<IActionResult> ShenjositegjitheNjoftimet()
        {
            var perdoruesi = User.Identity?.Name;
            var njoftimet = await _konteksti.Njoftimet
                .Where(n => !n.EshteLexuar && (n.Perdoruesi == null || n.Perdoruesi == perdoruesi))
                .ToListAsync();

            foreach (var njoftim in njoftimet)
            {
                njoftim.EshteLexuar = true;
                njoftim.DataLeximit = DateTime.UtcNow;
            }

            await _konteksti.SaveChangesAsync();
            TempData["Sukses"] = "Të gjitha njoftimet u shënuan si të lexuara.";
            return RedirectToAction(nameof(Index));
        }
    }
}
