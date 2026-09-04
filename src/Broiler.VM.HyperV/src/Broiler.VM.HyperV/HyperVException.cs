// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Globalization;

namespace Broiler.VM.HyperV;

/// <summary>
/// Thrown when a Windows Hypervisor Platform call fails.
/// </summary>
public sealed class HyperVException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="HyperVException"/> class.</summary>
    public HyperVException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="HyperVException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    public HyperVException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="HyperVException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this one.</param>
    public HyperVException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private HyperVException(string message, string operation, int hresult)
        : base(message)
    {
        Operation = operation;
        HResult = hresult;
    }

    /// <summary>The name of the native entry point that failed, for example WHvMapGpaRange.</summary>
    public string? Operation { get; }

    /// <summary>Throws when <paramref name="hresult"/> is a failure code.</summary>
    /// <param name="hresult">The HRESULT returned by the native call.</param>
    /// <param name="operation">The native entry point that produced it.</param>
    /// <exception cref="HyperVException">The call failed.</exception>
    internal static void ThrowIfFailed(int hresult, string operation)
    {
        if (hresult >= 0)
        {
            return;
        }

        throw Create(hresult, operation);
    }

    internal static HyperVException Create(int hresult, string operation)
    {
        string message = string.Format(
            CultureInfo.InvariantCulture,
            "{0} failed with HRESULT 0x{1:X8}{2}.",
            operation,
            hresult,
            Explain(hresult) is { } hint ? " (" + hint + ")" : string.Empty);

        return new HyperVException(message, operation, hresult);
    }

    private static string? Explain(int hresult) => hresult switch
    {
        unchecked((int)0x80070005) => "access denied - the process is not permitted to create partitions",
        unchecked((int)0x80070057) => "invalid parameter",
        unchecked((int)0x8007000E) => "out of memory",
        unchecked((int)0x80070032) => "not supported - check that the Windows Hypervisor Platform feature is enabled",
        unchecked((int)0x80370100) => "the virtual machine or container operation is not valid in the current state",
        unchecked((int)0xC0350005) => "the hypervisor does not support the requested operation",

        // Windows names the memory block it creates for a partition after the owning process, so
        // the second partition in a process is refused the moment it asks for guest memory. It is
        // worth spelling out, because the raw code says "partition name" and the caller is looking
        // at a perfectly ordinary call to map RAM.
        unchecked((int)0xC0370008) => "only one partition per process may own guest memory on this "
            + "build of Windows - dispose the other PicoVm before creating this one, or host the "
            + "second machine in its own process",
        _ => null,
    };
}
