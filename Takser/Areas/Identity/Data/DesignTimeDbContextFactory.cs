using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Tasker.Data;

namespace Takser.Areas.Identity.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskerContext>
    {
        public TaskerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<TaskerContext>();
            var connectionString = configuration.GetConnectionString("TaskerContextConnection");
            builder.UseSqlServer(connectionString);
            return new TaskerContext(builder.Options);
        }
    }
}