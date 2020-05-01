using AutoMapper;
using Database.JsonService;
using Logger;
using Logger.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectSerializer.Contracts;
using System.IO;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Mapping;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;
using Tasker.Infra.Services;

namespace Tasker.Infra.Extensions
{
    public static class DIExtensions
    {
        public static void UseDI(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            RegisterTaskerCoreComponents(services);
            RegisterDadabases(services);

            services.AddAutoMapper(typeof(ModelToResourceProfile));

            AddConfiguration(services);

            services.AddHostedService<FileUploaderService>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ITasksGroupService, TasksGroupService>();
            services.AddSingleton<IWorkTaskService, WorkTaskService>();
            services.AddSingleton<INoteService, NoteService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IDbRepository<ITasksGroup>, TasksGroupRepository>();
            services.AddSingleton<IDbRepository<IWorkTask>, WorkTaskRepository>();
        }

        private static void RegisterDadabases(IServiceCollection services)
        {
            services.AddSingleton<IAppDbContext, AppDbContext>();
        }

        private static void RegisterTaskerCoreComponents(IServiceCollection services)
        {
            services.AddSingleton<INoteBuilder, NoteBuilder>();
            services.AddSingleton<INotesSubjectBuilder, NotesSubjectBuilder>();
            services.AddSingleton<IWorkTask, WorkTask>();
            services.AddSingleton<ITasksGroup, TaskGroup>();
            services.AddSingleton<ITasksGroupBuilder, TaskGroupBuilder>();
            services.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();
            services.AddSingleton<ILogger, ConsoleLogger>();
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddYamlFile(Path.Combine("Infra", "Options", "ConfigFile", "TaskerConfig.yaml"), optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to DatabaseConfigurtaion.
            services.Configure<DatabaseConfigurtaion>(configuration);
            services.AddOptions();
        }
    }
}