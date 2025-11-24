using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;

namespace KosovaDoganaModerne.Depo
{
    public class Depoja_KomentiDeges : IDepoja_KomentiDeges
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public Depoja_KomentiDeges(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<KomentiDeges>> MerrTeGjitha()
        {
            return await _konteksti.KomentetDegeve
                .AsNoTracking()
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<KomentiDeges?> MerrSipasID(int id)
        {
            return await _konteksti.KomentetDegeve
                .Include(k => k.VleraProduktit)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<IEnumerable<KomentiDeges>> MerrSipasDegës(string dega)
        {
            dega = dega.Trim();
            return await _konteksti.KomentetDegeve
                .AsNoTracking()
                .Where(k => EF.Functions.Like(k.EmriDeges.ToLower(), $"%{dega.ToLower()}%"))
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<KomentiDeges>> MerrSipasKoditTarifor(string kodiTarifor)
        {
            kodiTarifor = kodiTarifor.Trim();
            return await _konteksti.KomentetDegeve
                .AsNoTracking()
                .Where(k => EF.Functions.Like(k.KodiTarifar.ToLower(), $"%{kodiTarifor.ToLower()}%"))
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<KomentiDeges>> MerrSipasVleresProduktit(int vleraProduktitId)
        {
            return await _konteksti.KomentetDegeve
                .Include(k => k.VleraProduktit)
                .Where(k => k.VleraProduktit_Id == vleraProduktitId)
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<KomentiDeges>> MerrKomentetEpalexuara()
        {
            return await _konteksti.KomentetDegeve
                .AsNoTracking()
                .Where(k => !k.EshteLexuar)
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<KomentiDeges>> MerrKomentetEpazgjidhura()
        {
            return await _konteksti.KomentetDegeve
                .AsNoTracking()
                .Where(k => !k.EshteZgjidhur)
                .OrderByDescending(k => k.DataDergimit)
                .ToListAsync();
        }

        public async Task<KomentiDeges> Krijo(KomentiDeges komenti)
        {
            komenti.DataDergimit = DateTime.UtcNow;
            _konteksti.KomentetDegeve.Add(komenti);
            await _konteksti.SaveChangesAsync();
            return komenti;
        }

        public async Task<KomentiDeges> Perditeso(KomentiDeges komenti)
        {
            _konteksti.KomentetDegeve.Update(komenti);
            await _konteksti.SaveChangesAsync();
            return komenti;
        }

        public async Task<bool> Fshi(int id)
        {
            var komenti = await MerrSipasID(id);
            if (komenti == null)
                return false;

            _konteksti.KomentetDegeve.Remove(komenti);
            await _konteksti.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ShënosiLexuar(int id)
        {
            var komenti = await MerrSipasID(id);
            if (komenti == null)
                return false;

            komenti.EshteLexuar = true;
            await _konteksti.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ShënosiZgjidhur(int id, string pergjigjja, string pergjigurNga)
        {
            var komenti = await MerrSipasID(id);
            if (komenti == null)
                return false;

            komenti.EshteZgjidhur = true;
            komenti.EshteLexuar = true;
            komenti.Pergjigja = pergjigjja;
            komenti.PergjigjetNga = pergjigurNga;
            komenti.DataPergjigjes = DateTime.UtcNow;
            await _konteksti.SaveChangesAsync();
            return true;
        }
    }
}
