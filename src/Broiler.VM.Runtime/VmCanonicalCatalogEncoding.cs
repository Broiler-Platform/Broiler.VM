namespace Broiler.VM;

/// <summary>
/// The canonical byte encoding of a catalog, which is its identity.
/// </summary>
/// <remarks>
/// <para>
/// Entries are walked in ascending ordinal identity order and each is emitted in descriptor-row
/// order: strings as their UTF-8 bytes preceded by a four-byte little-endian length, integers as
/// four little-endian bytes, declarations as one byte, and sets preceded by a four-byte
/// little-endian count. There are no field names, separators, whitespace or culture anywhere in it.
/// </para>
/// <para>
/// <strong>What is included, and why the rest is not.</strong> The encoding carries identity,
/// support ranges, both contract integers, the representation and lifetime kinds, the three
/// capability declarations, the payload kind range, conformance and diagnostics identity, the
/// package ID, sharing, fault recovery, and the two work-unit declarations. It excludes the verifier
/// instance and the factory delegate, which have no stable bytes; the thread affinity and
/// concurrent-verification flags, whose changes are already carried by the descriptor revision; the
/// limits, matrix and capability imports, which are tunable or host-overridable policy; and the
/// package version and owner tag, which are inert metadata. A host retuning a limit has not changed
/// what the catalog <em>is</em>, and an encoding that said otherwise would make drift detection
/// noisy enough to be ignored.
/// </para>
/// </remarks>
internal static class VmCanonicalCatalogEncoding
{
    internal static byte[] Encode(System.Collections.Generic.IReadOnlyList<VmProfileDescriptor> orderedEntries)
    {
        var buffer = new System.Collections.Generic.List<byte>(256);

        WriteInt32(buffer, orderedEntries.Count);

        foreach (var descriptor in orderedEntries)
        {
            WriteString(buffer, descriptor.ProfileId.ToString());                       // row 1
            WriteString(buffer, descriptor.DisplayName);                                // row 2
            WriteInt32(buffer, descriptor.DescriptorRevision);                           // row 3
            WriteUInt32(buffer, descriptor.SupportedFormatVersions.Min);                 // row 4
            WriteUInt32(buffer, descriptor.SupportedFormatVersions.Max);

            var manifests = descriptor.AcceptedFeatureManifests;                         // row 5
            WriteInt32(buffer, manifests.IsDefault ? 0 : manifests.Length);

            if (!manifests.IsDefault)
            {
                // Normalized before encoding rather than trusted to arrive sorted: the ordering
                // rule is what makes declaration order unobservable, so it is applied here rather
                // than assumed of the caller.
                var sorted = new VmFeatureManifestId[manifests.Length];

                for (var index = 0; index < manifests.Length; index++)
                {
                    sorted[index] = manifests[index];
                }

                System.Array.Sort(sorted, static (left, right) => left.CompareTo(right));

                foreach (var manifest in sorted)
                {
                    WriteString(buffer, manifest.ToString());
                }
            }

            buffer.Add((byte)descriptor.ArtifactRepresentationKind);                     // row 8
            buffer.Add((byte)descriptor.ArtifactLifetimeKind);                           // row 9
            buffer.Add((byte)descriptor.GuestInitiatedLoads.Kind);                       // row 18
            WriteInt32(buffer, descriptor.GuestInitiatedLoads.MinimumProviderCapabilityVersion);
            buffer.Add((byte)descriptor.AsynchronousInstantiation);                      // row 19
            buffer.Add((byte)descriptor.ExternalSuspension);                             // row 20
            WriteInt32(buffer, descriptor.PayloadKindIdRange.MinInclusive);              // row 21
            WriteInt32(buffer, descriptor.PayloadKindIdRange.MaxInclusive);
            WriteInt32(buffer, descriptor.BuiltAgainstCoreContractVersion);              // row 22
            WriteInt32(buffer, descriptor.AuthoredCoreContractVersion);                  // row 23
            WriteString(buffer, descriptor.ConformanceManifestId.ToString());            // row 24
            WriteInt32(buffer, descriptor.ConformanceManifestVersion);
            WriteString(buffer, descriptor.DiagnosticsIdentity.ToString());              // row 25
            WriteString(buffer, descriptor.PackageIdentity.PackageId);                   // row 26, ID only
            buffer.Add((byte)descriptor.ArtifactSharing);                                // row 27
            buffer.Add((byte)descriptor.FaultRecovery);                                  // row 28
            WriteUInt32(buffer, descriptor.MaxUnchargedWork);                            // row 29
            WriteUInt32(buffer, descriptor.ChargingGranularity);                         // row 30
        }

        return buffer.ToArray();
    }

    private static void WriteString(System.Collections.Generic.List<byte> buffer, string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value ?? string.Empty);
        WriteInt32(buffer, bytes.Length);
        buffer.AddRange(bytes);
    }

    private static void WriteInt32(System.Collections.Generic.List<byte> buffer, int value) =>
        WriteUInt32(buffer, unchecked((uint)value));

    private static void WriteUInt32(System.Collections.Generic.List<byte> buffer, uint value)
    {
        buffer.Add((byte)(value & 0xFF));
        buffer.Add((byte)((value >> 8) & 0xFF));
        buffer.Add((byte)((value >> 16) & 0xFF));
        buffer.Add((byte)((value >> 24) & 0xFF));
    }
}
