using AutoMapper;
using DownloaderProvider.DatabaseEntities;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DownloaderProvider
{
    public class ConverterDbEntitiesToEntities : Profile
    {
        public ConverterDbEntitiesToEntities()
        {
            //karma job
            CreateMap<DbKarmaDownloadJob, KarmaDownloadJob>()
                .ForMember(m => (long)m.TaskStatuses,
                    x => x.MapFrom(source => (TaskStatuses)source.TaskStatusId))
                .ReverseMap()
                .ForMember(m=> m.TaskId, x=>x.MapFrom(source => source.TaskId))
                .ForMember(m => m.TaskTemplateId, x => x.MapFrom(source => source.TaskTemplateId))
                .ForMember(m=>m.SaverTemplateId, x=>x.MapFrom(source => source.SaverTemplateId));

            CreateMap<KarmaDownloadJob, DbKarmaDownloadJob>()
                .ForMember(m => (TaskStatuses)m.TaskStatusId,
                    x=> x.MapFrom(source => (long)source.TaskStatuses))
                .ForMember(m => m.TaskId, x => x.MapFrom(source => source.TaskId))
                .ForMember(m => m.TaskTemplateId, x => x.MapFrom(source => source.TaskTemplateId))
                .ForMember(m => m.SaverTemplateId, x => x.MapFrom(source => source.SaverTemplateId));

            //karma service
            CreateMap<DbKarmaService, KarmaService>()
                .ForMember(m => (long)m.ServiceStatus,
                    x => x.MapFrom(source => (ServiceStatuses)source.ServiceStatus))
                .ReverseMap()
                .ForMember(m => m.ServiceId, x => x.MapFrom(source => source.ServiceId))
                .ForMember(m => m.ServiceTitle, x => x.MapFrom(source => source.ServiceTitle));

            CreateMap<KarmaService, DbKarmaService>()
                .ForMember(m => (ServiceStatuses)m.ServiceStatus,
                    x => x.MapFrom(source => (long)source.ServiceStatus))
                .ForMember(m => m.ServiceId, x => x.MapFrom(source => source.ServiceId))
                .ForMember(m => m.ServiceTitle, x => x.MapFrom(source => source.ServiceTitle));

            //calculation template
            CreateMap<DbCalculationJson, CalculationJson>()
                .ForMember(m => m.JsonParameters, x => x.MapFrom(source => source.TaskTemplate))
                .ForMember(m => m.TaskType, x => x.MapFrom(source => source.TaskType));

            CreateMap<CalculationJson, DbCalculationJson>()
                .ForMember(m => m.TaskTemplate, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => m.TaskType, x => x.MapFrom(source => source.TaskType));

            //saver template
            CreateMap<DbSaverJson, SaverJson>()
                .ForMember(m => m.JsonParameters, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => m.SaverType, x => x.MapFrom(source => source.SaverType));

            CreateMap<SaverJson, DbSaverJson>()
                .ForMember(m => m.JsonParameters, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => m.SaverType, x => x.MapFrom(source => source.SaverType));

            CreateMap<DbFinDataSource, FinDataSource>()
                .ForMember(m => m.FinDataSourceId, x => x.MapFrom(source => source.FinDataSourceId))
                .ForMember(m => m.FinDataSourceIdent, x => x.MapFrom(source => source.FinDataSourceIdent));
        }
    }

    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ConverterDbEntitiesToEntities>();
            });

            return config;
        }
    }


    public class ConverterDto
    {
        private static ConverterDto _instance;
        private bool _isInitialized = false;
        private ConverterDbEntitiesToEntities entitiesToEntities;
        private AutoMapperConfiguration MapperConfiguration;

        private ConverterDto()
        {
            entitiesToEntities = new ConverterDbEntitiesToEntities();            
        }

        public static U ConvertDto<T, U>(T entity)
        {
            var config = AutoMapperConfiguration.Configure();
            IMapper mapper = config.CreateMapper();
            return mapper.Map<T, U>(entity);
        }

        public static IEnumerable<U> ConvertDto<T, U>(IEnumerable<T> entities)
        {
            var config = AutoMapperConfiguration.Configure();
            IMapper mapper = config.CreateMapper();
            var list = entities.Select(z => mapper.Map<T, U>(z));
            return list;
        }
    }
}