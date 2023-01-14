using Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Api
{
    public class Function
    {
        private readonly ILogger _logger;
        private readonly IRumble _rumble;

        public Function(ILoggerFactory loggerFactory, IRumble rumble)
        {
            _logger = loggerFactory.CreateLogger<Function>();
            _rumble = rumble;
        }

        [Function("TestFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "rumblerss/{query}")] HttpRequestData req, string query)
        {
            var channel = await _rumble.Get(query);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(channel);

            return response;
        }
    }
}
