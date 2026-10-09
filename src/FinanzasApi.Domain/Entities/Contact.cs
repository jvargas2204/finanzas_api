using FinanzasApi.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.src.FinanzasApi.Domain.Entities
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ContactKind Kind { get; set; }

        public string? Email { get; set; }
        public string? TaxId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
