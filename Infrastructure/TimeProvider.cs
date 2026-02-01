using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.MediumWorker.Infrastructure
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}

	public sealed class SystemClock : IClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
