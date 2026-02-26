using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Poc.MediumWorker.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false)
			.AddJsonFile("appsettings.Development.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var connStr = configuration.GetConnectionString("Postgres")
					  ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");

		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(connStr)
			.Options;

		return new AppDbContext(options);
	}
}
