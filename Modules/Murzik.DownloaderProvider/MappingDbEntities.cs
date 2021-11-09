using AutoMapper;
using Murzik.DownloaderProvider.DbEntities;
using Murzik.Entities;
using Murzik.Entities.Enumerations;

namespace Murzik.DownloaderProvider
{
    public class MappingDbEntities : Profile
    {
        public MappingDbEntities()
        {
            //karma job
            CreateMap<DbKarmaDownloadJob, KarmaDownloadJob>()
                .ForMember(m => (long)m.TaskStatuses,
                    x => x.MapFrom(source => (TaskStatuses)source.TaskStatusId))
                .ReverseMap()
                .ForMember(m => m.TaskId, x => x.MapFrom(source => source.TaskId))
                .ForMember(m => m.TaskTemplateId, x => x.MapFrom(source => source.TaskTemplateId))
                .ForMember(m => m.SaverTemplateId, x => x.MapFrom(source => source.SaverTemplateId));

            CreateMap<KarmaDownloadJob, DbKarmaDownloadJob>()
                .ForMember(m => (TaskStatuses)m.TaskStatusId,
                    x => x.MapFrom(source => (long)source.TaskStatuses))
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
                .ForMember(m => m.TaskType, x => x.MapFrom(source => source.TaskType))
                .ForMember(m => m.TaskTemplateFolderId, x => x.MapFrom(source => source.TaskTemplateFolderId));

            CreateMap<CalculationJson, DbCalculationJson>()
                .ForMember(m => m.TaskTemplate, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => m.TaskType, x => x.MapFrom(source => source.TaskType))
                .ForMember(m=> m.TaskTemplateFolderId, x=>x.MapFrom(source => source.TaskTemplateFolderId));

            //saver template
            CreateMap<DbSaverJson, SaverJson>()
                .ForMember(m => m.JsonParameters, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => (long)m.SaverType,
                    x => x.MapFrom(source => (SaverTypes)source.SaverType));
                //.ForMember(m => m.SaverType, x => x.MapFrom(source => source.SaverType));
                //.ForMember(m => m., x => x.MapFrom(source => source.));

            CreateMap<SaverJson, DbSaverJson>()
                .ForMember(m => m.JsonParameters, x => x.MapFrom(source => source.JsonParameters))
                .ForMember(m => (SaverTypes)m.SaverType,
                    x => x.MapFrom(source => (long)source.SaverType))
                /*.ForMember(m => m.SaverType, x => x.MapFrom(source => source.SaverType))*/;
        }
    }
}