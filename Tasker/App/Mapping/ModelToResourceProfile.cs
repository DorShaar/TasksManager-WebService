using AutoMapper;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
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
                .ForMember(dest => dest.Status,
                                opt => opt.MapFrom(source => source.IsFinished ? "Closed" : "Open"))
                .ForMember(dest => dest.Size,
                                opt => opt.MapFrom(source => source.Size));

            CreateMap<IWorkTask, WorkTaskResource>()
                .ForMember(dest => dest.TaskId,
                           option => option.MapFrom(source => source.ID))
                .ForMember(dest => dest.GroupName,
                           option => option.MapFrom(source => source.GroupName))
                .ForMember(dest => dest.Description,
                           option => option.MapFrom(source => source.Description))
                .ForMember(dest => dest.Status,
                           option => option.MapFrom(source => source.Status.ToString()));
        }
    }
}