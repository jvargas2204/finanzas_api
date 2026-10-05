using FinanzasApi.Data;
using FinanzasApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

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
