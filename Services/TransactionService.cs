using System.Text.Json;
using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<TransactionDto>> GetAllAsync(Guid workspaceId, TransactionFilterRequest filter)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.WorkspaceId == workspaceId)
            .AsQueryable();

        // Apply filters
        if (filter.AccountId.HasValue)
            query = query.Where(t => t.AccountId == filter.AccountId.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.ContactId.HasValue)
            query = query.Where(t => t.ContactId == filter.ContactId.Value);

        if (!string.IsNullOrEmpty(filter.Flow))
            query = query.Where(t => t.Flow == Enum.Parse<FlowType>(filter.Flow, true));

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(t => t.Status == Enum.Parse<TxnStatus>(filter.Status, true));

        if (filter.DateFrom.HasValue)
            query = query.Where(t => t.OccurredOn >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(t => t.OccurredOn <= filter.DateTo.Value);

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t => t.Description != null && t.Description.ToLower().Contains(search));
        }

        if (filter.Tags != null && filter.Tags.Any())
        {
            query = query.Where(t => t.TransactionTags.Any(tt => filter.Tags.Contains(tt.TagId)));
        }

        var total = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(t => t.OccurredOn)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var dtos = transactions.Select(MapToDto).ToList();

        return new PaginatedResponse<TransactionDto>(
            dtos,
            total,
            filter.Page,
            filter.PageSize,
            (int)Math.Ceiling(total / (double)filter.PageSize)
        );
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, Guid workspaceId)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.WorkspaceId == workspaceId);

        return transaction == null ? null : MapToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionRequest request, Guid workspaceId, Guid? userId)
    {
        // Validate account belongs to workspace
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.WorkspaceId == workspaceId);

        if (account == null)
            throw new InvalidOperationException("La cuenta no existe o no pertenece al workspace");

        // Validate currency matches account
        if (request.CurrencyCode != account.CurrencyCode)
            throw new InvalidOperationException("La moneda de la transacción no coincide con la de la cuenta");

        // Validate category flow matches transaction flow
        if (request.CategoryId.HasValue)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId.Value && c.WorkspaceId == workspaceId);

            if (category == null)
                throw new InvalidOperationException("La categoría no existe o no pertenece al workspace");

            var txnFlow = Enum.Parse<FlowType>(request.Flow, true);
            if (category.Flow != txnFlow)
                throw new InvalidOperationException("La categoría no coincide con el tipo de transacción");
        }

        var transaction = new Transaction
        {
            WorkspaceId = workspaceId,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            ContactId = request.ContactId,
            Flow = Enum.Parse<FlowType>(request.Flow, true),
            Status = string.IsNullOrEmpty(request.Status) ? TxnStatus.Cleared : Enum.Parse<TxnStatus>(request.Status, true),
            OccurredOn = request.OccurredOn,
            DueOn = request.DueOn,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            FxRate = request.FxRate,
            AmountBase = request.Amount * request.FxRate,
            Description = request.Description,
            Source = string.IsNullOrEmpty(request.Source) ? TxnSource.Manual : Enum.Parse<TxnSource>(request.Source, true),
            ExternalId = request.ExternalId,
            Metadata = request.Metadata != null ? JsonSerializer.Serialize(request.Metadata) : "{}",
            CreatedBy = userId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return MapToDto(transaction);
    }

    public async Task<TransactionDto?> UpdateAsync(Guid id, UpdateTransactionRequest request, Guid workspaceId)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.WorkspaceId == workspaceId);

        if (transaction == null) return null;

        if (request.AccountId.HasValue) transaction.AccountId = request.AccountId.Value;
        if (request.CategoryId.HasValue) transaction.CategoryId = request.CategoryId;
        if (request.ContactId.HasValue) transaction.ContactId = request.ContactId;
        if (request.Flow != null) transaction.Flow = Enum.Parse<FlowType>(request.Flow, true);
        if (request.Status != null) transaction.Status = Enum.Parse<TxnStatus>(request.Status, true);
        if (request.OccurredOn.HasValue) transaction.OccurredOn = request.OccurredOn.Value;
        if (request.DueOn.HasValue) transaction.DueOn = request.DueOn;
        if (request.Amount.HasValue) transaction.Amount = request.Amount.Value;
        if (request.CurrencyCode != null) transaction.CurrencyCode = request.CurrencyCode;
        if (request.FxRate.HasValue) transaction.FxRate = request.FxRate.Value;
        if (request.Description != null) transaction.Description = request.Description;
        if (request.Source != null) transaction.Source = Enum.Parse<TxnSource>(request.Source, true);
        if (request.Metadata != null) transaction.Metadata = JsonSerializer.Serialize(request.Metadata);

        // Recalculate amount_base
        transaction.AmountBase = transaction.Amount * transaction.FxRate;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid workspaceId)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.WorkspaceId == workspaceId);

        if (transaction == null) return false;

        // Soft delete
        transaction.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static TransactionDto MapToDto(Transaction t) => new(
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
    );
}
