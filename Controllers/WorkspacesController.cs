using FinanzasApi.Services;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;
    private readonly ILogger<WorkspacesController> _logger;

    public WorkspacesController(IWorkspaceService workspaceService, ILogger<WorkspacesController> logger)
    {
        _workspaceService = workspaceService;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkspaceDto>> GetById(Guid id)
    {
        try
        {
            var workspace = await _workspaceService.GetByIdAsync(id);
            if (workspace == null) return NotFound(new { message = "Workspace no encontrado" });
            return Ok(workspace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el workspace {WorkspaceId}", id);
            return StatusCode(500, new { message = "Error al obtener el workspace" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WorkspaceDto>> Create([FromBody] CreateWorkspaceRequest request)
    {
        try
        {
            var userId = GetUserId();
            var workspace = await _workspaceService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el workspace");
            return StatusCode(500, new { message = "Error al crear el workspace" });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkspaceDto>> Update(Guid id, [FromBody] UpdateWorkspaceRequest request)
    {
        try
        {
            var workspace = await _workspaceService.UpdateAsync(id, request);
            if (workspace == null) return NotFound(new { message = "Workspace no encontrado" });
            return Ok(workspace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el workspace {WorkspaceId}", id);
            return StatusCode(500, new { message = "Error al actualizar el workspace" });
        }
    }

    private Guid GetUserId() => Guid.Parse("00000000-0000-0000-0000-000000000001");
}
