using FinanzasApi.Application.Services;
using FinanzasApi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        ITransactionService transactionService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TransactionDto>>> GetAll([FromQuery] TransactionFilterRequest filter)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var result = await _transactionService.GetAllAsync(workspaceId, filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las transacciones");
            return StatusCode(500, new { message = "Error al obtener las transacciones" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var transaction = await _transactionService.GetByIdAsync(id, workspaceId);
            if (transaction == null) return NotFound(new { message = "Transacción no encontrada" });
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la transacción {TransactionId}", id);
            return StatusCode(500, new { message = "Error al obtener la transacción" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var userId = GetUserId();
            var transaction = await _transactionService.CreateAsync(request, workspaceId, userId);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la transacción");
            return StatusCode(500, new { message = "Error al crear la transacción" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var transaction = await _transactionService.UpdateAsync(id, request, workspaceId);
            if (transaction == null) return NotFound(new { message = "Transacción no encontrada" });
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la transacción {TransactionId}", id);
            return StatusCode(500, new { message = "Error al actualizar la transacción" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var result = await _transactionService.DeleteAsync(id, workspaceId);
            if (!result) return NotFound(new { message = "Transacción no encontrada" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la transacción {TransactionId}", id);
            return StatusCode(500, new { message = "Error al eliminar la transacción" });
        }
    }

    private async Task<Guid> GetWorkspaceId()
    {
        return await _workspaceResolver.GetDefaultWorkspaceIdAsync();
    }

    private Guid? GetUserId() => null;
}
