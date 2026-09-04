// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using Xunit;

// Windows lets only one partition per process own guest memory, so two test classes building
// machines at the same time would fight over that one slot. The suite is fast enough that running
// it serially costs nothing.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
