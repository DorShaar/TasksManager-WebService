using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Extensions;
using Tasker.Domain.Models;

namespace Takser.Api.Controllers
{
    [Route("api/[controller]")]
    public class TasksGroupsController : Controller
    {
        private readonly ITasksGroupService mTasksGroupService;
        private readonly IMapper mMapper;

        public TasksGroupsController(ITasksGroupService taskService, IMapper mapper)
        {
            mTasksGroupService = taskService;
            mMapper = mapper;
        }

        [HttpGet("action")]
        public IEnumerable<TasksGroupResource> Groups()
        {
            IEnumerable<TasksGroup> groups = mTasksGroupService.ListAsync().Result;
            return mMapper
                .Map<IEnumerable<TasksGroup>, IEnumerable<TasksGroupResource>>(groups);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveTasksGroupResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            TasksGroup group = mMapper.Map<SaveTasksGroupResource, TasksGroup>(resource);

            TasksGroupResponse result = await mTasksGroupService.SaveAsync(group);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            TasksGroupResource tasksGroupResource = mMapper.Map<TasksGroup, TasksGroupResource>(result.Group);
            return Ok(tasksGroupResource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] SaveTasksGroupResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            TasksGroup tasksGroup = mMapper.Map<SaveTasksGroupResource, TasksGroup>(resource);
            TasksGroupResponse result = await mTasksGroupService.UpdateAsync(id, tasksGroup);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            TasksGroupResource tasksGroupResource = mMapper.Map<TasksGroup, TasksGroupResource>(result.Group);
            return Ok(tasksGroupResource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGroupAsync(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            TasksGroupResponse result = await mTasksGroupService.RemoveAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            TasksGroupResource tasksGroupResource = mMapper.Map<TasksGroup, TasksGroupResource>(result.Group);
            return Ok(tasksGroupResource);
        }
    }
}