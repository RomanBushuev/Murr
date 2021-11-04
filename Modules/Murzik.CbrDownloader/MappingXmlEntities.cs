using AutoMapper;
using Murzik.CbrDownloader.XmlEntities;
using Murzik.Entities.Cbr;

namespace Murzik.CbrDownloader
{
    internal class MappingXmlEntities : Profile
    {
        public MappingXmlEntities()
        {
            CreateMap<KeyRate, KeyRateXml>()
                .ReverseMap();
            CreateMap<Kr, KrXml>()
                .ReverseMap();

            CreateMap<Currencies, CurrenciesXml>()
                .ReverseMap();
            CreateMap<ValuteCursOnDate, ValuteCursOnDateXml>()
                .ReverseMap();

            CreateMap<MosPrime, MosPrimeXml>()
                .ReverseMap();
            CreateMap<MP, MPXml>()
                .ReverseMap();

            CreateMap<RoisFix, RoisFixXml>()
                .ReverseMap();
            CreateMap<Rf, RfXml>()
                .ReverseMap();

            CreateMap<Ruonia, RuoniaXml>()
                .ReverseMap();
            CreateMap<Ro, RoXml>()
                .ReverseMap();
        }
    }
}