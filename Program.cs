using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Poc.MediumWorker.Configuration;
using Poc.MediumWorker.Data;
using Poc.MediumWorker.Grpc.Services;
using Poc.MediumWorker.HostedServices;
using Poc.MediumWorker.Infrastructure;
using Poc.MediumWorker.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Bind Worker options (keep your current pattern)
var workerOptions = new WorkerOptions();
builder.Configuration.GetSection("Worker").Bind(workerOptions);
builder.Services.AddSingleton(workerOptions);

// Optional Windows Service support
if (OperatingSystem.IsWindows())
{
	builder.Services.AddWindowsService(options =>
	{
		options.ServiceName = "PocMediumWorker";
	});
}

// EF Core + Postgres
var connStr = builder.Configuration.GetConnectionString("Postgres")
	?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

// Your existing DI
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IHeartbeatService, HeartbeatService>();
builder.Services.AddSingleton<IJobRunner, SampleJobRunner>();

builder.Services.AddHostedService<WorkerHostedService>();

// gRPC server
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
	// HTTP/1.1 for browser / ALB health checks
	options.ListenAnyIP(5000, listenOptions =>
	{
		listenOptions.Protocols = HttpProtocols.Http1;
	});

	options.ListenAnyIP(5001, listenOptions =>
	{
		listenOptions.Protocols = HttpProtocols.Http2;
	});
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	try
	{
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		db.Database.Migrate();
		app.Logger.LogInformation("Database migrated successfully.");
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "Database migration failed at startup.");
	}
}


// gRPC endpoints
app.MapGrpcService<UspGrpcServiceImpl>();
if (app.Environment.IsDevelopment())
{
	app.MapGrpcReflectionService();
}

// Optional health endpoint for ECS / ALB health checks
app.MapGet("/health", () => "OK");

app.Run();
