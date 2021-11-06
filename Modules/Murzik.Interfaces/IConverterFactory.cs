using System;

namespace Murzik.Interfaces
{
    public interface IConverterFactory
    {
        void AddTConverter(Type type, IConverter converter);

        IConverter GetConverter(Type type);
    }
}
