using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;

namespace KosovaDoganaModerne.Depo
{
    public class Depoja_ShpenzimiTransportit : IDepoja_ShpenzimiTransportit
    {
        private readonly AplikacioniDbKonteksti _konteksti;

        public Depoja_ShpenzimiTransportit(AplikacioniDbKonteksti konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<ShpenzimiTransportit>> MerrTeGjitha()
        {
            return await _konteksti.ShpenzimetTransportit
                .Include(s => s.Ndryshimet)
                .OrderBy(s => s.VendiOrigjines)
                .ThenBy(s => s.VendiDestinacionit)
                .ToListAsync();
        }

        public async Task<ShpenzimiTransportit?> MerrSipasID(int id)
        {
            return await _konteksti.ShpenzimetTransportit
                .Include(s => s.Ndryshimet)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ShpenzimiTransportit?> MerrSipasID_PaTracking(int id)
        {
            return await _konteksti.ShpenzimetTransportit
                .AsNoTracking()
                .Include(s => s.Ndryshimet)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<ShpenzimiTransportit>> Kerko(string? vendiOrigjines, string? vendiDestinacionit, string? llojiTransportit)
        {
            var query = _konteksti.ShpenzimetTransportit
                .Where(s => s.Aktiv)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(vendiOrigjines))
            {
                vendiOrigjines = vendiOrigjines.Trim();
                query = query.Where(s => EF.Functions.Like(s.VendiOrigjines.ToLower(), $"%{vendiOrigjines.ToLower()}%"));
            }

            if (!string.IsNullOrWhiteSpace(vendiDestinacionit))
            {
                vendiDestinacionit = vendiDestinacionit.Trim();
                query = query.Where(s => EF.Functions.Like(s.VendiDestinacionit.ToLower(), $"%{vendiDestinacionit.ToLower()}%"));
            }

            if (!string.IsNullOrWhiteSpace(llojiTransportit))
            {
                llojiTransportit = llojiTransportit.Trim();
                query = query.Where(s => EF.Functions.Like(s.LlojiTransportit.ToLower(), $"%{llojiTransportit.ToLower()}%"));
            }

            return await query
                .Include(s => s.Ndryshimet)
                .OrderBy(s => s.VendiOrigjines)
                .ToListAsync();
        }

        public async Task<ShpenzimiTransportit> Krijo(ShpenzimiTransportit shpenzimi)
        {
            shpenzimi.DataKrijimit = DateTime.UtcNow;
            _konteksti.ShpenzimetTransportit.Add(shpenzimi);
            await _konteksti.SaveChangesAsync();
            return shpenzimi;
        }

        public async Task<ShpenzimiTransportit> Perditeso(ShpenzimiTransportit shpenzimi)
        {
            shpenzimi.DataPerditesimit = DateTime.UtcNow;
            _konteksti.ShpenzimetTransportit.Update(shpenzimi);
            await _konteksti.SaveChangesAsync();
            return shpenzimi;
        }

        public async Task<bool> Fshi(int id)
        {
            var shpenzimi = await MerrSipasID(id);
            if (shpenzimi == null)
                return false;

            shpenzimi.Aktiv = false;
            shpenzimi.DataPerditesimit = DateTime.UtcNow;
            await _konteksti.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NdryshimiTransportit>> MerrHistorine(int shpenzimiId)
        {
            return await _konteksti.NdryshimetTransportit
                .Where(n => n.ShpenzimiTransportit_Id == shpenzimiId)
                .OrderByDescending(n => n.Ndryshuar_Me)
                .ToListAsync();
        }

        public async Task<NdryshimiTransportit> ShtoNeHistori(NdryshimiTransportit ndryshimi)
        {
            ndryshimi.Ndryshuar_Me = DateTime.UtcNow;
            _konteksti.NdryshimetTransportit.Add(ndryshimi);
            await _konteksti.SaveChangesAsync();
            return ndryshimi;
        }
    }
}
