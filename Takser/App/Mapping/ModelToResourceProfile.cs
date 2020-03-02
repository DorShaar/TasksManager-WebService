using AutoMapper;
using Tasker.App.Resources;
using Tasker.Domain.Models;

namespace Tasker.App.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<TasksGroup, TasksGroupResource>();

            CreateMap<WorkTask, WorkTaskResource>()
                .ForMember(src => src.GroupName, 
                           opt => opt.MapFrom(src => src.ParentGroup.Name));
        }
    }
}