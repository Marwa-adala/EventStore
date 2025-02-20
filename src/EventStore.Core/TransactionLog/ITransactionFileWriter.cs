// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;
using EventStore.Core.TransactionLog.LogRecords;

namespace EventStore.Core.TransactionLog {
	public interface ITransactionFileWriter : IDisposable {
		void Open();
		bool CanWrite(int numBytes);
		bool Write(ILogRecord record, out long newPos);
		void OpenTransaction();
		void WriteToTransaction(ILogRecord record, out long newPos);
		bool TryWriteToTransaction(ILogRecord record, out long newPos);
		void CommitTransaction();
		bool HasOpenTransaction();
		void Flush();
		void Close();

		long Position { get; }
		long FlushedPosition { get; }
	}
}
