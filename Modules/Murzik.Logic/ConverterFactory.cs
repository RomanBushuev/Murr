using Murzik.Entities.Cbr.Packs;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Interfaces;
using Murzik.Logic.Cbr.Converter;
using Murzik.Logic.Moex.Converter;
using System;
using System.Collections.Generic;

namespace Murzik.Logic
{
    public class ConverterFactory : IConverterFactory
    {
        public ConverterFactory()
        {
            AddTConverter(typeof(PackCurrencies), new ConverterForeignExchange());
            AddTConverter(typeof(PackMoexBonds), new ConverterMoexBonds());
            AddTConverter(typeof(PackMoexBondQuote), new ConverterMoexBondQuotes());
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
