using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;

namespace KosovaDoganaModerne.Depo
{
    public class Depoja_Dega : IDepoja_Dega
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public Depoja_Dega(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<Dega>> MerrTeGjitha()
        {
            return await _konteksti.Deget
                .Include(d => d.Perdoruesit)
                .OrderBy(d => d.EmriDeges)
                .ToListAsync();
        }

        public async Task<Dega?> MerrSipasID(int id)
        {
            return await _konteksti.Deget
                .Include(d => d.Perdoruesit)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Dega?> MerrSipasKodit(string kodiDeges)
        {
            return await _konteksti.Deget
                .Include(d => d.Perdoruesit)
                .FirstOrDefaultAsync(d => d.KodiDeges == kodiDeges);
        }

        public async Task<IEnumerable<Dega>> MerrDegetAktive()
        {
            return await _konteksti.Deget
                .Where(d => d.Eshte_Aktiv)
                .OrderBy(d => d.EmriDeges)
                .ToListAsync();
        }

        public async Task<Dega> Krijo(Dega dega)
        {
            dega.Krijuar_Me = DateTime.UtcNow;
            _konteksti.Deget.Add(dega);
            await _konteksti.SaveChangesAsync();
            return dega;
        }

        public async Task<Dega> Perditeso(Dega dega)
        {
            _konteksti.Deget.Update(dega);
            await _konteksti.SaveChangesAsync();
            return dega;
        }

        public async Task<bool> Fshi(int id)
        {
            var dega = await MerrSipasID(id);
            if (dega == null)
                return false;

            dega.Eshte_Aktiv = false;
            await _konteksti.SaveChangesAsync();
            return true;
        }
    }
}
