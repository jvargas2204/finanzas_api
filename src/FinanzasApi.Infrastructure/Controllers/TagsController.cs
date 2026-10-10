using FinanzasApi.Application.Services;
using FinanzasApi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ILogger<TagsController> _logger;

    public TagsController(ILogger<TagsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{workspaceId:guid}")]
    [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TagDto>>> GetAll(Guid workspaceId)
    {
        try
        {
            // TODO: Implement with actual service
            var tags = new List<TagDto>();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tags");
            return StatusCode(500, new { message = "Error al obtener los tags" });
        }
    }
}
