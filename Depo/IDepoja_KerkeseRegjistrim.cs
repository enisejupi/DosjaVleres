using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Depo
{
    public interface IDepoja_KerkeseRegjistrim
    {
        Task<KerkeseRegjistrim?> MerrSipasId(int id);
        Task<IEnumerable<KerkeseRegjistrim>> MerrTeGjitha();
        Task<IEnumerable<KerkeseRegjistrim>> MerrSipasStatusit(string statusi);
        Task<KerkeseRegjistrim?> MerrSipasEmail(string email);
        Task Shto(KerkeseRegjistrim kerkese);
        Task Perditeso(KerkeseRegjistrim kerkese);
        Task Fshi(int id);
    }
}
