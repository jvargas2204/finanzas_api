using FinanzasApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanzasApi.Domain.Enums;

namespace FinanzasApi.Domain.Entities
{

    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        [Required]
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid? ContactId { get; set; }
        public Contact? Contact { get; set; }

        [Required]
        public FlowType Flow { get; set; }

        public TxnStatus Status { get; set; } = TxnStatus.Cleared;

        [Required]
        public DateOnly OccurredOn { get; set; }

        public DateOnly? DueOn { get; set; }

        [Required]
        [Column(TypeName = "numeric(19,4)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,8)")]
        public decimal FxRate { get; set; } = 1;

        [Required]
        [Column(TypeName = "numeric(19,4)")]
        public decimal AmountBase { get; set; }

        public string? Description { get; set; }

        public Guid? TransferGroupId { get; set; }

        public TxnSource Source { get; set; } = TxnSource.Manual;

        public string? ExternalId { get; set; }

        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; } = "{}";

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}
