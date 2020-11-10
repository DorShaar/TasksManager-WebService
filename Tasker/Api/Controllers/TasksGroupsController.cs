using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData.TasksGroups;
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
        private readonly IMapper mMapper;
        private readonly ILogger<TasksGroupsController> mLogger;

        public TasksGroupsController(ITasksGroupService taskService,
            IMapper mapper,
            ILogger<TasksGroupsController> logger)
        {
            mTasksGroupService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IEnumerable<TasksGroupResource>> ListGroupsAsync()
        {
            mLogger.LogDebug("Requesting groups");

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync().ConfigureAwait(false);

            IEnumerable<TasksGroupResource> taskGroupResources = mMapper
                .Map<IEnumerable<ITasksGroup>, IEnumerable<TasksGroupResource>>(groups);

            mLogger.LogDebug($"Found {taskGroupResources.Count()} groups");

            return taskGroupResources;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostGroupAsync(string id, [FromBody] TasksGroupResource saveTasksGroupResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (saveTasksGroupResource == null)
                return BadRequest("New group name resource is null");

            mLogger.LogDebug($"Requesting updating group id {id} with name {saveTasksGroupResource.GroupName}");

            try
            {
                IResponse<ITasksGroup> result =
                    await mTasksGroupService.UpdateGroupAsync(id, saveTasksGroupResource.GroupName).ConfigureAwait(false);

                mLogger.LogDebug($"Update result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Update operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutTaskGroupAsync([FromBody] TasksGroupResource newTaskGroupResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (newTaskGroupResource == null)
                return BadRequest("New tasks group resource is null");

            mLogger.LogDebug($"Requesting putting new group {newTaskGroupResource.GroupName}");

            try
            {
                IResponse<ITasksGroup> result =
                    await mTasksGroupService.SaveAsync(newTaskGroupResource.GroupName).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Put operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroup(string id)
        {
            mLogger.LogDebug($"Requesting deleting group id {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            try
            {
                IResponse<ITasksGroup> result = await mTasksGroupService.RemoveAsync(id).ConfigureAwait(false);

                mLogger.LogDebug($"Remove result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch(Exception ex)
            {
                mLogger.LogError(ex, "Remove operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}