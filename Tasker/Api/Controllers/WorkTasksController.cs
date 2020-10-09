using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
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
        private readonly ILogger<WorkTasksController> mLogger;

        public WorkTasksController(ITasksGroupService taskService, IWorkTaskService workTaskService,
            IMapper mapper, ILogger<WorkTasksController> logger)
        {
            mTasksGroupService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            mWorkTaskService = workTaskService ?? throw new ArgumentNullException(nameof(workTaskService));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IEnumerable<WorkTaskResource>> ListTasksAsync()
        {
            IEnumerable<WorkTaskResource> workTaskResources = new List<WorkTaskResource>();

            mLogger.LogDebug("Requesting all tasks");

            IEnumerable<IWorkTask> tasks = await mWorkTaskService.ListAllAsync().ConfigureAwait(false);

            if (tasks == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(tasks);

            mLogger.LogDebug($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
        }

        [HttpGet("{groupId}")]
        public async Task<IEnumerable<WorkTaskResource>> ListTasksOfSpecificGroupAsync(string groupId)
        {
            IEnumerable<WorkTaskResource> workTaskResources = new List<WorkTaskResource>();

            if (string.IsNullOrEmpty(groupId))
                return workTaskResources;

            mLogger.LogDebug($"Requesting all tasks from group id {groupId}");

            ITasksGroup group = (await mTasksGroupService.ListAsync().ConfigureAwait(false))
                .SingleOrDefault(group => group.ID.Equals(groupId));

            if (group == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(group.GetAllTasks());

            mLogger.LogDebug($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostTaskAsync(string id, [FromBody] WorkTaskResource saveWorkTaskResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (saveWorkTaskResource == null)
                return BadRequest("Work task resource is null");

            mLogger.LogDebug($"Requesting updating work task id {id}");

            try
            {
                saveWorkTaskResource.TaskId = id;
                IResponse<IWorkTask> result =
                    await mTasksGroupService.UpdateTaskAsync(saveWorkTaskResource).ConfigureAwait(false);

                mLogger.LogDebug($"Update result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Update operation failed with error");
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

            mLogger.LogDebug($"Requesting putting new task {newWorkTaskResource.Description} to group {newWorkTaskResource.GroupName}");

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
                mLogger.LogError(ex, "Put operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTask(string id)
        {
            mLogger.LogDebug($"Requesting deleting task id {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            try
            {
                IResponse<IWorkTask> result = await mTasksGroupService.RemoveTaskAsync(id).ConfigureAwait(false);

                mLogger.LogDebug($"Remove result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Remove operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}