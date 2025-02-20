// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using EventStore.Core.Index.Hashes;
using NUnit.Framework;

namespace EventStore.Core.Tests.Hashes {
	[TestFixture]
	public class xxhash_hash_should {
		// calculated from reference XXhash implementation at http://code.google.com/p/xxhash/
		public static uint XXHashReferenceVerificationValue = 0x56D249B1;

		[Test]
		public void pass_smhasher_verification_test() {
			Assert.IsTrue(SMHasher.VerificationTest(new XXHashUnsafe(), XXHashReferenceVerificationValue));
		}

		[Test, Category("LongRunning"), Explicit]
		public void pass_smhasher_sanity_test() {
			Assert.IsTrue(SMHasher.SanityTest(new XXHashUnsafe()));
		}
	}
}
