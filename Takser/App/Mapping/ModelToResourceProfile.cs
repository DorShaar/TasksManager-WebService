using AutoMapper;
using TaskData.Contracts;
using Tasker.App.Resources;

namespace Tasker.App.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<ITasksGroup, TasksGroupResource>()
                .ForMember(dest => dest.GroupId,
                            opt => opt.MapFrom(source => source.ID))
                .ForMember(dest => dest.GroupName,
                            opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.IsFinished,
                                opt => opt.MapFrom(source => source.IsFinished))
                .ForMember(dest => dest.Size,
                                opt => opt.MapFrom(source => source.Size));

            CreateMap<IWorkTask, WorkTaskResource>()
                .ForMember(dest => dest.GroupName, 
                           option => option.MapFrom(source => source.GroupName));
        }
    }
}