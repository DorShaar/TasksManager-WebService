using Logger.Contracts;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace Tasker.Api.Controllers
{
    [Route("api/[controller]")]
    public class OpenIdConnectController : Controller
    {
        private readonly ILogger mLogger;
        private readonly IClientRequestParametersProvider mClientRequestParametersProvider;

        public OpenIdConnectController(IClientRequestParametersProvider clientRequestParametersProvider,
            ILogger logger)
        {
            mClientRequestParametersProvider = clientRequestParametersProvider;
            mLogger = logger;
        }

        [HttpGet("{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            mLogger.Log($"Getting client parameters for client id {clientId}");
            var parameters = mClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);
        }
    }
}