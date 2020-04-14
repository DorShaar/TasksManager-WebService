using AutoMapper;
using TaskData.Contracts;
using Tasker.App.Resources;

namespace Tasker.App.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<ITasksGroup, TasksGroupResource>();

            CreateMap<IWorkTask, WorkTaskResource>()
                .ForMember(src => src.GroupName, 
                           opt => opt.MapFrom(src => src.GroupName));
        }
    }
}