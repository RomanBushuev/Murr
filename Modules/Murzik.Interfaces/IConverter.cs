using Murzik.Entities.MurrData;

namespace Murzik.Interfaces
{
    public interface IConverter
    {
        PackValues ConvertToPackValues<T>(T importData);
    }
}
