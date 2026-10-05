using FinanzasApi.Models.DTOs;
using FinanzasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(
        IContactService contactService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<ContactsController> logger
    )
    {
        _contactService = contactService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    [HttpGet("{workspaceId:guid}")]
    public async Task<ActionResult<List<ContactDto>>> GetAll(Guid workspaceId)
    {
        try
        {
            var contacts = await _contactService.GetAllAsync(workspaceId);
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los contactos");
            return StatusCode(500, new { message = "Error al obtener los contactos" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactRequest request)
    {
        try
        {
            var workspaceId = await _workspaceResolver.GetDefaultWorkspaceIdAsync();
            var contact = await _contactService.CreateAsync(request, workspaceId);
            return CreatedAtAction(nameof(GetAll), new { workspaceId }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el contacto");
            return StatusCode(500, new { message = "Error al crear el contacto" });
        }
    }

    [HttpPut("{id:guid}/{workspaceId:guid}")]
    public async Task<ActionResult<ContactDto>> Update(Guid id, Guid workspaceId, [FromBody] UpdateContactRequest request)
    {
        try
        {
            var contact = await _contactService.UpdateAsync(id, request, workspaceId);
            if (contact == null) return NotFound(new { message = "Contacto no encontrado" });
            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el contacto {ContactId}", id);
            return StatusCode(500, new { message = "Error al actualizar el contacto" });
        }
    }

    [HttpDelete("{id:guid}/{workspaceId:guid}")]
    public async Task<IActionResult> Delete(Guid id, Guid workspaceId)
    {
        try
        {
            var result = await _contactService.DeleteAsync(id, workspaceId);
            if (!result) return NotFound(new { message = "Contacto no encontrado" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el contacto {ContactId}", id);
            return StatusCode(500, new { message = "Error al eliminar el contacto" });
        }
    }
}
