using FinanzasApi.Models.DTOs;
using FinanzasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrenciesController> _logger;

    public CurrenciesController(ICurrencyService currencyService, ILogger<CurrenciesController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CurrencyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CurrencyDto>>> GetAll()
    {
        try
        {
            var currencies = await _currencyService.GetAllAsync();
            return Ok(currencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las monedas");
            return StatusCode(500, new { message = "Error al obtener las monedas" });
        }
    }
}
