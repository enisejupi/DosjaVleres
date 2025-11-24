using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;
using Microsoft.EntityFrameworkCore;

namespace KosovaDoganaModerne.Depo
{
    public class Depoja_KerkeseRegjistrim : IDepoja_KerkeseRegjistrim
    {
        private readonly AplikacioniDbKonteksti _context;

        public Depoja_KerkeseRegjistrim(AplikacioniDbKonteksti context)
        {
            _context = context;
        }

        public async Task<KerkeseRegjistrim?> MerrSipasId(int id)
        {
            return await _context.KerkesatRegjistrim.FindAsync(id);
        }

        public async Task<IEnumerable<KerkeseRegjistrim>> MerrTeGjitha()
        {
            return await _context.KerkesatRegjistrim
                .OrderByDescending(k => k.DataKerkeses)
                .ToListAsync();
        }

        public async Task<IEnumerable<KerkeseRegjistrim>> MerrSipasStatusit(string statusi)
        {
            return await _context.KerkesatRegjistrim
                .Where(k => k.Statusi == statusi)
                .OrderByDescending(k => k.DataKerkeses)
                .ToListAsync();
        }

        public async Task<KerkeseRegjistrim?> MerrSipasEmail(string email)
        {
            return await _context.KerkesatRegjistrim
                .FirstOrDefaultAsync(k => k.Email.ToLower() == email.ToLower());
        }

        public async Task Shto(KerkeseRegjistrim kerkese)
        {
            await _context.KerkesatRegjistrim.AddAsync(kerkese);
            await _context.SaveChangesAsync();
        }

        public async Task Perditeso(KerkeseRegjistrim kerkese)
        {
            _context.KerkesatRegjistrim.Update(kerkese);
            await _context.SaveChangesAsync();
        }

        public async Task Fshi(int id)
        {
            var kerkese = await MerrSipasId(id);
            if (kerkese != null)
            {
                _context.KerkesatRegjistrim.Remove(kerkese);
                await _context.SaveChangesAsync();
            }
        }
    }
}
