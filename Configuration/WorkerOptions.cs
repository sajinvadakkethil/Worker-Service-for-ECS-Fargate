using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.MediumWorker.Configuration
{
	public class WorkerOptions
	{
		public int HeartbeatSeconds { get; set; } = 10;
		public int JobIntervalSeconds { get; set; } = 60;
		public string InstanceName { get; set; } = "PocWorker";
	}
}
