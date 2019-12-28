using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Domain.Services;
using TaskManagerWebService.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Extensions;
using TaskManagerWebService.Domain.Services.Communication;

namespace TaskManagerWebService.Controllers
{
    [Route("/api/[controller]")]
    public class TasksGroupController : Controller
    {
        private readonly ITasksGroupService mTasksGroupService;
        private readonly IMapper mMapper;

        public TasksGroupController(ITasksGroupService taskService, IMapper mapper)
        {
            mTasksGroupService = taskService;
            mMapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<TasksGroupResource>> GetAllAsync()
        {
            IEnumerable<TasksGroup> groups = await mTasksGroupService.ListAsync();
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
        public async Task<IActionResult> RemoveAsync(string id)
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