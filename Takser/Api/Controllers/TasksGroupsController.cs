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
        private readonly IMapper mMapper;
        private readonly ILogger mLogger;

        public TasksGroupsController(ITasksGroupService taskService, IMapper mapper, ILogger logger)
        {
            mTasksGroupService = taskService;
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

        [HttpPost("{id}")]
        public async Task<IActionResult> PostGroupAsync(string id, [FromBody] TasksGroupResource saveTasksGroupResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (saveTasksGroupResource == null)
                return BadRequest("New group name resource is null");

            mLogger.Log($"Requesting updating group id {id} with name {saveTasksGroupResource.GroupName}");

            try
            {
                IResponse<ITasksGroup> result = await mTasksGroupService.UpdateGroupAsync(id, saveTasksGroupResource.GroupName);

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

        [HttpPut]
        public async Task<IActionResult> PutWorkTaskAsync([FromBody] TasksGroupResource newTaskGroupResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (newTaskGroupResource == null)
                return BadRequest("New tasks group resource is null");

            mLogger.Log($"Requesting putting new group {newTaskGroupResource.GroupName}");

            try
            {
                IResponse<ITasksGroup> result = await mTasksGroupService.SaveAsync(newTaskGroupResource.GroupName);

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
                return Ok(tasksGroupResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Put operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

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