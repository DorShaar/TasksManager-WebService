using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Domain.Models;

namespace Takser.Api.Controllers
{
    [Route("api/[controller]")]
    public class WorkTasksController : ControllerBase
    {
        private readonly IWorkTaskService mWorkTaskService;
        private readonly IMapper mMapper;

        public WorkTasksController(IWorkTaskService workTaskService, IMapper mapper)
        {
            mWorkTaskService = workTaskService;
            mMapper = mapper;
        }

        [HttpGet("action")]
        public async Task<IEnumerable<WorkTaskResource>> Tasks()
        {
            IEnumerable<WorkTask> tasks = await mWorkTaskService.ListAsync();
            IEnumerable<WorkTaskResource> workTaskResource = mMapper.Map<IEnumerable<WorkTask>, IEnumerable<WorkTaskResource>>(tasks);
            return workTaskResource;
        }
    }
}