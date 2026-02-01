using Microsoft.Extensions.Logging;
using Poc.MediumWorker.Configuration;
using Poc.MediumWorker.Infrastructure;

namespace Poc.MediumWorker.Services;

public interface IHeartbeatService
{
	Task BeatAsync(CancellationToken ct);
}

public sealed class HeartbeatService : IHeartbeatService
{
	private readonly ILogger<HeartbeatService> _logger;
	private readonly IClock _clock;
	private readonly WorkerOptions _options;

	public HeartbeatService(ILogger<HeartbeatService> logger, IClock clock, WorkerOptions options)
	{
		_logger = logger;
		_clock = clock;
		_options = options;
	}

	public Task BeatAsync(CancellationToken ct)
	{
		_logger.LogInformation("[{Instance}] Heartbeat at {Time}", _options.InstanceName, _clock.Now);
		return Task.CompletedTask;
	}
}
