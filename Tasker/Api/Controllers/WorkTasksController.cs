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
    [ApiController]
    public class WorkTasksController : Controller
    {
        private readonly ITasksGroupService mTasksGroupService;
        private readonly IWorkTaskService mWorkTaskService;
        private readonly IMapper mMapper;
        private readonly ILogger mLogger;

        public WorkTasksController(ITasksGroupService taskService, IWorkTaskService workTaskService,
            IMapper mapper, ILogger logger)
        {
            mTasksGroupService = taskService;
            mWorkTaskService = workTaskService;
            mMapper = mapper;
            mLogger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WorkTaskResource>> ListTasksAsync()
        {
            IEnumerable<WorkTaskResource> workTaskResources = new List<WorkTaskResource>();

            mLogger.Log("Requesting all tasks");

            IEnumerable<IWorkTask> tasks = await mWorkTaskService.ListAsync().ConfigureAwait(false);

            if (tasks == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(tasks);

            mLogger.Log($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
        }

        [HttpGet("{groupId}")]
        public async Task<IEnumerable<WorkTaskResource>> ListTasksOfSpecificGroupAsync(string groupId)
        {
            IEnumerable<WorkTaskResource> workTaskResources = new List<WorkTaskResource>();

            if (string.IsNullOrEmpty(groupId))
                return workTaskResources;

            mLogger.Log($"Requesting all tasks from group id {groupId}");

            ITasksGroup group = (await mTasksGroupService.ListAsync().ConfigureAwait(false))
                .SingleOrDefault(group => group.ID.Equals(groupId));

            if (group == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(group.GetAllTasks());

            mLogger.Log($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostTaskAsync(string id, [FromBody] WorkTaskResource saveWorkTaskResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (saveWorkTaskResource == null)
                return BadRequest("Work task resource is null");

            mLogger.Log($"Requesting updating work task id {id}");

            try
            {
                saveWorkTaskResource.TaskId = id;
                IResponse<IWorkTask> result =
                    await mTasksGroupService.UpdateTaskAsync(saveWorkTaskResource).ConfigureAwait(false);

                mLogger.Log($"Update result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError("Update operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutWorkTaskAsync([FromBody] WorkTaskResource newWorkTaskResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (newWorkTaskResource == null)
                return BadRequest("New work task resource is null");

            mLogger.Log($"Requesting putting new task {newWorkTaskResource.Description} to group {newWorkTaskResource.GroupName}");

            try
            {
                IResponse<IWorkTask> result =
                        await mTasksGroupService.SaveTaskAsync(newWorkTaskResource.GroupName, newWorkTaskResource.Description)
                        .ConfigureAwait(false);

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError("Put operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTask(string id)
        {
            mLogger.Log($"Requesting deleting task id {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            try
            {
                IResponse<IWorkTask> result = await mTasksGroupService.RemoveTaskAsync(id).ConfigureAwait(false);

                mLogger.Log($"Remove result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError("Remove operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}