namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The witness input for rule B5c, compiled into the test assembly so that the same metadata
/// scanner that clears every shipping assembly can be shown flagging a real violation.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is never called, and its ImplMap row is the entire point.</b> A declaration is what rule
/// B5c reads: a method definition of this assembly carrying <c>PinvokeImpl</c>, whose import names
/// a module and an entry point. Nothing here executes, and nothing here would work if it did -
/// <c>libc</c> is not on the machine this suite usually runs on, which is exactly why a rule about
/// what an image CAN do reads the declaration rather than watching a call.
/// </para>
/// <para>
/// <b>THE TWO SPELLINGS ARE WITNESSED IN TWO PLACES, AND THAT IS DELIBERATE RATHER THAN A GAP.</b>
/// This one is the older <c>[DllImport]</c>, which needs no unsafe block and so needs no change to
/// this project. The generated <c>[LibraryImport]</c> spelling is witnessed by the checkout itself:
/// the arming path is written that way, and B5c's clean direction asserts that the arming
/// assembly's import table carries both platforms' protection entry points before it reads an
/// empty answer about anything else. So the claim that the two spellings are one reading is held
/// by the suite from both ends - a real generated import that must be seen, and a hand-written one
/// that must be reported.
/// </para>
/// <para>
/// If B5c ever stops flagging this type, B5c has stopped working, and
/// <see cref="NativeMappingRuleTests"/> fails rather than passing quietly.
/// </para>
/// </remarks>
internal static class NativeMappingWitness
{
    [System.Runtime.InteropServices.DllImport(
        "libc", EntryPoint = "mprotect", SetLastError = true)]
    internal static extern int ArmPage(nint address, nuint length, int protect);
}
