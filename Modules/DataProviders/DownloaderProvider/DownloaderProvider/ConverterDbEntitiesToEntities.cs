using AutoMapper;
using DownloaderProvider.DatabaseEntities;
using DownloaderProvider.Entities;
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
            //long enum
            CreateMap<DbKarmaDownloadJob, KarmaDownloadJob>()
                .ForMember(m => (long)m.TaskStatuses,
                    x => x.MapFrom(source => (TaskStatuses)source.TaskStatusId))
                .ReverseMap()
                .ForMember(m=> m.TaskId, x=>x.MapFrom(source => source.TaskId))
                .ForMember(m => m.TaskTemplateId, x => x.MapFrom(source => source.TaskTemplateId));

            CreateMap<KarmaDownloadJob, DbKarmaDownloadJob>()
                .ForMember(m => (TaskStatuses)m.TaskStatusId,
                    x=> x.MapFrom(source => (long)source.TaskStatuses))
                .ForMember(m => m.TaskId, x => x.MapFrom(source => source.TaskId))
                .ForMember(m => m.TaskTemplateId, x => x.MapFrom(source => source.TaskTemplateId));
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