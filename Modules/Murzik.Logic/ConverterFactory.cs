using Murzik.Entities.Cbr;
using Murzik.Interfaces;
using Murzik.Logic.Cbr;
using System;
using System.Collections.Generic;

namespace Murzik.Logic
{
    public class ConverterFactory : IConverterFactory
    {
        public ConverterFactory()
        {
            AddTConverter(typeof(PackCurrencies), new ConverterForeignExchange());
        }

        private Dictionary<Type, IConverter> _storage = new Dictionary<Type, IConverter>();

        public void AddTConverter(Type type, IConverter converter)
        {
            _storage[type] = converter;
        }

        public IConverter GetConverter(Type type)
        {
            return _storage[type];
        }
    }
}
