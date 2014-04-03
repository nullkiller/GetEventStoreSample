using System;

namespace EventStore.Commands
{
	public interface Command
	{
		Guid AggregateId { get; set; }
		int Version { get; set; }
	}
}