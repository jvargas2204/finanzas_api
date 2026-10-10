using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.Application.Dtos;

// ============ Workspace DTOs ============
public record WorkspaceDto(
    Guid Id,
    string Name,
    string BaseCurrency,
    short FiscalYearStartMonth,
    DateTime CreatedAt
);

public record CreateWorkspaceRequest(
    [Required] string Name,
    [Required] string BaseCurrency,
    short FiscalYearStartMonth = 1
);

public record UpdateWorkspaceRequest(
    string? Name,
    string? BaseCurrency,
    short? FiscalYearStartMonth
);

// ============ Account DTOs ============
public record AccountDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string Type,
    string CurrencyCode,
    decimal OpeningBalance,
    bool IsActive,
    DateTime CreatedAt,
    decimal? Balance
);

public record CreateAccountRequest(
    [Required] string Name,
    [Required] string Type,
    [Required] string CurrencyCode,
    decimal OpeningBalance = 0
);

public record UpdateAccountRequest(
    string? Name,
    string? Type,
    string? CurrencyCode,
    decimal? OpeningBalance,
    bool? IsActive
);

// ============ Category DTOs ============
public record CategoryDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentId,
    string Name,
    string Flow,
    bool IsTaxDeductible,
    bool IsActive
);

public record CreateCategoryRequest(
    [Required] string Name,
    [Required] string Flow,
    Guid? ParentId = null,
    bool IsTaxDeductible = false
);

public record UpdateCategoryRequest(
    string? Name,
    string? Flow,
    Guid? ParentId,
    bool? IsTaxDeductible,
    bool? IsActive
);

// ============ Contact DTOs ============
public record ContactDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string Kind,
    string? Email,
    string? TaxId,
    DateTime CreatedAt
);

public record CreateContactRequest(
    [Required] string Name,
    [Required] string Kind,
    string? Email = null,
    string? TaxId = null
);

public record UpdateContactRequest(
    string? Name,
    string? Kind,
    string? Email,
    string? TaxId
);

// ============ Tag DTOs ============
public record TagDto(
    Guid Id,
    Guid WorkspaceId,
    string Name
);

public record CreateTagRequest(
    [Required] string Name
);

// ============ Transaction DTOs ============
public record TransactionDto(
    Guid Id,
    Guid WorkspaceId,
    Guid AccountId,
    string AccountName,
    Guid? CategoryId,
    string? CategoryName,
    Guid? ContactId,
    string? ContactName,
    string Flow,
    string Status,
    DateOnly OccurredOn,
    DateOnly? DueOn,
    decimal Amount,
    string CurrencyCode,
    decimal FxRate,
    decimal AmountBase,
    string? Description,
    Guid? TransferGroupId,
    string Source,
    string? ExternalId,
    string Metadata,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTransactionRequest(
    [Required] Guid AccountId,
    [Required] string Flow,
    [Required] DateOnly OccurredOn,
    [Required] [Range(0.01, double.MaxValue)] decimal Amount,
    [Required] string CurrencyCode,
    Guid? CategoryId = null,
    Guid? ContactId = null,
    string Status = "cleared",
    DateOnly? DueOn = null,
    decimal FxRate = 1,
    string? Description = null,
    string Source = "manual",
    string? ExternalId = null,
    Dictionary<string, object>? Metadata = null
);

public record UpdateTransactionRequest(
    Guid? AccountId,
    Guid? CategoryId,
    Guid? ContactId,
    string? Flow,
    string? Status,
    DateOnly? OccurredOn,
    DateOnly? DueOn,
    decimal? Amount,
    string? CurrencyCode,
    decimal? FxRate,
    string? Description,
    string? Source,
    Dictionary<string, object>? Metadata
);

public record TransactionFilterRequest(
    Guid? AccountId,
    Guid? CategoryId,
    Guid? ContactId,
    string? Flow,
    string? Status,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Search,
    List<Guid>? Tags,
    int Page = 1,
    int PageSize = 20
);

// ============ Budget DTOs ============
public record BudgetDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? CategoryId,
    string? CategoryName,
    string Name,
    decimal Amount,
    string Period,
    DateOnly StartDate,
    DateOnly? EndDate,
    short AlertThresholdPct,
    bool IncludeSubcategories,
    bool IsActive
);

public record CreateBudgetRequest(
    [Required] string Name,
    [Required] [Range(0.01, double.MaxValue)] decimal Amount,
    [Required] DateOnly StartDate,
    Guid? CategoryId = null,
    string Period = "monthly",
    DateOnly? EndDate = null,
    short AlertThresholdPct = 80,
    bool IncludeSubcategories = true
);

public record UpdateBudgetRequest(
    string? Name,
    Guid? CategoryId,
    decimal? Amount,
    string? Period,
    DateOnly? StartDate,
    DateOnly? EndDate,
    short? AlertThresholdPct,
    bool? IncludeSubcategories,
    bool? IsActive
);

// ============ Dashboard DTOs ============
public record DashboardSummaryDto(
    decimal TotalBalance,
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    decimal MonthlyNet,
    int PendingCount,
    int BudgetAlerts,
    List<TransactionDto> RecentTransactions,
    List<CategorySpendingDto> TopCategories,
    List<MonthlyTrendDto> MonthlyTrend
);

public record CategorySpendingDto(
    Guid CategoryId,
    string CategoryName,
    decimal Total,
    decimal Percentage,
    string Color
);

public record MonthlyTrendDto(
    string Month,
    decimal Income,
    decimal Expense,
    decimal Net
);

// ============ Currency DTOs ============
public record CurrencyDto(
    string Code,
    string Name,
    short MinorUnits
);

// ============ Paginated Response ============
public record PaginatedResponse<T>(
    List<T> Data,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);

// ============ API Response Wrapper ============
public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    List<string>? Errors = null
);
