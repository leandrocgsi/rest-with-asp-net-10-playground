using Microsoft.AspNetCore.Mvc;

namespace RestWithASPNET10Erudio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestLogsController : ControllerBase
    {
        private readonly ILogger<TestLogsController> _logger;

        public TestLogsController(ILogger<TestLogsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test-logs")]
        public IActionResult TestLogs()
        {
            _logger.LogDebug("This is a DEBUG log");
            _logger.LogInformation("This is an INFO log");
            _logger.LogWarning("This is a WARNING log");
            _logger.LogError("This is an ERROR log");
            _logger.LogCritical("This is a CRITICAL log");

            return Ok("Logs generated successfully!");
        }
    }
}
