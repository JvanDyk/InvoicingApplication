namespace AndreyevInterview.Shared.Context;

public class AIDbContext : DbContext
{
    public AIDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<InvoiceHistoryEntity> InvoicesHistory { get; set; }
    public DbSet<LineItemEntity> LineItemEntitys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Client
        modelBuilder.Entity<ClientEntity>().Property(b => b.Name).IsRequired();
        modelBuilder.Entity<ClientEntity>().Property(b => b.Email).IsRequired();

        // Invoice
        modelBuilder.Entity<InvoiceEntity>().Property(b => b.Description).IsRequired();
        modelBuilder.Entity<InvoiceEntity>().Property(b => b.ClientId).IsRequired();

        // Line item
        modelBuilder.Entity<LineItemEntity>().Property(b => b.InvoiceId).IsRequired();
        modelBuilder.Entity<LineItemEntity>().Property(b => b.Quantity).IsRequired();
        modelBuilder.Entity<LineItemEntity>().Property(b => b.Cost).IsRequired();
        modelBuilder.Entity<LineItemEntity>().Property(b => b.isBillable).IsRequired();
    }
}