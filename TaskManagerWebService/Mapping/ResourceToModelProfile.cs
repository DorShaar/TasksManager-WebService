using AutoMapper;
using MyFirstWebApp.Domain.Models;
using TaskManagerWebService.Resources;

namespace TaskManagerWebService.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<SaveTasksGroupResource, TasksGroup>();
        }
    }
}