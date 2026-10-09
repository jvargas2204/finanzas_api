using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetAllAsync(Guid workspaceId)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId)
            .ToListAsync();

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, Guid workspaceId)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, Guid workspaceId)
    {
        var category = new Category
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Flow = Enum.Parse<FlowType>(request.Flow, true),
            ParentId = request.ParentId,
            IsTaxDeductible = request.IsTaxDeductible
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request, Guid workspaceId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        if (category == null) return null;

        if (request.Name != null) category.Name = request.Name;
        if (request.Flow != null) category.Flow = Enum.Parse<FlowType>(request.Flow, true);
        if (request.ParentId.HasValue) category.ParentId = request.ParentId;
        if (request.IsTaxDeductible.HasValue) category.IsTaxDeductible = request.IsTaxDeductible.Value;
        if (request.IsActive.HasValue) category.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid workspaceId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        if (category == null) return false;

        category.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static CategoryDto MapToDto(Category c) => new(
        c.Id,
        c.WorkspaceId,
        c.ParentId,
        c.Name,
        c.Flow.ToString(),
        c.IsTaxDeductible,
        c.IsActive
    );
}
