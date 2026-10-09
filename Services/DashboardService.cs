using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(Guid workspaceId)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateOnly(now.Year, now.Month, 1);

        // Total balance across all accounts
        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.WorkspaceId == workspaceId && a.IsActive)
            .ToListAsync();

        var totalBalance = 0m;
        foreach (var account in accounts)
        {
            var income = await _context.Transactions
                .Where(t => t.AccountId == account.Id && t.Flow == FlowType.Income && t.Status == TxnStatus.Cleared)
                .SumAsync(t => t.Amount);

            var expense = await _context.Transactions
                .Where(t => t.AccountId == account.Id && t.Flow == FlowType.Expense && t.Status == TxnStatus.Cleared)
                .SumAsync(t => t.Amount);

            totalBalance += account.OpeningBalance + income - expense;
        }

        // Monthly income
        var monthlyIncome = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId 
                && t.Flow == FlowType.Income 
                && t.Status == TxnStatus.Cleared
                && t.OccurredOn >= startOfMonth
                && t.TransferGroupId == null)
            .SumAsync(t => t.AmountBase);

        // Monthly expense
        var monthlyExpense = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId 
                && t.Flow == FlowType.Expense 
                && t.Status == TxnStatus.Cleared
                && t.OccurredOn >= startOfMonth
                && t.TransferGroupId == null)
            .SumAsync(t => t.AmountBase);

        // Pending count
        var pendingCount = await _context.Transactions
            .AsNoTracking()
            .CountAsync(t => t.WorkspaceId == workspaceId && t.Status == TxnStatus.Pending);

        // Budget alerts
        var budgetAlerts = await _context.BudgetAlerts
            .AsNoTracking()
            .CountAsync(a => a.Budget.WorkspaceId == workspaceId && a.AcknowledgedAt == null);

        // Recent transactions
        var recentTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId)
            .OrderByDescending(t => t.OccurredOn)
            .ThenByDescending(t => t.CreatedAt)
            .Take(5)
            .ToListAsync();

        // Top categories this month
        var categorySpending = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId 
                && t.Flow == FlowType.Expense 
                && t.Status == TxnStatus.Cleared
                && t.OccurredOn >= startOfMonth
                && t.TransferGroupId == null
                && t.CategoryId != null)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.AmountBase) })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToListAsync();

        var totalCategorySpending = categorySpending.Sum(c => c.Total);
        var colors = new[] { "#6366f1", "#8b5cf6", "#ec4899", "#f59e0b", "#10b981" };

        var topCategories = new List<CategorySpendingDto>();
        for (int i = 0; i < categorySpending.Count; i++)
        {
            var cat = categorySpending[i];
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cat.CategoryId);
            topCategories.Add(new CategorySpendingDto(
                cat.CategoryId!.Value,
                category?.Name ?? "Otros",
                cat.Total,
                totalCategorySpending > 0 ? (decimal)(cat.Total / totalCategorySpending * 100) : 0,
                colors[i % colors.Length]
            ));
        }

        // Monthly trend (last 6 months)
        var monthlyTrend = new List<MonthlyTrendDto>();
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = startOfMonth.AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);

            var income = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.WorkspaceId == workspaceId 
                    && t.Flow == FlowType.Income 
                    && t.Status == TxnStatus.Cleared
                    && t.OccurredOn >= monthStart 
                    && t.OccurredOn < monthEnd
                    && t.TransferGroupId == null)
                .SumAsync(t => t.AmountBase);

            var expense = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.WorkspaceId == workspaceId 
                    && t.Flow == FlowType.Expense 
                    && t.Status == TxnStatus.Cleared
                    && t.OccurredOn >= monthStart 
                    && t.OccurredOn < monthEnd
                    && t.TransferGroupId == null)
                .SumAsync(t => t.AmountBase);

            monthlyTrend.Add(new MonthlyTrendDto(
                monthStart.ToString("MMM"),
                income,
                expense,
                income - expense
            ));
        }

        return new DashboardSummaryDto(
            totalBalance,
            monthlyIncome,
            monthlyExpense,
            monthlyIncome - monthlyExpense,
            pendingCount,
            budgetAlerts,
            recentTransactions.Select(t => new TransactionDto(
                t.Id,
                t.WorkspaceId,
                t.AccountId,
                t.Account?.Name ?? "",
                t.CategoryId,
                t.Category?.Name,
                t.ContactId,
                t.Contact?.Name,
                t.Flow.ToString(),
                t.Status.ToString(),
                t.OccurredOn,
                t.DueOn,
                t.Amount,
                t.CurrencyCode,
                t.FxRate,
                t.AmountBase,
                t.Description,
                t.TransferGroupId,
                t.Source.ToString(),
                t.ExternalId,
                t.Metadata,
                t.CreatedAt,
                t.UpdatedAt
            )).ToList(),
            topCategories,
            monthlyTrend
        );
    }
}
