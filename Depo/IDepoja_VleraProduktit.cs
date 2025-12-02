using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Depo
{
    public interface IDepoja_VleraProduktit
    {
        Task<IEnumerable<VleraProduktit>> MerrTeGjitha();

        Task<VleraProduktit?> MerrSipasID(int id);

        Task<VleraProduktit?> MerrSipasID_PaTracking(int id);

        Task<VleraProduktit?> MerrSipasKodit(string kodi);

        Task<IEnumerable<VleraProduktit>> Kerko(string? termi, string? kategoria, string? llojiKerkimit = null, bool vetemAktive = true);

        Task<VleraProduktit> Krijo(VleraProduktit vlera);

        /// <summary>
        /// Përditëso një vlerë produkti ekzistuese
        /// </summary>
        Task<VleraProduktit> Perditeso(VleraProduktit vlera);

        Task<bool> Fshi(int id);

        Task<bool> Ruaj();

        Task<IEnumerable<HistoriaVlerave>> MerrHistorine(int produktId);

        Task<HistoriaVlerave> ShtoNeHistori(HistoriaVlerave historia);

        // Image operations
        Task<IEnumerable<ImazhetProduktit>> MerrImazhetProduktit(int produktId);
        
        Task<ImazhetProduktit?> MerrImazhSipasID(int imazhId);
        
        Task<ImazhetProduktit> ShtoImazh(ImazhetProduktit imazh);
        
        Task ShtoImazhe(List<ImazhetProduktit> imazhet);
        
        Task<bool> FshiImazhin(int imazhId);
        
        Task VendosImazhKryesor(int imazhId, int produktId);
    }
}
