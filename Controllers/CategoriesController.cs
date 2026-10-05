using FinanzasApi.Models.DTOs;
using FinanzasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IWorkspaceResolverService _workspaceResolver;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        IWorkspaceResolverService workspaceResolver,
        ILogger<CategoriesController> logger
    )
    {
        _categoryService = categoryService;
        _workspaceResolver = workspaceResolver;
        _logger = logger;
    }

    [HttpGet("{workspaceId:guid}")]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(Guid workspaceId)
    {
        try
        {
            var categories = await _categoryService.GetAllAsync(workspaceId);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las categorías");
            return StatusCode(500, new { message = "Error al obtener las categorías" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var workspaceId = await _workspaceResolver.GetDefaultWorkspaceIdAsync();
            var category = await _categoryService.CreateAsync(request, workspaceId);
            return CreatedAtAction(nameof(GetAll), new { workspaceId }, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la categoría");
            return StatusCode(500, new { message = "Error al crear la categoría" });
        }
    }

    [HttpPut("{id:guid}/{workspaceId:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, Guid workspaceId, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(id, request, workspaceId);
            if (category == null) return NotFound(new { message = "Categoría no encontrada" });
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la categoría {CategoryId}", id);
            return StatusCode(500, new { message = "Error al actualizar la categoría" });
        }
    }

    [HttpDelete("{id:guid}/{workspaceId:guid}")]
    public async Task<IActionResult> Delete(Guid id, Guid workspaceId)
    {
        try
        {
            var result = await _categoryService.DeleteAsync(id, workspaceId);
            if (!result) return NotFound(new { message = "Categoría no encontrada" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la categoría {CategoryId}", id);
            return StatusCode(500, new { message = "Error al eliminar la categoría" });
        }
    }
}
