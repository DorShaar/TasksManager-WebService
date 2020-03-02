using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tasker.Domain.Models;

namespace Tasker.Infra.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        private const int MAXIMAL_GROUP_NAME_LENGTH = 30;
        private const int MAXIMAL_TASK_NAME_LENGTH = 60;

        public DbSet<TasksGroup> TasksGroups { get; set; }
        public DbSet<WorkTask> WorkTasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            BuildTasksGroupEntity(builder);
            BuildTasksEntity(builder);
        }

        private void BuildTasksGroupEntity(ModelBuilder builder)
        {
            builder.Entity<TasksGroup>().ToTable("TasksGroups");
            builder.Entity<TasksGroup>().HasKey(p => p.ID);
            builder.Entity<TasksGroup>().Property(p => p.ID).IsRequired(true).ValueGeneratedOnAdd();
            builder.Entity<TasksGroup>().Property(p => p.Name).IsRequired(true).HasMaxLength(MAXIMAL_GROUP_NAME_LENGTH);
            builder.Entity<TasksGroup>().HasMany(p => p.Tasks).WithOne(p => p.ParentGroup).HasForeignKey(p => p.ID);

            builder.Entity<TasksGroup>().HasData(CreateTasksGroups());
        }

        private TasksGroup[] CreateTasksGroups()
        {
            List<TasksGroup> tasksGroups = new List<TasksGroup>
            {
                new TasksGroup { ID = "100", Name = "Free-Tasks" },
                new TasksGroup { ID = "101", Name = "Work" }
            };

            return tasksGroups.ToArray();
        }

        private void BuildTasksEntity(ModelBuilder builder)
        {
            builder.Entity<WorkTask>().ToTable("Tasks");
            builder.Entity<WorkTask>().HasKey(task => task.ID);
            builder.Entity<WorkTask>().Property(task => task.ID).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<WorkTask>().Property(task => task.ID).IsRequired().HasMaxLength(MAXIMAL_TASK_NAME_LENGTH);

            builder.Entity<WorkTask>().HasData(CreateTasks());
        }

        private WorkTask[] CreateTasks()
        {
            List<WorkTask> tasksGroups = new List<WorkTask>
            {
                new WorkTask { ID = "100", Name = "Clean house" },
                new WorkTask { ID = "101", Name = "Update License" }
            };

            return tasksGroups.ToArray();
        }
    }
}