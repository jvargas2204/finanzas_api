using FinanzasApi.Models.DTOs;
using FinanzasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(
        IAccountService accountService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las cuentas del workspace
    /// </summary>
    [HttpGet("{workspaceId:guid}")]
    [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AccountDto>>> GetAll(Guid workspaceId)
    {
        try
        {
            var accounts = await _accountService.GetAllAsync(workspaceId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las cuentas");
            return StatusCode(500, new { message = "Error al obtener las cuentas" });
        }
    }

    /// <summary>
    /// Obtiene una cuenta por su ID
    /// </summary>
    [HttpGet("{id:guid}/{workspaceId:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetById(Guid id, Guid workspaceId)
    {
        try
        {
            var account = await _accountService.GetByIdAsync(id, workspaceId);

            if (account == null)
                return NotFound(new { message = "Cuenta no encontrada" });

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la cuenta {AccountId}", id);
            return StatusCode(500, new { message = "Error al obtener la cuenta" });
        }
    }

    /// <summary>
    /// Crea una nueva cuenta
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request)
    {
        try
        {
            var workspaceId = await GetWorkspaceId();
            var account = await _accountService.CreateAsync(request, workspaceId);

            return CreatedAtAction(nameof(GetById), new { id = account.Id, workspaceId }, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la cuenta");
            return StatusCode(500, new { message = "Error al crear la cuenta" });
        }
    }

    /// <summary>
    /// Actualiza una cuenta existente
    /// </summary>
    [HttpPut("{id:guid}/{workspaceId:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> Update(Guid id, Guid workspaceId, [FromBody] UpdateAccountRequest request)
    {
        try
        {
            var account = await _accountService.UpdateAsync(id, request, workspaceId);

            if (account == null)
                return NotFound(new { message = "Cuenta no encontrada" });

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la cuenta {AccountId}", id);
            return StatusCode(500, new { message = "Error al actualizar la cuenta" });
        }
    }

    /// <summary>
    /// Elimina una cuenta (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}/{workspaceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, Guid workspaceId)
    {
        try
        {
            var result = await _accountService.DeleteAsync(id, workspaceId);

            if (!result)
                return NotFound(new { message = "Cuenta no encontrada" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la cuenta {AccountId}", id);
            return StatusCode(500, new { message = "Error al eliminar la cuenta" });
        }
    }

    private async Task<Guid> GetWorkspaceId()
    {
        return await _workspaceResolver.GetDefaultWorkspaceIdAsync();
    }
}
