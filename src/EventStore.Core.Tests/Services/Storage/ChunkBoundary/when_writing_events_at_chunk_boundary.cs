// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using NUnit.Framework;

namespace EventStore.Core.Tests.Services.Storage.ChunkBoundary;

// Note: this test does not verify the behaviour of the StorageWriterService.
// It only verifies if TFChunkWriter works properly.

[TestFixture(typeof(LogFormat.V2), typeof(string))]
[TestFixture(typeof(LogFormat.V3), typeof(uint))]
public class when_writing_events_at_chunk_boundary<TLogFormat, TStreamId> : ReadIndexTestScenario<TLogFormat, TStreamId> {
	private long _writerChk;

	protected override void WriteTestScenario() {
		WriteSingleEvent("ES", 0, new string('.', 4000));
		WriteSingleEvent("ES", 1, new string('.', 4000));
		WriteSingleEvent("ES", 2, new string('.', 4000), retryOnFail: true); // chunk 1

		// verify that the writer is in the correct chunk
		var chunk = WriterCheckpoint.ReadNonFlushed() / Db.Config.ChunkSize;
		Assert.AreEqual(1, chunk);

		_writerChk = WriterCheckpoint.Read();
	}

	[Test]
	public void writer_checkpoint_is_flushed() {
		Assert.AreEqual(Db.Config.ChunkSize, _writerChk);
	}
}
