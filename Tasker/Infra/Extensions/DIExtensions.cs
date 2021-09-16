using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData.Ioc;
using TaskData.ObjectSerializer.JsonService;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Mapping;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Infra.HostedServices;
using Tasker.Infra.Options;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;
using Tasker.Infra.Services;
using Tasker.Infra.Services.Notifier;

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
            RegisterLogger(services);

            services.AddAutoMapper(typeof(ModelToResourceProfile));

            AddConfiguration(services);

            services.AddHostedService<FileUploaderService>();
            services.AddHostedService<TaskerNotifier>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ITasksGroupService, TasksGroupService>();
            services.AddSingleton<IWorkTaskService, WorkTaskService>();
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<ICloudService, GoogleDriveCloudService>();
            services.AddSingleton<INotifierService, NotifierService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IArchiverService, ArchiverService>();
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
            services.UseJsonObjectSerializer();
            services.UseTaskerDataEntities();
            services.RegisterRegularWorkTaskProducer();
            services.RegisterRegularTasksGroupProducer();
        }

        private static void RegisterLogger(IServiceCollection services)
        {
            services.AddLogging(loggerBuilder =>
                loggerBuilder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            string configFileName;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                configFileName = "TaskerConfig - work.yaml";
            else
                configFileName = "TaskerConfig.yaml";
            configurationBuilder.AddYamlFile(Path.Combine("Infra", "Options", "ConfigFile", configFileName), optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to given configurtaion.
            services.Configure<DatabaseConfigurtaion>(configuration);
            services.Configure<TaskerConfiguration>(configuration);
            services.AddOptions();
        }
    }
}