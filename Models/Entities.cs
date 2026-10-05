using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanzasApi.Models;

// Enums
public enum MemberRole { Owner, Admin, Editor, Viewer }
public enum AccountType { Bank, Cash, CreditCard, DigitalWallet }
public enum FlowType { Income, Expense }
public enum ContactKind { Client, Supplier, Both }
public enum TxnStatus { Pending, Cleared }
public enum TxnSource { Manual, CsvImport, BankApi }
public enum BudgetPeriod { Weekly, Monthly, Quarterly, Yearly }

// Currency
public class Currency
{
    [Key]
    [MaxLength(3)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public short MinorUnits { get; set; } = 2;
}

// User
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Email { get; set; } = string.Empty;

    public string? PasswordHash { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Workspace
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

// WorkspaceMember
public class WorkspaceMember
{
    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public MemberRole Role { get; set; } = MemberRole.Owner;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

// Account
public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public AccountType Type { get; set; }

    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Column(TypeName = "numeric(19,4)")]
    public decimal OpeningBalance { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

// Category
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

// Contact
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

// Tag
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

// Transaction
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

// TransactionTag
public class TransactionTag
{
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

// Budget
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

// BudgetAlert
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
