// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Threading;
using System.Threading.Tasks;
using EventStore.Core.TransactionLog.Chunks;
using Serilog;

namespace EventStore.Core.TransactionLog.Scavenging {
	public class ChunkMerger : IChunkMerger {
		private readonly ILogger _logger;
		private readonly bool _mergeChunks;
		private readonly IChunkMergerBackend _backend;
		private readonly Throttle _throttle;

		public ChunkMerger(
			ILogger logger,
			bool mergeChunks,
			IChunkMergerBackend backend,
			Throttle throttle) {

			_logger = logger;
			_mergeChunks = mergeChunks;
			_backend = backend;
			_throttle = throttle;
		}

		public ValueTask MergeChunks(
			ScavengePoint scavengePoint,
			IScavengeStateForChunkMerger state,
			ITFChunkScavengerLog scavengerLogger,
			CancellationToken cancellationToken) {

			_logger.Debug("SCAVENGING: Started new scavenge chunk merging phase for {scavengePoint}",
				scavengePoint.GetName());

			var checkpoint = new ScavengeCheckpoint.MergingChunks(scavengePoint);
			state.SetCheckpoint(checkpoint);
			return MergeChunks(checkpoint, state, scavengerLogger, cancellationToken);
		}

		public ValueTask MergeChunks(
			ScavengeCheckpoint.MergingChunks checkpoint,
			IScavengeStateForChunkMerger state,
			ITFChunkScavengerLog scavengerLogger,
			CancellationToken cancellationToken) {

			if (_mergeChunks) {
				_logger.Debug("SCAVENGING: Merging chunks from checkpoint: {checkpoint}", checkpoint);
				return _backend.MergeChunks(scavengerLogger, _throttle, cancellationToken);
			}

			_logger.Debug("SCAVENGING: Merging chunks is disabled");
			return ValueTask.CompletedTask;
		}
	}
}
