using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WakeCommerce.API.Logging;

namespace WakeCommerce.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class LogsController : ControllerBase
{
    private readonly IMemoryLogStore _logStore;

    public LogsController(IMemoryLogStore logStore)
    {
        _logStore = logStore;
    }

    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MemoryLogEntry>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(_logStore.GetEntries());
    }
}
