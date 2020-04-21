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

            IEnumerable<IWorkTask> tasks = await mWorkTaskService.ListAsync();

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

            ITasksGroup group = (await mTasksGroupService.ListAsync())
                .Where(group => group.ID.Equals(groupId)).SingleOrDefault();

            if (group == null)
                return workTaskResources;

            workTaskResources = mMapper.Map<IEnumerable<IWorkTask>, IEnumerable<WorkTaskResource>>(group.GetAllTasks());

            mLogger.Log($"Found {workTaskResources.Count()} work tasks");

            return workTaskResources;
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
                        await mTasksGroupService.SaveTaskAsync(newWorkTaskResource.GroupName, newWorkTaskResource.Description);

                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                WorkTaskResource workTaskResource = mMapper.Map<IWorkTask, WorkTaskResource>(result.ResponseObject);
                return Ok(workTaskResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Put operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}