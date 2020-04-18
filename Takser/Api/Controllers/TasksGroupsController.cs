using AutoMapper;
using Logger.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Extensions;

namespace Takser.Api.Controllers
{
    [Route("api/[controller]")]
    public class TasksGroupsController : Controller
    {
        private readonly ITasksGroupService mTasksGroupService;
        private readonly IWorkTaskService mWorkTaskService;
        private readonly IMapper mMapper;
        private readonly ILogger mLogger;

        public TasksGroupsController(ITasksGroupService taskService, IWorkTaskService workTaskService,
            IMapper mapper, ILogger logger)
        {
            mTasksGroupService = taskService;
            mWorkTaskService = workTaskService;
            mMapper = mapper;
            mLogger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<TasksGroupResource>> ListGroupsAsync()
        {
            mLogger.Log("Requesting groups");

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync();
            
            IEnumerable<TasksGroupResource> taskGroupResources = mMapper
                .Map<IEnumerable<ITasksGroup>, IEnumerable<TasksGroupResource>>(groups);

            mLogger.Log($"Found {taskGroupResources.Count()} groups");

            return taskGroupResources;
        }

        [HttpGet("{groupId}")]
        public async Task<IEnumerable<WorkTaskResource>> ListTasksOfSpecificGroupAsync(string groupId)
        {
            IEnumerable<WorkTaskResource> workTaskResources = new List<WorkTaskResource>();

            if (string.IsNullOrEmpty(groupId))
                return workTaskResources;

            mLogger.Log($"Requesting all tasks from group id {groupId}");

            ITasksGroup group = (await mTasksGroupService.ListAsync())
                .Where(group => group.ID.Equals(groupId)).SingleOrDefault();

            if (group == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(group.GetAllTasks());

            mLogger.Log($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostAsync(string id, [FromBody] SaveTasksGroupResource saveTasksGroupResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (saveTasksGroupResource == null)
                return BadRequest("New group name resource is null");

            mLogger.Log($"Requesting updating group id {id} with name {saveTasksGroupResource.GroupName}");

            try
            {
                IResponse<ITasksGroup> result = await mTasksGroupService.UpdateAsync(id, saveTasksGroupResource.GroupName);

                mLogger.Log($"Update result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Update operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAsync(string id, [FromBody] SaveTasksGroupResource resource)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    ITasksGroup tasksGroup = mMapper.Map<SaveTasksGroupResource, ITasksGroup>(resource);
        //    Response<ITasksGroup> result = await mTasksGroupService.UpdateAsync(id, tasksGroup.Name);

        //    if (!result.IsSuccess)
        //        return BadRequest(result.Message);

        //    TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
        //    return Ok(tasksGroupResource);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroup(string id)
        {
            mLogger.Log($"Requesting deleting group id {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            try
            {
                IResponse<ITasksGroup> result = await mTasksGroupService.RemoveAsync(id);

                mLogger.Log($"Remove result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch(Exception ex)
            {
                mLogger.LogError($"Remove operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}