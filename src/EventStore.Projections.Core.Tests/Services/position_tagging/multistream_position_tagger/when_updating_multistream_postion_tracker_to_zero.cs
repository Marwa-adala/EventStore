// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using EventStore.Core.Data;
using EventStore.Projections.Core.Services.Processing;
using EventStore.Projections.Core.Services.Processing.Checkpointing;
using EventStore.Projections.Core.Services.Processing.MultiStream;
using NUnit.Framework;

namespace EventStore.Projections.Core.Tests.Services.position_tagging.multistream_position_tagger {
	[TestFixture]
	public class when_updating_multistream_postion_tracker_to_zero {
		private MultiStreamPositionTagger _tagger;
		private PositionTracker _positionTracker;

		[SetUp]
		public void When() {
			_tagger = new MultiStreamPositionTagger(0, new[] {"stream1", "stream2"});
			_positionTracker = new PositionTracker(_tagger);
			// when 

			_positionTracker.UpdateByCheckpointTagInitial(_tagger.MakeZeroCheckpointTag());
		}

		[Test]
		public void streams_are_set_up() {
			Assert.Contains("stream1", _positionTracker.LastTag.Streams.Keys);
			Assert.Contains("stream2", _positionTracker.LastTag.Streams.Keys);
		}

		[Test]
		public void stream_position_is_minus_one() {
			Assert.AreEqual(ExpectedVersion.NoStream, _positionTracker.LastTag.Streams["stream1"]);
			Assert.AreEqual(ExpectedVersion.NoStream, _positionTracker.LastTag.Streams["stream2"]);
		}
	}
}
