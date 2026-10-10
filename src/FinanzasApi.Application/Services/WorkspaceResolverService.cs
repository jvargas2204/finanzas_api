using FinanzasApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Application.Services;

public interface IWorkspaceResolverService
{
    Task<Guid> GetDefaultWorkspaceIdAsync();
}

public class WorkspaceResolverService : IWorkspaceResolverService
{
    private readonly ApplicationDbContext _context;

    public WorkspaceResolverService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> GetDefaultWorkspaceIdAsync()
    {
        var workspace = await _context.Workspaces
            .OrderBy(w => w.CreatedAt)
            .FirstOrDefaultAsync();

        if (workspace == null)
        {
            throw new InvalidOperationException("No existe ningún workspace en la base de datos. Ejecute el endpoint /api/init/seed primero.");
        }

        return workspace.Id;
    }
}
