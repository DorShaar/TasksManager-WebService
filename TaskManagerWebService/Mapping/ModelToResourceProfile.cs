using AutoMapper;
using MyFirstWebApp.Domain.Models;
using MyFirstWebApp.Resources;

namespace MyFirstWebApp.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<TasksGroup, TasksGroupResource>();
        }
    }
}