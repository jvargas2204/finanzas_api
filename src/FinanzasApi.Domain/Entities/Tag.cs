using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.src.FinanzasApi.Domain.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}
