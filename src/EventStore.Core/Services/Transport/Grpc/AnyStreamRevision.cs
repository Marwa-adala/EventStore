// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;
using HashCode = EventStore.Core.Services.Transport.Common.HashCode;

namespace EventStore.Core.Services.Transport.Grpc {
	public struct AnyStreamRevision : IEquatable<AnyStreamRevision> {
		public static readonly AnyStreamRevision NoStream = new AnyStreamRevision(Constants.NoStream);
		public static readonly AnyStreamRevision Any = new AnyStreamRevision(Constants.Any);
		public static readonly AnyStreamRevision StreamExists = new AnyStreamRevision(Constants.StreamExists);
		private readonly int _value;

		private static class Constants {
			public const int NoStream = 1;
			public const int Any = 2;
			public const int StreamExists = 4;
		}

		public static AnyStreamRevision FromInt64(long value) => new AnyStreamRevision(-Convert.ToInt32(value));

		public AnyStreamRevision(int value) {
			switch (value) {
				case Constants.NoStream:
				case Constants.Any:
				case Constants.StreamExists:
					_value = value;
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(value));
			}
		}

		public readonly bool Equals(AnyStreamRevision other) => _value == other._value;
		public override readonly bool Equals(object obj) => obj is AnyStreamRevision other && Equals(other);
		public override readonly int GetHashCode() => HashCode.Hash.Combine(_value);
		public static bool operator ==(AnyStreamRevision left, AnyStreamRevision right) => left.Equals(right);
		public static bool operator !=(AnyStreamRevision left, AnyStreamRevision right) => !left.Equals(right);
		public readonly long ToInt64() => -Convert.ToInt64(_value);
		public static implicit operator int(AnyStreamRevision streamRevision) => streamRevision._value;

		public override string ToString() => _value switch {
			Constants.NoStream => nameof(NoStream),
			Constants.Any => nameof(Any),
			Constants.StreamExists => nameof(StreamExists),
			_ => _value.ToString()
		};
	}
}
