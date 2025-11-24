using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;
using System.Text.Json;

namespace KosovaDoganaModerne.Sherbime
{
    public class SherbimetAuditimit
    {
        private readonly AplikacioniDbKonteksti _konteksti;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SherbimetAuditimit(AplikacioniDbKonteksti konteksti, IHttpContextAccessor httpContextAccessor)
        {
            _konteksti = konteksti;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Menaxhimi i regjistrave të auditimit

        public async Task<RegjistriAuditimit> RegjistroVeprim(
            string perdoruesi,
            string llojiVeprimit,
            string entiteti,
            string? entititiId = null,
            object? vlerat_vjetra = null,
            object? vlerat_reja = null,
            string? detajet = null,
            bool eshte_suksesshem = true,
            string? mesazhiGabimit = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var sesioni_Id = httpContext?.Session.Id;
            var adresaIP = MerrAdresenIP();
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            var regjistri = new RegjistriAuditimit
            {
                Perdoruesi = perdoruesi,
                LlojiVeprimit = llojiVeprimit,
                Entiteti = entiteti,
                Entiteti_Id = entititiId,
                Detajet = detajet,
                Vlerat_Vjetra = vlerat_vjetra != null ? JsonSerializer.Serialize(vlerat_vjetra) : null,
                Vlerat_Reja = vlerat_reja != null ? JsonSerializer.Serialize(vlerat_reja) : null,
                Koha = DateTime.UtcNow,
                AdresaIP = adresaIP,
                UserAgent = userAgent,
                Eshte_Suksesshem = eshte_suksesshem,
                MesazhiGabimit = mesazhiGabimit,
                Sesioni_Id = sesioni_Id
            };

            _konteksti.RegjistriAuditimit.Add(regjistri);
            await _konteksti.SaveChangesAsync();

            // Përditëso numrin e veprimeve në sesion
            await PerditësoNumrinVeprimeve(sesioni_Id);

            return regjistri;
        }

        public async Task<IEnumerable<RegjistriAuditimit>> MerrRegjistrat(
            string? perdoruesi = null,
            string? llojiVeprimit = null,
            string? entiteti = null,
            DateTime? ngaData = null,
            DateTime? deriData = null,
            int? numriRreshtave = null)
        {
            var query = _konteksti.RegjistriAuditimit.AsQueryable();

            if (!string.IsNullOrWhiteSpace(perdoruesi))
                query = query.Where(r => r.Perdoruesi == perdoruesi);

            if (!string.IsNullOrWhiteSpace(llojiVeprimit))
                query = query.Where(r => r.LlojiVeprimit == llojiVeprimit);

            if (!string.IsNullOrWhiteSpace(entiteti))
                query = query.Where(r => r.Entiteti == entiteti);

            if (ngaData.HasValue)
                query = query.Where(r => r.Koha >= ngaData.Value);

            if (deriData.HasValue)
                query = query.Where(r => r.Koha <= deriData.Value);

            query = query.OrderByDescending(r => r.Koha);

            if (numriRreshtave.HasValue)
                query = query.Take(numriRreshtave.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<RegjistriAuditimit>> MerrRegjistratPerEntitet(string entiteti, string entititiId)
        {
            return await _konteksti.RegjistriAuditimit
                .Where(r => r.Entiteti == entiteti && r.Entiteti_Id == entititiId)
                .OrderByDescending(r => r.Koha)
                .ToListAsync();
        }

        #endregion

        #region Menaxhimi i sesioneve

        public async Task<SesioniPerdoruesit> FilloSesion(string perdoruesi)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var sesioni_Id = httpContext?.Session.Id ?? Guid.NewGuid().ToString();
            var adresaIP = MerrAdresenIP();
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            var sesioni = new SesioniPerdoruesit
            {
                Sesioni_Id = sesioni_Id,
                Perdoruesi = perdoruesi,
                Filluar_Me = DateTime.UtcNow,
                Eshte_Aktiv = true,
                AdresaIP = adresaIP,
                UserAgent = userAgent,
                AktivitetiI_Fundit = DateTime.UtcNow
            };

            _konteksti.SesionetPerdoruesve.Add(sesioni);
            await _konteksti.SaveChangesAsync();

            // Regjistro fillimin e sesionit në audit
            await RegjistroVeprim(perdoruesi, "FilloSesion", "SesioniPerdoruesit", sesioni_Id);

            return sesioni;
        }

        /// <summary>
        /// Përfundo një sesion përdoruesi
        /// </summary>
        public async Task<bool> PerfundoSesion(string sesioni_Id, string? arsyeja = null)
        {
            var sesioni = await _konteksti.SesionetPerdoruesve
                .FirstOrDefaultAsync(s => s.Sesioni_Id == sesioni_Id && s.Eshte_Aktiv);

            if (sesioni == null)
                return false;

            sesioni.Perfunduar_Me = DateTime.UtcNow;
            sesioni.Eshte_Aktiv = false;
            sesioni.ArsyejaPerfundimit = arsyeja ?? "Normal";
            sesioni.Kohezgjatja_Minuta = (int)(sesioni.Perfunduar_Me.Value - sesioni.Filluar_Me).TotalMinutes;

            await _konteksti.SaveChangesAsync();

            // Regjistro përfundimin e sesionit në audit
            await RegjistroVeprim(sesioni.Perdoruesi, "PerfundoSesion", "SesioniPerdoruesit", sesioni_Id, detajet: arsyeja);

            return true;
        }

        public async Task<IEnumerable<SesioniPerdoruesit>> MerrSesionetAktive()
        {
            return await _konteksti.SesionetPerdoruesve
                .Where(s => s.Eshte_Aktiv)
                .OrderByDescending(s => s.AktivitetiI_Fundit)
                .ToListAsync();
        }

        public async Task<IEnumerable<SesioniPerdoruesit>> MerrSesionetPerdoruesit(string perdoruesi, bool vetemAktive = false)
        {
            var query = _konteksti.SesionetPerdoruesve
                .Where(s => s.Perdoruesi == perdoruesi);

            if (vetemAktive)
                query = query.Where(s => s.Eshte_Aktiv);

            return await query
                .OrderByDescending(s => s.Filluar_Me)
                .ToListAsync();
        }

        /// <summary>
        /// Përditëso aktivitetin e fundit të sesionit
        /// </summary>
        public async Task PerditësoAktivitetin(string? sesioni_Id)
        {
            if (string.IsNullOrWhiteSpace(sesioni_Id))
                return;

            var sesioni = await _konteksti.SesionetPerdoruesve
                .FirstOrDefaultAsync(s => s.Sesioni_Id == sesioni_Id && s.Eshte_Aktiv);

            if (sesioni != null)
            {
                sesioni.AktivitetiI_Fundit = DateTime.UtcNow;
                await _konteksti.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Përditëso numrin e veprimeve në sesion
        /// </summary>
        private async Task PerditësoNumrinVeprimeve(string? sesioni_Id)
        {
            if (string.IsNullOrWhiteSpace(sesioni_Id))
                return;

            var sesioni = await _konteksti.SesionetPerdoruesve
                .FirstOrDefaultAsync(s => s.Sesioni_Id == sesioni_Id && s.Eshte_Aktiv);

            if (sesioni != null)
            {
                sesioni.NumriVeprimeve++;
                sesioni.AktivitetiI_Fundit = DateTime.UtcNow;
                await _konteksti.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Përfundo sesionet e vjetra të papërdorura
        /// </summary>
        public async Task<int> PerfundoSesionetVjetra(int minutat = 30)
        {
            var kohaLimit = DateTime.UtcNow.AddMinutes(-minutat);
            var sesionetVjetra = await _konteksti.SesionetPerdoruesve
                .Where(s => s.Eshte_Aktiv && s.AktivitetiI_Fundit < kohaLimit)
                .ToListAsync();

            foreach (var sesioni in sesionetVjetra)
            {
                await PerfundoSesion(sesioni.Sesioni_Id, "Sesion i vjetëruar (timeout)");
            }

            return sesionetVjetra.Count;
        }

        #endregion

        #region Metoda Ndihmëse / Helper Methods

        private string? MerrAdresenIP()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Kontrollo për proxy
            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            return ipAddress;
        }

        #endregion
    }
}
