using FinanzasApi.Data;
using FinanzasApi.Domain.Entities;
using FinanzasApi.Domain.Enums;
using FinanzasApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Application.Services;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;

    public ContactService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ContactDto>> GetAllAsync(Guid workspaceId)
    {
        var contacts = await _context.Contacts
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId)
            .ToListAsync();

        return contacts.Select(MapToDto).ToList();
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id, Guid workspaceId)
    {
        var contact = await _context.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        return contact == null ? null : MapToDto(contact);
    }

    public async Task<ContactDto> CreateAsync(CreateContactRequest request, Guid workspaceId)
    {
        var contact = new Contact
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Kind = Enum.Parse<ContactKind>(request.Kind, true),
            Email = request.Email,
            TaxId = request.TaxId
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        return MapToDto(contact);
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactRequest request, Guid workspaceId)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        if (contact == null) return null;

        if (request.Name != null) contact.Name = request.Name;
        if (request.Kind != null) contact.Kind = Enum.Parse<ContactKind>(request.Kind, true);
        if (request.Email != null) contact.Email = request.Email;
        if (request.TaxId != null) contact.TaxId = request.TaxId;

        await _context.SaveChangesAsync();
        return MapToDto(contact);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid workspaceId)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId);

        if (contact == null) return false;

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ContactDto MapToDto(Contact c) => new(
        c.Id,
        c.WorkspaceId,
        c.Name,
        c.Kind.ToString(),
        c.Email,
        c.TaxId,
        c.CreatedAt
    );
}
