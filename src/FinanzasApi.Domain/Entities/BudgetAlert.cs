using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanzasApi.src.FinanzasApi.Domain.Entities
{
    public class BudgetAlert
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BudgetId { get; set; }
        public Budget Budget { get; set; } = null!;

        [Required]
        public DateOnly PeriodStart { get; set; }

        public short ThresholdPct { get; set; }

        [Required]
        [Column(TypeName = "numeric(19,4)")]
        public decimal SpentAmount { get; set; }

        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

        public DateTime? AcknowledgedAt { get; set; }
    }
}
