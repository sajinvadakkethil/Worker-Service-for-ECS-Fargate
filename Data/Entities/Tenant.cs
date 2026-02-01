namespace Poc.MediumWorker.Data.Entities;

public sealed class Tenant
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string TenantKey { get; set; } = default!;
	public string Name { get; set; } = default!;
	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
