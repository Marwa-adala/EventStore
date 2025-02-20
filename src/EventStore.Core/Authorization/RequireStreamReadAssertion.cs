// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EventStore.Plugins.Authorization;

namespace EventStore.Core.Authorization {
	public class RequireStreamReadAssertion : IAssertion {
		private static readonly Operation StreamRead = new Operation(Operations.Streams.Read);
		private readonly IStreamPermissionAssertion _streamAssertion;

		public RequireStreamReadAssertion(IStreamPermissionAssertion streamAssertion) {
			_streamAssertion = streamAssertion;
			Information = streamAssertion.Information;
		}

		public AssertionInformation Information { get; }
		public Grant Grant { get; } = Grant.Unknown;

		public ValueTask<bool> Evaluate(ClaimsPrincipal cp, Operation operation, PolicyInformation policy,
			EvaluationContext context) {
			if (operation == Operations.Subscriptions.ProcessMessages ||
			    operation == Operations.Subscriptions.ReplayParked) {
				var stream = FindStreamId(operation.Parameters.Span);
				return _streamAssertion.Evaluate(cp,
					StreamRead.WithParameter(Operations.Streams.Parameters.StreamId(stream)), policy, context);
			}

			return new ValueTask<bool>(false);
		}

		private string FindStreamId(ReadOnlySpan<Parameter> parameters) {
			for (int i = 0; i < parameters.Length; i++)
				if (parameters[i].Name == "streamId")
					return parameters[i].Value;

			return null;
		}
	}
}
