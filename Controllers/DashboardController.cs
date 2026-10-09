using FinanzasApi.Services;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<DashboardController> logger
    )
    {
        _dashboardService = dashboardService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    [HttpGet("summary/{workspaceId:guid}")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(Guid workspaceId)
    {
        try
        {
            var summary = await _dashboardService.GetSummaryAsync(workspaceId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el resumen del dashboard");
            return StatusCode(500, new { message = "Error al obtener el resumen del dashboard" });
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDefaultSummary()
    {
        try
        {
            var workspaceId = await _workspaceResolver.GetDefaultWorkspaceIdAsync();
            var summary = await _dashboardService.GetSummaryAsync(workspaceId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el resumen del dashboard");
            return StatusCode(500, new { message = "Error al obtener el resumen del dashboard" });
        }
    }
}
