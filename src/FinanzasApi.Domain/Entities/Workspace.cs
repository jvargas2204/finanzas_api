using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.src.FinanzasApi.Domain.Entities
{
    public class Workspace
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(3)]
        public string BaseCurrency { get; set; } = "COP";

        public short FiscalYearStartMonth { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
