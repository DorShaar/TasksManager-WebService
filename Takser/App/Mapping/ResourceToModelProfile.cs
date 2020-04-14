using AutoMapper;
using TaskData.Contracts;
using Tasker.App.Resources;

namespace Tasker.App.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<SaveTasksGroupResource, ITasksGroup>();
        }
    }
}