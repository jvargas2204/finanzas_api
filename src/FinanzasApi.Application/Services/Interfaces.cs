using FinanzasApi.Domain.Entities;
using FinanzasApi.Domain.Enums;
using FinanzasApi.Application.Dtos;

namespace FinanzasApi.Application.Services;

public interface IWorkspaceService
{
    Task<WorkspaceDto?> GetByIdAsync(Guid id);
    Task<WorkspaceDto> CreateAsync(CreateWorkspaceRequest request, Guid userId);
    Task<WorkspaceDto?> UpdateAsync(Guid id, UpdateWorkspaceRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<List<WorkspaceDto>> GetByUserAsync(Guid userId);
}

public interface IAccountService
{
    Task<List<AccountDto>> GetAllAsync(Guid workspaceId);
    Task<AccountDto?> GetByIdAsync(Guid id, Guid workspaceId);
    Task<AccountDto> CreateAsync(CreateAccountRequest request, Guid workspaceId);
    Task<AccountDto?> UpdateAsync(Guid id, UpdateAccountRequest request, Guid workspaceId);
    Task<bool> DeleteAsync(Guid id, Guid workspaceId);
}

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(Guid workspaceId);
    Task<CategoryDto?> GetByIdAsync(Guid id, Guid workspaceId);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, Guid workspaceId);
    Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request, Guid workspaceId);
    Task<bool> DeleteAsync(Guid id, Guid workspaceId);
}

public interface ITransactionService
{
    Task<PaginatedResponse<TransactionDto>> GetAllAsync(Guid workspaceId, TransactionFilterRequest filter);
    Task<TransactionDto?> GetByIdAsync(Guid id, Guid workspaceId);
    Task<TransactionDto> CreateAsync(CreateTransactionRequest request, Guid workspaceId, Guid? userId);
    Task<TransactionDto?> UpdateAsync(Guid id, UpdateTransactionRequest request, Guid workspaceId);
    Task<bool> DeleteAsync(Guid id, Guid workspaceId);
}

public interface IContactService
{
    Task<List<ContactDto>> GetAllAsync(Guid workspaceId);
    Task<ContactDto?> GetByIdAsync(Guid id, Guid workspaceId);
    Task<ContactDto> CreateAsync(CreateContactRequest request, Guid workspaceId);
    Task<ContactDto?> UpdateAsync(Guid id, UpdateContactRequest request, Guid workspaceId);
    Task<bool> DeleteAsync(Guid id, Guid workspaceId);
}

public interface IBudgetService
{
    Task<List<BudgetDto>> GetAllAsync(Guid workspaceId);
    Task<BudgetDto?> GetByIdAsync(Guid id, Guid workspaceId);
    Task<BudgetDto> CreateAsync(CreateBudgetRequest request, Guid workspaceId);
    Task<BudgetDto?> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid workspaceId);
    Task<bool> DeleteAsync(Guid id, Guid workspaceId);
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(Guid workspaceId);
}

public interface ICurrencyService
{
    Task<List<CurrencyDto>> GetAllAsync();
}
