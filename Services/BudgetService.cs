using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _context;

    public BudgetService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BudgetDto>> GetAllAsync(Guid workspaceId)
    {
        var budgets = await _context.Budgets
            .AsNoTracking()
            .Where(b => b.WorkspaceId == workspaceId)
            .ToListAsync();

        return budgets.Select(MapToDto).ToList();
    }

    public async Task<BudgetDto?> GetByIdAsync(Guid id, Guid workspaceId)
    {
        var budget = await _context.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.WorkspaceId == workspaceId);

        return budget == null ? null : MapToDto(budget);
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetRequest request, Guid workspaceId)
    {
        var budget = new Budget
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Period = string.IsNullOrEmpty(request.Period) ? BudgetPeriod.Monthly : Enum.Parse<BudgetPeriod>(request.Period, true),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AlertThreshold_pct = request.AlertThresholdPct,
            IncludeSubcategories = request.IncludeSubcategories
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        return MapToDto(budget);
    }

    public async Task<BudgetDto?> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid workspaceId)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.WorkspaceId == workspaceId);

        if (budget == null) return null;

        if (request.Name != null) budget.Name = request.Name;
        if (request.CategoryId.HasValue) budget.CategoryId = request.CategoryId;
        if (request.Amount.HasValue) budget.Amount = request.Amount.Value;
        if (request.Period != null) budget.Period = Enum.Parse<BudgetPeriod>(request.Period, true);
        if (request.StartDate.HasValue) budget.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) budget.EndDate = request.EndDate;
        if (request.AlertThresholdPct.HasValue) budget.AlertThreshold_pct = request.AlertThresholdPct.Value;
        if (request.IncludeSubcategories.HasValue) budget.IncludeSubcategories = request.IncludeSubcategories.Value;
        if (request.IsActive.HasValue) budget.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return MapToDto(budget);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid workspaceId)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.WorkspaceId == workspaceId);

        if (budget == null) return false;

        budget.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static BudgetDto MapToDto(Budget b) => new(
        b.Id,
        b.WorkspaceId,
        b.CategoryId,
        b.Category?.Name,
        b.Name,
        b.Amount,
        b.Period.ToString(),
        b.StartDate,
        b.EndDate,
        b.AlertThreshold_pct,
        b.IncludeSubcategories,
        b.IsActive
    );
}
