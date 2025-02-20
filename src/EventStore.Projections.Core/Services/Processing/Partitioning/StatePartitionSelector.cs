// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using EventStore.Projections.Core.Messages;

namespace EventStore.Projections.Core.Services.Processing.Partitioning {
	public abstract class StatePartitionSelector {
		public abstract string GetStatePartition(EventReaderSubscriptionMessage.CommittedEventReceived @event);
		public abstract bool EventReaderBasePartitionDeletedIsSupported();
	}
}
