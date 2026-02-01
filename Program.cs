using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Poc.MediumWorker.Configuration;
using Poc.MediumWorker.Data;
using Poc.MediumWorker.Grpc.Services;
using Poc.MediumWorker.HostedServices;
using Poc.MediumWorker.Infrastructure;
using Poc.MediumWorker.Services;

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

builder.Logging.AddConsole();

var app = builder.Build();

// gRPC endpoints
app.MapGrpcService<UspGrpcServiceImpl>();

// Optional health endpoint for ECS / ALB health checks
app.MapGet("/health", () => "OK");

app.Run();
