﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData.IDsProducer;
using TaskData.TasksGroups;

namespace Tasker.Infra.Persistence.Context
{
    public class AppDbContext : IAppDbContext
    {
        private const string DatabaseName = "tasks.db";
        private const string NextIdHolderName = "id_producer.db";

        private readonly IObjectSerializer mSerializer;
        private readonly DatabaseConfigurtaion mConfiguration;
        private readonly IIDProducer mIdProducer;
        private readonly ILogger<AppDbContext> mLogger;

        private readonly string NextIdPath;
        public List<ITasksGroup> Entities { get; private set; } = new List<ITasksGroup>();
        public string DatabaseFilePath { get; }
        public string DefaultTasksGroup { get => mConfiguration.DefaultTasksGroup; }
        public string NotesDirectoryPath { get => mConfiguration.NotesDirectoryPath; }
        public string NotesTasksDirectoryPath { get => mConfiguration.NotesTasksDirectoryPath; }

        public AppDbContext(IOptions<DatabaseConfigurtaion> configuration,
            IObjectSerializer serializer,
            IIDProducer idProducer,
            ILogger<AppDbContext> logger)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            mConfiguration = configuration.Value;

            mSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            mIdProducer = idProducer ?? throw new ArgumentNullException(nameof(idProducer));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (!Directory.Exists(mConfiguration.DatabaseDirectoryPath))
            {
                mLogger.LogError($"No database directory found in path {mConfiguration.DatabaseDirectoryPath}");
                return;
            }

            DatabaseFilePath = Path.Combine(mConfiguration.DatabaseDirectoryPath, DatabaseName);
            NextIdPath = Path.Combine(mConfiguration.DatabaseDirectoryPath, NextIdHolderName);
        }

        public Task LoadDatabase()
        {
            try
            {
                LoadTaskskGroups();
                LoadNextIdToProduce();
            }
            catch (Exception ex)
            {
                mLogger.LogError("Unable to deserialize whole information", ex);
            }

            return Task.CompletedTask;
        }

        private void LoadTaskskGroups()
        {
            if (!File.Exists(DatabaseFilePath))
            {
                mLogger.LogError($"Database file {DatabaseFilePath} does not exists");
                throw new FileNotFoundException("Database does not exists", DatabaseFilePath);
            }

            mLogger.LogInformation($"Going to load database from {DatabaseFilePath}");
            Entities = mSerializer.Deserialize<List<ITasksGroup>>(DatabaseFilePath);
        }

        private void LoadNextIdToProduce()
        {
            if (!File.Exists(NextIdPath))
            {
                mLogger.LogError($"Database file {NextIdPath} does not exists");
                throw new FileNotFoundException("Database does not exists", NextIdPath);
            }

            mLogger.LogInformation("Going to load next id");
            mIdProducer.SetNextID(mSerializer.Deserialize<int>(NextIdPath));
        }

        public Task SaveCurrentDatabase()
        {
            if (string.IsNullOrEmpty(DatabaseFilePath))
            {
                mLogger.LogError("No database path was given");
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(NextIdPath))
            {
                mLogger.LogError("No next id path was given");
                return Task.CompletedTask;
            }

            try
            {
                SaveTasksGroups();
                SaveNextId();
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Unable to serialize database in {mConfiguration.DatabaseDirectoryPath}", ex);
            }

            return Task.CompletedTask;
        }

        private void SaveTasksGroups()
        {
            mSerializer.Serialize(Entities, DatabaseFilePath);
        }

        private void SaveNextId()
        {
            mSerializer.Serialize(mIdProducer.PeekForNextId(), NextIdPath);
        }
    }
}