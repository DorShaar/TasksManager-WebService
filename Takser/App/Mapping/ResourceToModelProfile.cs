using AutoMapper;
using Tasker.App.Resources;
using Tasker.Domain.Models;

namespace Tasker.App.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<SaveTasksGroupResource, TasksGroup>();
        }
    }
}