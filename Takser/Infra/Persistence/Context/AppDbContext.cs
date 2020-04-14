using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using Takser.Infra.Options;
using TaskData.Contracts;

namespace Tasker.Infra.Persistence.Context
{
    public class AppDbContext
    {
        private const string DatabaseName = "tasks.db";
        private const string NextIdHolderName = "id_producer.db";

        private readonly ILogger mLogger;
        private readonly IObjectSerializer mSerializer;
        private readonly DatabaseConfigurtaion mConfiguration;

        private readonly string NextIdPath;
        public List<ITasksGroup> Entities { get; private set; } = new List<ITasksGroup>();
        public string DatabaseFilePath { get; }
        public string DefaultTasksGroup { get => mConfiguration.DefaultTasksGroup; }
        public string NotesDirectoryPath { get => mConfiguration.NotesDirectoryPath; }
        public string NotesTasksDirectoryPath { get => mConfiguration.NotesTasksDirectoryPath; }

        public AppDbContext(IOptions<DatabaseConfigurtaion> configuration, IObjectSerializer serializer, ILogger logger)
        {
            mConfiguration = configuration.Value;
            mSerializer = serializer;
            mLogger = logger;

            if (!Directory.Exists(mConfiguration.DatabaseDirectoryPath))
            {
                mLogger.LogError($"No database directory found in path {mConfiguration.DatabaseDirectoryPath}");
                return;
            }

            DatabaseFilePath = Path.Combine(mConfiguration.DatabaseDirectoryPath, DatabaseName);
            NextIdPath = Path.Combine(mConfiguration.DatabaseDirectoryPath, NextIdHolderName);
            LoadInformation();
        }

        private void LoadInformation()
        {
            try
            {
                LoadDatabase();
                LoadNextIdToProduce();
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Unable to deserialize whole information", ex);
            }
        }

        private void LoadDatabase()
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
            IDProducer.IDProducer.SetNextID(mSerializer.Deserialize<int>(NextIdPath));
        }
    }
}