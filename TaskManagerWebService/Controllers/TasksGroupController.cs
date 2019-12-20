using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFirstWebApp.Domain.Models;
using MyFirstWebApp.Domain.Services;
using MyFirstWebApp.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Extensions;
using TaskManagerWebService.Resources;

namespace MyFirstWebApp.Controllers
{
    [Route("/api/[controller]")]
    public class TasksGroupController : Controller
    {
        private readonly ITasksGroupService mTaskService;
        private readonly IMapper mMapper;

        public TasksGroupController(ITasksGroupService taskService, IMapper mapper)
        {
            mTaskService = taskService;
            mMapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<TasksGroupResource>> GetAllAsync()
        {
            IEnumerable<TasksGroup> groups = await mTaskService.ListAsync();
            return mMapper
                .Map<IEnumerable<TasksGroup>, IEnumerable<TasksGroupResource>>(groups);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveTasksGroupResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());
        }
    }
}