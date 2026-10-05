using FinanzasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionTag> TransactionTags => Set<TransactionTag>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetAlert> BudgetAlerts => Set<BudgetAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar nombres de tabla y columna en snake_case para que coincidan con el esquema original
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Tabla en snake_case
            entityType.SetTableName(entityType.GetTableName()!.ToSnakeCase());

            // Columnas en snake_case
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()!.ToSnakeCase());
            }
        }

        // Currency
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(3);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.MinorUnits).HasColumnName("minor_units").HasDefaultValue(2);
        });

        // Configurar nombres de columna para todas las propiedades restantes
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var currentColumnName = property.GetColumnName();
                if (currentColumnName != null && currentColumnName == property.Name)
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }
            }
        }

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        // Workspace
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.BaseCurrency).HasColumnName("base_currency").HasMaxLength(3);
            entity.Property(e => e.FiscalYearStartMonth).HasColumnName("fiscal_year_start_month").HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        // WorkspaceMember
        modelBuilder.Entity<WorkspaceMember>(entity =>
        {
            entity.HasKey(e => new { e.WorkspaceId, e.UserId });
            entity.Property(e => e.Role).HasConversion<string>().HasColumnName("role");
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("now()");
        });

        // Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasConversion<string>();
            entity.Property(e => e.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
            entity.Property(e => e.OpeningBalance).HasColumnName("opening_balance").HasPrecision(19, 4);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.HasIndex(e => new { e.WorkspaceId, e.Name }).IsUnique();
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Flow).HasColumnName("flow").HasConversion<string>();
            entity.Property(e => e.IsTaxDeductible).HasColumnName("is_tax_deductible").HasDefaultValue(false);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.HasIndex(e => new { e.WorkspaceId, e.ParentId, e.Name }).IsUnique();
        });

        // Contact
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Kind).HasColumnName("kind").HasConversion<string>();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired(false);
            entity.Property(e => e.TaxId).HasColumnName("tax_id").IsRequired(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.HasIndex(e => new { e.WorkspaceId, e.Name }).IsUnique();
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Flow).HasColumnName("flow").HasConversion<string>();
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(TxnStatus.Cleared);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(19, 4);
            entity.Property(e => e.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
            entity.Property(e => e.FxRate).HasColumnName("fx_rate").HasPrecision(18, 8).HasDefaultValue(1);
            entity.Property(e => e.AmountBase).HasColumnName("amount_base").HasPrecision(19, 4);
            entity.Property(e => e.Source).HasColumnName("source").HasConversion<string>().HasDefaultValue(TxnSource.Manual);
            entity.Property(e => e.Metadata).HasColumnName("metadata").HasDefaultValue("{}");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
            entity.HasIndex(e => new { e.WorkspaceId, e.OccurredOn });
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // TransactionTag
        modelBuilder.Entity<TransactionTag>(entity =>
        {
            entity.HasKey(e => new { e.TransactionId, e.TagId });
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
        });

        // Budget
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(19, 4);
            entity.Property(e => e.Period).HasColumnName("period").HasConversion<string>().HasDefaultValue(BudgetPeriod.Monthly);
            entity.Property(e => e.AlertThreshold_pct).HasColumnName("alert_threshold_pct").HasDefaultValue(80);
            entity.Property(e => e.IncludeSubcategories).HasColumnName("include_subcategories").HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        });

        // BudgetAlert
        modelBuilder.Entity<BudgetAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TriggeredAt).HasColumnName("triggered_at").HasDefaultValueSql("now()");
            entity.HasIndex(e => new { e.BudgetId, e.PeriodStart, e.ThresholdPct }).IsUnique();
        });

        // Seed currencies
        modelBuilder.Entity<Currency>().HasData(
            new Currency { Code = "COP", Name = "Peso colombiano", MinorUnits = 2 },
            new Currency { Code = "USD", Name = "Dólar estadounidense", MinorUnits = 2 },
            new Currency { Code = "EUR", Name = "Euro", MinorUnits = 2 },
            new Currency { Code = "MXN", Name = "Peso mexicano", MinorUnits = 2 },
            new Currency { Code = "ARS", Name = "Peso argentino", MinorUnits = 2 },
            new Currency { Code = "CLP", Name = "Peso chileno", MinorUnits = 0 },
            new Currency { Code = "PEN", Name = "Sol peruano", MinorUnits = 2 },
            new Currency { Code = "BRL", Name = "Real brasileño", MinorUnits = 2 }
        );
    }
}
