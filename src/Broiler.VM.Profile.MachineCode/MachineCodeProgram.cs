// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           7
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=10E71F
// Broiler-Human:        PENDING
public sealed class MachineCodeProgram : IVmVerifiedState
{
    public VmNativeArchitecture Architecture { get; }
    public byte[] Code { get; }
    public MachineCodeSymbol[] Symbols { get; }
    public MachineCodeEntry[] Entries { get; }
    public double[] Constants { get; }
    public MachineCodeLimits Limits { get; }

    public MachineCodeProgram(
        VmNativeArchitecture architecture,
        byte[] code,
        MachineCodeSymbol[] symbols,
        MachineCodeEntry[] entries,
        double[] constants,
        MachineCodeLimits limits)
    {
        Architecture = architecture;
        Code = code;
        Symbols = symbols;
        Entries = entries;
        Constants = constants;
        Limits = limits;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E52521
    // Broiler-Human:        PENDING
    public bool TryFindEntry(string name, out MachineCodeEntry entry)
    {
        for (var i = 0; i < Entries.Length; i++)
        {
            if (string.Equals(Entries[i].Name, name, System.StringComparison.Ordinal))
            {
                entry = Entries[i];
                return true;
            }
        }

        entry = default;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=910734
    // Broiler-Human:        PENDING
    public bool TryFindSymbol(uint functionIndex, out MachineCodeSymbol symbol)
    {
        for (var i = 0; i < Symbols.Length; i++)
        {
            if (Symbols[i].FunctionIndex == functionIndex)
            {
                symbol = Symbols[i];
                return true;
            }
        }

        symbol = default;
        return false;
    }
}
