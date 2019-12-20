using Microsoft.EntityFrameworkCore;
using TaskManagerWebService.Domain.Models;
using System.Collections.Generic;

namespace TaskManagerWebService.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        private const int MAXIMAL_GROUP_NAME_LENGTH = 30;
        private const int MAXIMAL_TASK_NAME_LENGTH = 60;

        public DbSet<TasksGroup> TasksGroups { get; set; }

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
            builder.Entity<TasksGroup>().HasKey(p => p.GroupId);
            builder.Entity<TasksGroup>().Property(p => p.GroupId).IsRequired(true).ValueGeneratedOnAdd();
            builder.Entity<TasksGroup>().Property(p => p.GroupName).IsRequired(true).HasMaxLength(MAXIMAL_GROUP_NAME_LENGTH);
            builder.Entity<TasksGroup>().HasMany(p => p.Tasks).WithOne(p => p.ParentGroup).HasForeignKey(p => p.GroupId);

            builder.Entity<TasksGroup>().HasData(CreateTasksGroups());
        }

        private TasksGroup[] CreateTasksGroups()
        {
            List<TasksGroup> tasksGroups = new List<TasksGroup>
            {
                new TasksGroup { GroupId = "100", GroupName = "Free-Tasks" },
                new TasksGroup { GroupId = "101", GroupName = "Work" }
            };

            return tasksGroups.ToArray();
        }

        private void BuildTasksEntity(ModelBuilder builder)
        {
            builder.Entity<WorkTask>().ToTable("Tasks");
            builder.Entity<WorkTask>().HasKey(p => p.TaskId);
            builder.Entity<WorkTask>().Property(p => p.TaskId).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<WorkTask>().Property(p => p.Name).IsRequired().HasMaxLength(MAXIMAL_TASK_NAME_LENGTH);
        }
    }
}