using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Mapping;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Infra.Persistence.Repositories;
using Tasker.Infra.Services;

namespace Takser.Infra.Extensions
{
    public static class DIExtensions
    {
        public static void UseDI(this IServiceCollection services)
        {
            services.AddSingleton<IDbRepository<ITasksGroup>, TasksGroupRepository>();
            services.AddSingleton<ITasksGroupService, TasksGroupService>();

            services.AddSingleton<IDbRepository<IWorkTask>, WorkTaskRepository>();
            services.AddSingleton<IWorkTaskService, WorkTaskService>();

            services.AddSingleton<INoteBuilder, NoteBuilder>();
            services.AddSingleton<INotesSubjectBuilder, NotesSubjectBuilder>();
            services.AddSingleton<IWorkTask, WorkTask>();
            services.AddSingleton<ITasksGroup, TaskGroup>();
            services.AddSingleton<ITasksGroupBuilder, TaskGroupBuilder>();

            services.AddAutoMapper(typeof(ModelToResourceProfile));
            services.AddAutoMapper(typeof(ResourceToModelProfile));

            services.AddHostedService<FileUploaderService>();
        }
    }
}