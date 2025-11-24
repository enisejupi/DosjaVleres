using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Depo
{
    public interface IDepoja_ShpenzimiTransportit
    {
        Task<IEnumerable<ShpenzimiTransportit>> MerrTeGjitha();
        Task<ShpenzimiTransportit?> MerrSipasID(int id);
        Task<ShpenzimiTransportit?> MerrSipasID_PaTracking(int id);
        Task<IEnumerable<ShpenzimiTransportit>> Kerko(string? vendiOrigjines, string? vendiDestinacionit, string? llojiTransportit);
        Task<ShpenzimiTransportit> Krijo(ShpenzimiTransportit shpenzimi);
        Task<ShpenzimiTransportit> Perditeso(ShpenzimiTransportit shpenzimi);
        Task<bool> Fshi(int id);
        Task<IEnumerable<NdryshimiTransportit>> MerrHistorine(int shpenzimiId);
        Task<NdryshimiTransportit> ShtoNeHistori(NdryshimiTransportit ndryshimi);
    }
}
