using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Depo
{
    public interface IDepoja_KomentiDeges
    {
        Task<IEnumerable<KomentiDeges>> MerrTeGjitha();
        Task<KomentiDeges?> MerrSipasID(int id);
        Task<IEnumerable<KomentiDeges>> MerrSipasDegës(string dega);
        Task<IEnumerable<KomentiDeges>> MerrSipasKoditTarifor(string kodiTarifor);
        Task<IEnumerable<KomentiDeges>> MerrSipasVleresProduktit(int vleraProduktitId);
        Task<IEnumerable<KomentiDeges>> MerrKomentetEpalexuara();
        Task<IEnumerable<KomentiDeges>> MerrKomentetEpazgjidhura();
        Task<KomentiDeges> Krijo(KomentiDeges komenti);
        Task<KomentiDeges> Perditeso(KomentiDeges komenti);
        Task<bool> Fshi(int id);
        Task<bool> ShënosiLexuar(int id);
        Task<bool> ShënosiZgjidhur(int id, string pergjigjja, string pergjigurNga);
    }
}
