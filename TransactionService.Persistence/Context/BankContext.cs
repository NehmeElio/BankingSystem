using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Persistence.Context;

public partial class BankContext : DbContext
{
    
    private readonly ITenantService _tenantService;
    private readonly IModifyConnectionService _modifyConnectionService;
    private readonly ILogger<BankContext> _logger;
    public BankContext(ITenantService tenantService, IModifyConnectionService modifyConnectionService, ILogger<BankContext> logger)
    {
        _tenantService = tenantService;
        _modifyConnectionService = modifyConnectionService;
        _logger = logger;
    }

    public BankContext(DbContextOptions<BankContext> options, ITenantService tenantService, IModifyConnectionService modifyConnectionService, ILogger<BankContext> logger)
        : base(options)
    {
        _tenantService = tenantService;
        _modifyConnectionService = modifyConnectionService;
        _logger = logger;
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<IntervalType> IntervalTypes { get; set; }

    public virtual DbSet<RecurrentTransaction> RecurrentTransactions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenant = _tenantService.GetTenant();
        var connectionString = _modifyConnectionService.ModifyConnectionString(tenant?.User, tenant?.Branch);
        optionsBuilder.UseNpgsql(connectionString);
        
        _logger.LogInformation("Database access with user {user} and branch {branch}", tenant?.User, tenant?.Branch);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("account_pk");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "account_email_unique").IsUnique();

            entity.HasIndex(e => e.Username, "account_username_unique").IsUnique();

            entity.Property(e => e.AccountId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Balance).HasPrecision(10, 2);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("account_customer_customerid_fk");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("branch_pk");

            entity.ToTable("Branch","public");

            entity.Property(e => e.BranchId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BranchName).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customer_pk");

            entity.ToTable("Customer","public");

            entity.Property(e => e.CustomerId).UseIdentityAlwaysColumn();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<IntervalType>(entity =>
        {
            entity.HasKey(e => e.IntervalTypeId).HasName("intervaltype_pk");

            entity.ToTable("IntervalType","public");

            entity.Property(e => e.IntervalTypeId).UseIdentityAlwaysColumn();
            entity.Property(e => e.IntervalTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<RecurrentTransaction>(entity =>
        {
            entity.HasKey(e => e.RecurrentTransactionId).HasName("recurrenttransaction_pk");

            entity.ToTable("RecurrentTransaction");

            entity.Property(e => e.RecurrentTransactionId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Balance).HasPrecision(10, 2);
            entity.Property(e => e.Version).HasDefaultValue(0);

            entity.HasOne(d => d.Account).WithMany(p => p.RecurrentTransactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("recurrenttransaction_customer_customerid_fk");

            entity.HasOne(d => d.IntervalType).WithMany(p => p.RecurrentTransactions)
                .HasForeignKey(d => d.IntervalTypeId)
                .HasConstraintName("recurrenttransaction_intervaltype_intervaltypeid_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pk");

            entity.ToTable("Role","public");

            entity.Property(e => e.RoleId).UseIdentityAlwaysColumn();
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transaction_pk");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("transaction_customer_customerid_fk");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .HasConstraintName("transaction_transactiontype_transactiontypeid_fk");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.TransactionTypeId).HasName("transactiontype_pk");

            entity.ToTable("TransactionType","public");

            entity.Property(e => e.TransactionTypeId).UseIdentityAlwaysColumn();
            entity.Property(e => e.TransactionTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pk");

            entity.ToTable("User","public");

            entity.Property(e => e.UserId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("user_branch_branchid_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_role_roleid_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
