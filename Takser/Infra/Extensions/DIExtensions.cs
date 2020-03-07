using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Takser.Infra.Services;
using Tasker.App.Mapping;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Domain.Models;
using Tasker.Infra.Persistence.Repositories;
using TaskManagerWebService.Persistence.Repositories;
using TaskManagerWebService.Services;

namespace Takser.Infra.Extensions
{
    public static class DIExtensions
    {
        public static void UseDI(this IServiceCollection services)
        {
            services.AddSingleton<IDbRepository<TasksGroup>, TasksGroupRepository>();
            services.AddSingleton<ITasksGroupService, TasksGroupService>();

            services.AddSingleton<IDbRepository<WorkTask>, WorkTaskRepository>();
            services.AddSingleton<IWorkTaskService, WorkTaskService>();

            services.AddAutoMapper(typeof(ModelToResourceProfile));
            services.AddAutoMapper(typeof(ResourceToModelProfile));

            services.AddHostedService<FileUploaderService>();
        }
    }
}