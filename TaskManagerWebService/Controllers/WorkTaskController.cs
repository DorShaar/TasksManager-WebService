using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Domain.Services;
using TaskManagerWebService.Resources;

namespace TaskManagerWebService.Controllers
{
    [Route("api/[controller]")]
    public class WorkTaskController : ControllerBase
    {
        private readonly IWorkTaskService mWorkTaskService;
        private readonly IMapper mMapper;

        public WorkTaskController(IWorkTaskService workTaskService, IMapper mapper)
        {
            mWorkTaskService = workTaskService;
            mMapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<WorkTaskResource>> ListAsync()
        {
            IEnumerable<WorkTask> tasks = await mWorkTaskService.ListAsync();
            IEnumerable<WorkTaskResource> workTaskResource = mMapper.Map<IEnumerable<WorkTask>, IEnumerable<WorkTaskResource>>(tasks);
            return workTaskResource;
        }
    }
}