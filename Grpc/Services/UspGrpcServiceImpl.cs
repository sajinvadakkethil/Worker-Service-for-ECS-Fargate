using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Poc.MediumWorker.Contracts;
using Poc.MediumWorker.Data;
using Poc.MediumWorker.Data.Entities;

namespace Poc.MediumWorker.Grpc.Services;

public sealed class UspGrpcServiceImpl : UspService.UspServiceBase
{
	private readonly AppDbContext _db;

	public UspGrpcServiceImpl(AppDbContext db) => _db = db;

	public override async Task<CreateTenantReply> CreateTenant(CreateTenantRequest request, ServerCallContext context)
	{
		var tenantKey = request.TenantKey?.Trim();
		var name = request.Name?.Trim();

		if (string.IsNullOrWhiteSpace(tenantKey) || string.IsNullOrWhiteSpace(name))
			throw new RpcException(new Status(StatusCode.InvalidArgument, "tenantKey and name are required."));

		var exists = await _db.Tenants.AnyAsync(t => t.TenantKey == tenantKey, context.CancellationToken);
		if (exists)
			throw new RpcException(new Status(StatusCode.AlreadyExists, $"Tenant '{tenantKey}' already exists."));

		var entity = new Tenant { TenantKey = tenantKey, Name = name };
		_db.Tenants.Add(entity);
		await _db.SaveChangesAsync(context.CancellationToken);

		return new CreateTenantReply
		{
			Id = entity.Id.ToString(),
			TenantKey = entity.TenantKey,
			Name = entity.Name
		};
	}

	public override async Task<GetTenantReply> GetTenant(GetTenantRequest request, ServerCallContext context)
	{
		var tenantKey = request.TenantKey?.Trim();
		if (string.IsNullOrWhiteSpace(tenantKey))
			throw new RpcException(new Status(StatusCode.InvalidArgument, "tenantKey is required."));

		var tenant = await _db.Tenants.SingleOrDefaultAsync(t => t.TenantKey == tenantKey, context.CancellationToken);
		if (tenant is null)
			throw new RpcException(new Status(StatusCode.NotFound, $"Tenant '{tenantKey}' not found."));

		return new GetTenantReply
		{
			Id = tenant.Id.ToString(),
			TenantKey = tenant.TenantKey,
			Name = tenant.Name,
			CreatedAt = tenant.CreatedAt.ToString("O")
		};
	}
}
