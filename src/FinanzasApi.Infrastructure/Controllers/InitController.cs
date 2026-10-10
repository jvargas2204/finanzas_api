using FinanzasApi.Data;
using FinanzasApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FinanzasApi.Domain.Enums;

namespace FinanzasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InitController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InitController> _logger;

    public InitController(ApplicationDbContext context, ILogger<InitController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Inicializa la base de datos con datos de ejemplo
    /// </summary>
    [HttpPost("seed")]
    public async Task<ActionResult> SeedData()
    {
        try
        {
            // Verificar si ya existe data
            if (await _context.Workspaces.AnyAsync())
            {
                return Ok(new { message = "La base de datos ya tiene datos", workspaceId = await _context.Workspaces.Select(w => w.Id).FirstAsync() });
            }

            // Crear usuario por defecto
            var user = new User
            {
                Email = "demo@finanzaspro.com",
                FullName = "Usuario Demo"
            };
            _context.Users.Add(user);

            // Crear workspace por defecto
            var workspace = new Workspace
            {
                Name = "Mi Empresa",
                BaseCurrency = "COP"
            };
            _context.Workspaces.Add(workspace);

            // Crear membresía
            var member = new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = user.Id,
                Role = MemberRole.Owner
            };
            _context.WorkspaceMembers.Add(member);

            await _context.SaveChangesAsync();

            // Crear cuentas de ejemplo
            var accounts = new List<Account>
            {
                new() { WorkspaceId = workspace.Id, Name = "Bancolombia - Ahorros", Type = AccountType.Bank, CurrencyCode = "COP", OpeningBalance = 5000000 },
                new() { WorkspaceId = workspace.Id, Name = "Efectivo", Type = AccountType.Cash, CurrencyCode = "COP", OpeningBalance = 500000 },
                new() { WorkspaceId = workspace.Id, Name = "Tarjeta de Crédito", Type = AccountType.CreditCard, CurrencyCode = "COP", OpeningBalance = 0 }
            };
            _context.Accounts.AddRange(accounts);

            // Crear categorías de ejemplo
            var categories = new List<Category>
            {
                new() { WorkspaceId = workspace.Id, Name = "Transporte", Flow = FlowType.Expense, IsTaxDeductible = true },
                new() { WorkspaceId = workspace.Id, Name = "Software y herramientas", Flow = FlowType.Expense, IsTaxDeductible = true },
                new() { WorkspaceId = workspace.Id, Name = "Marketing y publicidad", Flow = FlowType.Expense, IsTaxDeductible = true },
                new() { WorkspaceId = workspace.Id, Name = "Alimentación", Flow = FlowType.Expense, IsTaxDeductible = false },
                new() { WorkspaceId = workspace.Id, Name = "Servicios públicos", Flow = FlowType.Expense, IsTaxDeductible = true },
                new() { WorkspaceId = workspace.Id, Name = "Servicios profesionales", Flow = FlowType.Income, IsTaxDeductible = false },
                new() { WorkspaceId = workspace.Id, Name = "Ventas de productos", Flow = FlowType.Income, IsTaxDeductible = false },
                new() { WorkspaceId = workspace.Id, Name = "Otros ingresos", Flow = FlowType.Income, IsTaxDeductible = false }
            };
            _context.Categories.AddRange(categories);

            // Crear contactos de ejemplo
            var contacts = new List<Contact>
            {
                new() { WorkspaceId = workspace.Id, Name = "Empresa ABC SAS", Kind = ContactKind.Client, Email = "contacto@empresaabc.com" },
                new() { WorkspaceId = workspace.Id, Name = "Proveedor XYZ", Kind = ContactKind.Supplier, Email = "ventas@proveedorxyz.com" }
            };
            _context.Contacts.AddRange(contacts);

            // Crear transacciones de ejemplo
            var transactions = new List<Transaction>
            {
                new() { WorkspaceId = workspace.Id, AccountId = accounts[0].Id, CategoryId = categories[5].Id, Flow = FlowType.Income, Status = TxnStatus.Cleared, OccurredOn = new DateOnly(2026, 9, 25), Amount = 3500000, CurrencyCode = "COP", FxRate = 1, AmountBase = 3500000, Description = "Pago proyecto consultoría", Source = TxnSource.Manual },
                new() { WorkspaceId = workspace.Id, AccountId = accounts[0].Id, CategoryId = categories[1].Id, Flow = FlowType.Expense, Status = TxnStatus.Cleared, OccurredOn = new DateOnly(2026, 9, 24), Amount = 85000, CurrencyCode = "COP", FxRate = 1, AmountBase = 85000, Description = "Suscripción Figma", Source = TxnSource.Manual },
                new() { WorkspaceId = workspace.Id, AccountId = accounts[1].Id, CategoryId = categories[3].Id, Flow = FlowType.Expense, Status = TxnStatus.Cleared, OccurredOn = new DateOnly(2026, 9, 23), Amount = 120000, CurrencyCode = "COP", FxRate = 1, AmountBase = 120000, Description = "Almuerzo cliente", Source = TxnSource.Manual },
                new() { WorkspaceId = workspace.Id, AccountId = accounts[0].Id, CategoryId = categories[6].Id, Flow = FlowType.Income, Status = TxnStatus.Pending, OccurredOn = new DateOnly(2026, 9, 28), Amount = 2800000, CurrencyCode = "COP", FxRate = 1, AmountBase = 2800000, Description = "Factura #2026-045", Source = TxnSource.Manual },
                new() { WorkspaceId = workspace.Id, AccountId = accounts[0].Id, CategoryId = categories[2].Id, Flow = FlowType.Expense, Status = TxnStatus.Cleared, OccurredOn = new DateOnly(2026, 9, 22), Amount = 450000, CurrencyCode = "COP", FxRate = 1, AmountBase = 450000, Description = "Campaña Google Ads", Source = TxnSource.Manual }
            };
            _context.Transactions.AddRange(transactions);

            // Crear presupuestos de ejemplo
            var budgets = new List<Budget>
            {
                new() { WorkspaceId = workspace.Id, CategoryId = categories[1].Id, Name = "Software mensual", Amount = 500000, Period = BudgetPeriod.Monthly, StartDate = new DateOnly(2026, 9, 1) },
                new() { WorkspaceId = workspace.Id, CategoryId = categories[2].Id, Name = "Marketing trimestral", Amount = 2000000, Period = BudgetPeriod.Quarterly, StartDate = new DateOnly(2026, 7, 1) }
            };
            _context.Budgets.AddRange(budgets);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Datos de ejemplo creados exitosamente", workspaceId = workspace.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar datos");
            return StatusCode(500, new { message = "Error al inicializar datos", error = ex.Message });
        }
    }
}
