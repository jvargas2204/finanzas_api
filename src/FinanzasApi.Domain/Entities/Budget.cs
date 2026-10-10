using FinanzasApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanzasApi.Domain.Enums;

namespace FinanzasApi.Domain.Entities
{
    public class Budget
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "numeric(19,4)")]
        public decimal Amount { get; set; }

        public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;

        [Required]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public short AlertThreshold_pct { get; set; } = 80;

        public bool IncludeSubcategories { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public ICollection<BudgetAlert> Alerts { get; set; } = new List<BudgetAlert>();
    }
}
