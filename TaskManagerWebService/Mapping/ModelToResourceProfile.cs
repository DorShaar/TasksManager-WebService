using AutoMapper;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Resources;

namespace TaskManagerWebService.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<TasksGroup, TasksGroupResource>();

            CreateMap<WorkTask, WorkTaskResource>()
                .ForMember(src => src.GroupName, 
                           opt => opt.MapFrom(src => src.ParentGroup.GroupName));
        }
    }
}