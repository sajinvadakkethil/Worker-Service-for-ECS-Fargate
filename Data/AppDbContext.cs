using Microsoft.EntityFrameworkCore;
using Poc.MediumWorker.Data.Entities;

namespace Poc.MediumWorker.Data;

public sealed class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Tenant> Tenants => Set<Tenant>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Tenant>(e =>
		{
			e.HasKey(x => x.Id);
			e.HasIndex(x => x.TenantKey).IsUnique();
			e.Property(x => x.TenantKey).HasMaxLength(64).IsRequired();
			e.Property(x => x.Name).HasMaxLength(200).IsRequired();
		});
	}
}
