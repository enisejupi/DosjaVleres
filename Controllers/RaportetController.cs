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

        public RaportetController(
            IDepoja_VleraProduktit depojaProdukteve, 
            IDepoja_KomentiDeges depojaKomenteve,
            IDepoja_ShpenzimiTransportit depojaTransportit,
            SherbimetRaporteve sherbimetRaporteve)
        {
            _depojaProdukteve = depojaProdukteve;
            _depojaKomenteve = depojaKomenteve;
            _depojaTransportit = depojaTransportit;
            _sherbimetRaporteve = sherbimetRaporteve;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("DosjaVlerave")]
        public async Task<IActionResult> DosjaVlerave(string? kategoria, string? origjina)
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

                var excelData = _sherbimetRaporteve.GjeneroExcelVleratProdukteve(vlerat, kategoria, origjina);
                var fileName = $"DosjaVlerave_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
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

                var excelData = _sherbimetRaporteve.GjeneroExcelVleratProdukteve(vlerat, null, null);
                var fileName = $"DosjaTeDisponueshme_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
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

                var excelData = _sherbimetRaporteve.GjeneroExcelKomentetDegeve(komentet, dega);
                var fileName = $"KomentetDegeve_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
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

                var excelData = _sherbimetRaporteve.GjeneroExcelShpenzimetTransportit(shpenzimet, llojiTransportit);
                var fileName = $"ShpenzimetTransportit_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Gabim"] = "Gabim gjatë gjenerimit të Excel: " + ex.Message;
                return RedirectToAction(nameof(ShpenzimetTransportit), new { llojiTransportit });
            }
        }
    }
}
