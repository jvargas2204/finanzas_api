using FinanzasApi.Data;
using FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Application.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ApplicationDbContext _context;

    public CurrencyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurrencyDto>> GetAllAsync()
    {
        var currencies = await _context.Currencies
            .AsNoTracking()
            .ToListAsync();

        return currencies.Select(c => new CurrencyDto(c.Code, c.Name, c.MinorUnits)).ToList();
    }
}
