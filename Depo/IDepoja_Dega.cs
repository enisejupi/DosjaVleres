using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Depo
{
    public interface IDepoja_Dega
    {
        Task<IEnumerable<Dega>> MerrTeGjitha();
        Task<Dega?> MerrSipasID(int id);
        Task<Dega?> MerrSipasKodit(string kodiDeges);
        Task<IEnumerable<Dega>> MerrDegetAktive();
        Task<Dega> Krijo(Dega dega);
        Task<Dega> Perditeso(Dega dega);
        Task<bool> Fshi(int id);
    }
}
