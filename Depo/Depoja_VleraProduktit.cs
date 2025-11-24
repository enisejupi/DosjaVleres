using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;

namespace KosovaDoganaModerne.Depo
{
    public class Depoja_VleraProduktit : IDepoja_VleraProduktit
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public Depoja_VleraProduktit(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<VleraProduktit>> MerrTeGjitha()
        {
            return await _konteksti.VleratProdukteve
                .Include(p => p.HistoriaVlerave)
                .OrderBy(p => p.EmriProduktit)
                .ToListAsync();
        }

        public async Task<VleraProduktit?> MerrSipasID(int id)
        {
            return await _konteksti.VleratProdukteve
                .Include(p => p.HistoriaVlerave)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<VleraProduktit?> MerrSipasID_PaTracking(int id)
        {
            return await _konteksti.VleratProdukteve
                .AsNoTracking()
                .Include(p => p.HistoriaVlerave)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<VleraProduktit?> MerrSipasKodit(string kodi)
        {
            return await _konteksti.VleratProdukteve
                .Include(p => p.HistoriaVlerave)
                .FirstOrDefaultAsync(p => p.KodiProduktit == kodi);
        }

        public async Task<IEnumerable<VleraProduktit>> Kerko(string? termi, string? kategoria, string? llojiKerkimit = null, bool vetemAktive = true)
        {
            var query = _konteksti.VleratProdukteve.AsQueryable();

            if (vetemAktive)
            {
                query = query.Where(p => p.Eshte_Aktiv);
            }

            if (!string.IsNullOrWhiteSpace(termi))
            {
                termi = termi.Trim().ToLower();
                
                // Përcakto llojin e kërkimit
                switch (llojiKerkimit?.ToLower())
                {
                    case "frazeesakte":
                        query = query.Where(p =>
                            p.EmriProduktit.ToLower().Contains(termi) ||
                            p.KodiProduktit.ToLower().Contains(termi) ||
                            (p.Pershkrimi != null && p.Pershkrimi.ToLower().Contains(termi)) ||
                            (p.Kodi != null && p.Kodi.ToLower().Contains(termi)) ||
                            (p.Origjina != null && p.Origjina.ToLower().Contains(termi)) ||
                            (p.Kategoria != null && p.Kategoria.ToLower().Contains(termi))
                        );
                        break;

                    case "tgjithafjalet":
                        var allWords = termi.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var word in allWords)
                        {
                            var w = word.Trim().ToLower();
                            query = query.Where(p =>
                                p.EmriProduktit.ToLower().Contains(w) ||
                                p.KodiProduktit.ToLower().Contains(w) ||
                                (p.Pershkrimi != null && p.Pershkrimi.ToLower().Contains(w)) ||
                                (p.Kodi != null && p.Kodi.ToLower().Contains(w)) ||
                                (p.Origjina != null && p.Origjina.ToLower().Contains(w)) ||
                                (p.Kategoria != null && p.Kategoria.ToLower().Contains(w))
                            );
                        }
                        break;

                    case "cdofjale":
                    default:
                        var anyWords = termi.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w.Trim().ToLower())
                            .ToArray();
                        if (anyWords.Length > 0)
                        {
                            query = query.Where(p =>
                                anyWords.Any(w =>
                                    p.EmriProduktit.ToLower().Contains(w) ||
                                    p.KodiProduktit.ToLower().Contains(w) ||
                                    (p.Pershkrimi != null && p.Pershkrimi.ToLower().Contains(w)) ||
                                    (p.Kodi != null && p.Kodi.ToLower().Contains(w)) ||
                                    (p.Origjina != null && p.Origjina.ToLower().Contains(w)) ||
                                    (p.Kategoria != null && p.Kategoria.ToLower().Contains(w))
                                )
                            );
                        }
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(kategoria))
            {
                query = query.Where(p => p.Kategoria == kategoria);
            }

            return await query
                .Include(p => p.HistoriaVlerave)
                .OrderBy(p => p.EmriProduktit)
                .ToListAsync();
        }

        public async Task<VleraProduktit> Krijo(VleraProduktit vlera)
        {
            vlera.Krijuar_Me = DateTime.UtcNow;
            _konteksti.VleratProdukteve.Add(vlera);
            await _konteksti.SaveChangesAsync();
            return vlera;
        }

        /// <summary>
        /// Përditëso një vlerë produkti ekzistuese
        /// </summary>
        public async Task<VleraProduktit> Perditeso(VleraProduktit vlera)
        {
            vlera.Modifikuar_Me = DateTime.UtcNow;
            _konteksti.VleratProdukteve.Update(vlera);
            await _konteksti.SaveChangesAsync();
            return vlera;
        }

        public async Task<bool> Fshi(int id)
        {
            var vlera = await MerrSipasID(id);
            if (vlera == null)
                return false;

            vlera.Eshte_Aktiv = false;
            vlera.Modifikuar_Me = DateTime.UtcNow;
            await _konteksti.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Ruaj()
        {
            return await _konteksti.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<HistoriaVlerave>> MerrHistorine(int produktId)
        {
            return await _konteksti.HistoriaVlerave
                .Where(h => h.VleraProduktit_Id == produktId)
                .OrderByDescending(h => h.Ndryshuar_Me)
                .ToListAsync();
        }

        public async Task<HistoriaVlerave> ShtoNeHistori(HistoriaVlerave historia)
        {
            historia.Ndryshuar_Me = DateTime.UtcNow;
            _konteksti.HistoriaVlerave.Add(historia);
            await _konteksti.SaveChangesAsync();
            return historia;
        }
    }
}
