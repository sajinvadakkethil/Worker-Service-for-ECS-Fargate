using Microsoft.Extensions.Logging;
using Poc.MediumWorker.Infrastructure;

namespace Poc.MediumWorker.Services;

public interface IJobRunner
{
	Task RunAsync(CancellationToken ct);
}

public sealed class SampleJobRunner : IJobRunner
{
	private readonly ILogger<SampleJobRunner> _logger;
	private readonly IClock _clock;

	public SampleJobRunner(ILogger<SampleJobRunner> logger, IClock clock)
	{
		_logger = logger;
		_clock = clock;
	}

	public async Task RunAsync(CancellationToken ct)
	{
		_logger.LogInformation("Job started at {Time}", _clock.Now);

		// Simulate real work
		await Task.Delay(TimeSpan.FromSeconds(2), ct);

		_logger.LogInformation("Job finished at {Time}", _clock.Now);
	}
}
