using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tasker.App.Services;

namespace Takser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudServicesController : Controller
    {
        private readonly ILogger<CloudServicesController> mLogger;
        private readonly ICloudService mCloudService;

        public CloudServicesController(ICloudService cloudService,
            ILogger<CloudServicesController> logger)
        {
            mCloudService = cloudService ?? throw new ArgumentNullException(nameof(cloudService));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPut]
        public async Task<IActionResult> SaveDatabaseAsync()
        {
            mLogger.LogDebug("Requesting saving database");

            try
            {
                bool uploadSuccess = await mCloudService.Upload().ConfigureAwait(false);

                if (!uploadSuccess)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return Ok();
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Put operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}