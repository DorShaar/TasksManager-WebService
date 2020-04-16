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

        [HttpGet("[action]")]
        public async Task<IEnumerable<TasksGroupResource>> Groups()
        {
            mLogger.Log("Requesting groups");

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync();
            
            IEnumerable<TasksGroupResource> taskGroupResources = mMapper
                .Map<IEnumerable<ITasksGroup>, IEnumerable<TasksGroupResource>>(groups);

            mLogger.Log($"Found {taskGroupResources.Count()} groups");

            return taskGroupResources;
        }

        //[HttpPost]
        //public async Task<IActionResult> PostAsync([FromBody] SaveTasksGroupResource resource)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    ITasksGroup group = mMapper.Map<SaveTasksGroupResource, ITasksGroup>(resource);

        //    Response<ITasksGroup> result = await mTasksGroupService.SaveAsync(group.Name);

        //    if (!result.IsSuccess)
        //        return BadRequest(result.Message);

        //    TasksGroupResource tasksGroupResource = mMapper.Map<ITasksGroup, TasksGroupResource>(result.ResponseObject);
        //    return Ok(tasksGroupResource);
        //}

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
                Response<ITasksGroup> result = await mTasksGroupService.RemoveAsync(id);

                mLogger.Log($"Remove result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return Conflict(result.Message);

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