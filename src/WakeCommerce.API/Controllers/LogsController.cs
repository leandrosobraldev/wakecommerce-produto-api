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

    /// <summary>
    /// Retorna os últimos 500 registros de log (data, hora, nível, requisição, mensagem).
    /// Útil para visualização em desenvolvimento. Em produção considere desabilitar ou restringir.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MemoryLogEntry>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(_logStore.GetEntries());
    }
}
