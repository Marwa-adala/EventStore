// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EventStore.Core.Tests {
	public class SpecificationWithFilePerTestFixture {
		protected string Filename;

		[OneTimeSetUp]
		public virtual Task TestFixtureSetUp() {
			var typeName = GetType().Name.Length > 30 ? GetType().Name.Substring(0, 30) : GetType().Name;
			Filename = Path.Combine(Path.GetTempPath(), string.Format("ES-{0}-{1}", Guid.NewGuid(), typeName));
			return Task.CompletedTask;
		}

		[OneTimeTearDown]
		public virtual void TestFixtureTearDown() {
			if (File.Exists(Filename))
				File.Delete(Filename);
		}
	}
}
