using FinanzasApi.Models.DTOs;
using FinanzasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(
        IBudgetService budgetService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<BudgetsController> logger
    )
    {
        _budgetService = budgetService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    [HttpGet("{workspaceId:guid}")]
    public async Task<ActionResult<List<BudgetDto>>> GetAll(Guid workspaceId)
    {
        try
        {
            var budgets = await _budgetService.GetAllAsync(workspaceId);
            return Ok(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los presupuestos");
            return StatusCode(500, new { message = "Error al obtener los presupuestos" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create([FromBody] CreateBudgetRequest request)
    {
        try
        {
            var workspaceId = await _workspaceResolver.GetDefaultWorkspaceIdAsync();
            var budget = await _budgetService.CreateAsync(request, workspaceId);
            return CreatedAtAction(nameof(GetAll), new { workspaceId }, budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el presupuesto");
            return StatusCode(500, new { message = "Error al crear el presupuesto" });
        }
    }

    [HttpPut("{id:guid}/{workspaceId:guid}")]
    public async Task<ActionResult<BudgetDto>> Update(Guid id, Guid workspaceId, [FromBody] UpdateBudgetRequest request)
    {
        try
        {
            var budget = await _budgetService.UpdateAsync(id, request, workspaceId);
            if (budget == null) return NotFound(new { message = "Presupuesto no encontrado" });
            return Ok(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el presupuesto {BudgetId}", id);
            return StatusCode(500, new { message = "Error al actualizar el presupuesto" });
        }
    }

    [HttpDelete("{id:guid}/{workspaceId:guid}")]
    public async Task<IActionResult> Delete(Guid id, Guid workspaceId)
    {
        try
        {
            var result = await _budgetService.DeleteAsync(id, workspaceId);
            if (!result) return NotFound(new { message = "Presupuesto no encontrado" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el presupuesto {BudgetId}", id);
            return StatusCode(500, new { message = "Error al eliminar el presupuesto" });
        }
    }
}
