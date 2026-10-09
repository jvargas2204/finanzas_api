using FinanzasApi.Data;
using FinanzasApi.Models;
using FinanzasApi.src.FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly ApplicationDbContext _context;

    public WorkspaceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkspaceDto?> GetByIdAsync(Guid id)
    {
        var workspace = await _context.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

        return workspace == null ? null : MapToDto(workspace);
    }

    public async Task<WorkspaceDto> CreateAsync(CreateWorkspaceRequest request, Guid userId)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            BaseCurrency = request.BaseCurrency,
            FiscalYearStartMonth = request.FiscalYearStartMonth
        };

        _context.Workspaces.Add(workspace);

        // Add creator as owner
        var member = new WorkspaceMember
        {
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = MemberRole.Owner
        };
        _context.WorkspaceMembers.Add(member);

        await _context.SaveChangesAsync();

        // Seed default categories
        await SeedDefaultCategories(workspace.Id);

        return MapToDto(workspace);
    }

    public async Task<WorkspaceDto?> UpdateAsync(Guid id, UpdateWorkspaceRequest request)
    {
        var workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null) return null;

        if (request.Name != null) workspace.Name = request.Name;
        if (request.BaseCurrency != null) workspace.BaseCurrency = request.BaseCurrency;
        if (request.FiscalYearStartMonth.HasValue) workspace.FiscalYearStartMonth = request.FiscalYearStartMonth.Value;

        await _context.SaveChangesAsync();
        return MapToDto(workspace);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null) return false;

        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WorkspaceDto>> GetByUserAsync(Guid userId)
    {
        var workspaces = await _context.Workspaces
            .AsNoTracking()
            .Where(w => w.Members.Any(m => m.UserId == userId))
            .ToListAsync();

        return workspaces.Select(MapToDto).ToList();
    }

    private async Task SeedDefaultCategories(Guid workspaceId)
    {
        var categories = new List<Category>
        {
            new() { WorkspaceId = workspaceId, Name = "Transporte", Flow = FlowType.Expense, IsTaxDeductible = true },
            new() { WorkspaceId = workspaceId, Name = "Software y herramientas", Flow = FlowType.Expense, IsTaxDeductible = true },
            new() { WorkspaceId = workspaceId, Name = "Marketing y publicidad", Flow = FlowType.Expense, IsTaxDeductible = true },
            new() { WorkspaceId = workspaceId, Name = "Alimentación", Flow = FlowType.Expense, IsTaxDeductible = false },
            new() { WorkspaceId = workspaceId, Name = "Servicios públicos", Flow = FlowType.Expense, IsTaxDeductible = true },
            new() { WorkspaceId = workspaceId, Name = "Impuestos y contabilidad", Flow = FlowType.Expense, IsTaxDeductible = true },
            new() { WorkspaceId = workspaceId, Name = "Servicios profesionales", Flow = FlowType.Income, IsTaxDeductible = false },
            new() { WorkspaceId = workspaceId, Name = "Ventas de productos", Flow = FlowType.Income, IsTaxDeductible = false },
            new() { WorkspaceId = workspaceId, Name = "Otros ingresos", Flow = FlowType.Income, IsTaxDeductible = false },
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();

        // Add subcategories
        var subcategories = new List<Category>();
        var transporte = categories.First(c => c.Name == "Transporte");
        var software = categories.First(c => c.Name == "Software y herramientas");
        var marketing = categories.First(c => c.Name == "Marketing y publicidad");
        var alimentacion = categories.First(c => c.Name == "Alimentación");
        var servicios = categories.First(c => c.Name == "Servicios públicos");
        var profesionales = categories.First(c => c.Name == "Servicios profesionales");

        subcategories.AddRange(new[]
        {
            new Category { WorkspaceId = workspaceId, ParentId = transporte.Id, Name = "Combustible", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = transporte.Id, Name = "Taxi y apps", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = transporte.Id, Name = "Peajes", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = software.Id, Name = "Suscripciones SaaS", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = software.Id, Name = "Hardware", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = marketing.Id, Name = "Anuncios en línea", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = marketing.Id, Name = "Diseño y contenido", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = alimentacion.Id, Name = "Restaurantes", Flow = FlowType.Expense, IsTaxDeductible = false },
            new Category { WorkspaceId = workspaceId, ParentId = alimentacion.Id, Name = "Supermercado", Flow = FlowType.Expense, IsTaxDeductible = false },
            new Category { WorkspaceId = workspaceId, ParentId = servicios.Id, Name = "Internet y telefonía", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = servicios.Id, Name = "Energía", Flow = FlowType.Expense, IsTaxDeductible = true },
            new Category { WorkspaceId = workspaceId, ParentId = profesionales.Id, Name = "Consultoría", Flow = FlowType.Income, IsTaxDeductible = false },
            new Category { WorkspaceId = workspaceId, ParentId = profesionales.Id, Name = "Proyectos por contrato", Flow = FlowType.Income, IsTaxDeductible = false },
        });

        _context.Categories.AddRange(subcategories);
        await _context.SaveChangesAsync();
    }

    private static WorkspaceDto MapToDto(Workspace w) => new(
        w.Id,
        w.Name,
        w.BaseCurrency,
        w.FiscalYearStartMonth,
        w.CreatedAt
    );
}
