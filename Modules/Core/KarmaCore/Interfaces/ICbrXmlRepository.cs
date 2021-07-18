using KarmaCore.Utils;

namespace KarmaCore.Interfaces
{
    public interface ICbrXmlRepository
    {
        Currencies GetCurrencies(string filename);
    }
}
