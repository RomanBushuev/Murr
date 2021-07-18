using KarmaCore.Entities;
using System.Data;

namespace KarmaCore.Interfaces
{
    public interface IFinInstrumentRepository
    {
        FinInstrument CreateOrGet(IDbConnection connection, FinInstrument finInstrument);
    }
}
