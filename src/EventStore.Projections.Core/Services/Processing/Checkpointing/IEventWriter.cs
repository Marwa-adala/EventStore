// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using EventStore.Projections.Core.Services.Processing.Emitting.EmittedEvents;

namespace EventStore.Projections.Core.Services.Processing.Checkpointing {
	public interface IEventWriter {
		void ValidateOrderAndEmitEvents(EmittedEventEnvelope[] events);
	}
}
