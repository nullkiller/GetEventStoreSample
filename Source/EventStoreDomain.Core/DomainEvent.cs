using System;

namespace EventStore.Domain.Core
{
	/// <summary>
	/// Denotes an event in the domain model.
	/// </summary>
	public interface DomainEvent
	{
		/// <summary>
		/// Gets the aggregate root id of the domain event.
		/// </summary>
		Guid AggregateId { get; }
    }
}