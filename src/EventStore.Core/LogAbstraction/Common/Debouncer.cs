// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventStore.Core.LogAbstraction.Common {
	/// Executes the func after each interval, but only if it has been triggered during the interval.
	/// The func should handle its own exceptions.
	public class Debouncer {
		private readonly TimeSpan _interval;
		private readonly Func<CancellationToken, Task> _func;
		private readonly CancellationToken _token;
		private readonly Task _task;
		private readonly ManualResetEventSlim _mres = new();

		public Debouncer(
			TimeSpan interval,
			Func<CancellationToken, Task> func,
			CancellationToken token) {

			_interval = interval;
			_func = func;
			_token = token;
			_task = RunAsync();
		}

		public void Trigger() {
			_mres.Set();
		}

		async Task RunAsync() {
			while (true) {
				await Task.Delay(_interval, _token);
				if (_mres.IsSet) {
					_mres.Reset();
					await _func(_token);
				}
			}
		}
	}
}

