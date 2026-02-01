using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Poc.MediumWorker.Configuration;
using Poc.MediumWorker.Services;

namespace Poc.MediumWorker.HostedServices;

public sealed class WorkerHostedService : BackgroundService
{
	private readonly ILogger<WorkerHostedService> _logger;
	private readonly WorkerOptions _options;
	private readonly IHeartbeatService _heartbeat;
	private readonly IJobRunner _jobRunner;

	public WorkerHostedService(
		ILogger<WorkerHostedService> logger,
		WorkerOptions options,
		IHeartbeatService heartbeat,
		IJobRunner jobRunner)
	{
		_logger = logger;
		_options = options;
		_heartbeat = heartbeat;
		_jobRunner = jobRunner;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Service starting. Instance={Instance}", _options.InstanceName);

		var heartbeatPeriod = TimeSpan.FromSeconds(_options.HeartbeatSeconds);
		var jobPeriod = TimeSpan.FromSeconds(_options.JobIntervalSeconds);

		var nextJob = DateTimeOffset.UtcNow;

		while (!stoppingToken.IsCancellationRequested)
		{
			await _heartbeat.BeatAsync(stoppingToken);

			if (DateTimeOffset.UtcNow >= nextJob)
			{
				try
				{
					await _jobRunner.RunAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Job failed.");
				}

				nextJob = DateTimeOffset.UtcNow.Add(jobPeriod);
			}

			await Task.Delay(heartbeatPeriod, stoppingToken);
		}

		_logger.LogInformation("Service stopping.");
	}
}
