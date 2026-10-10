using FinanzasApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

using FinanzasApi.Domain.Enums;

namespace FinanzasApi.Domain.Entities
{
    public class Category
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public FlowType Flow { get; set; }

        public bool IsTaxDeductible { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
