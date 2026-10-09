using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _context;

    public AccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountDto>> GetAllAsync(Guid workspaceId)
    {
        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.WorkspaceId == workspaceId)
            .ToListAsync();

        var result = new List<AccountDto>();
        foreach (var account in accounts)
        {
            var balance = await CalculateBalanceAsync(account.Id);
            result.Add(MapToDto(account, balance));
        }

        return result;
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, Guid workspaceId)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.WorkspaceId == workspaceId);

        if (account == null) return null;

        var balance = await CalculateBalanceAsync(account.Id);
        return MapToDto(account, balance);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, Guid workspaceId)
    {
        var account = new Account
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Type = Enum.Parse<AccountType>(request.Type, true),
            CurrencyCode = request.CurrencyCode,
            OpeningBalance = request.OpeningBalance
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return MapToDto(account, request.OpeningBalance);
    }

    public async Task<AccountDto?> UpdateAsync(Guid id, UpdateAccountRequest request, Guid workspaceId)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && a.WorkspaceId == workspaceId);

        if (account == null) return null;

        if (request.Name != null) account.Name = request.Name;
        if (request.Type != null) account.Type = Enum.Parse<AccountType>(request.Type, true);
        if (request.CurrencyCode != null) account.CurrencyCode = request.CurrencyCode;
        if (request.OpeningBalance.HasValue) account.OpeningBalance = request.OpeningBalance.Value;
        if (request.IsActive.HasValue) account.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();

        var balance = await CalculateBalanceAsync(account.Id);
        return MapToDto(account, balance);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid workspaceId)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && a.WorkspaceId == workspaceId);

        if (account == null) return false;

        account.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<decimal> CalculateBalanceAsync(Guid accountId)
    {
        var account = await _context.Accounts.AsNoTracking().FirstAsync(a => a.Id == accountId);

        var income = await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Flow == FlowType.Income && t.Status == TxnStatus.Cleared)
            .SumAsync(t => t.Amount);

        var expense = await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Flow == FlowType.Expense && t.Status == TxnStatus.Cleared)
            .SumAsync(t => t.Amount);

        return account.OpeningBalance + income - expense;
    }

    private static AccountDto MapToDto(Account a, decimal? balance) => new(
        a.Id,
        a.WorkspaceId,
        a.Name,
        a.Type.ToString(),
        a.CurrencyCode,
        a.OpeningBalance,
        a.IsActive,
        a.CreatedAt,
        balance
    );
}
