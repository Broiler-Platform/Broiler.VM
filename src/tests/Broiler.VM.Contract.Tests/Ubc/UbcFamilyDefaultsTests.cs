using System.Text;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The four family members universal bytecode contract version 2 adds with defaults, reached the way an
/// emitter reaches them - through a type argument, on a family that does not override them - and the two
/// answers they speak in.
/// </summary>
/// <remarks>
/// The executor paths that act on the answers belong to the bytecode emitter, which no test project may
/// reference; the fixture composition runs them over a probe family. What is held here is what every
/// family that overrides nothing gets: entry points answered from the Entries section exactly as the
/// verified program's own lookup answers them, every instance ready, no start unit and nothing to
/// release - and answers whose default value is no answer at all.
/// </remarks>
public sealed class UbcFamilyDefaultsTests
{
    private static UbcEntryAnswer Resolve<TFamily>(UbcVerifiedProgram program, string name)
        where TFamily : struct, IUbcFamily =>
        TFamily.ResolveEntry(new object(), program, Encoding.UTF8.GetBytes(name));

    private static UbcInstanceAnswer Admit<TFamily>()
        where TFamily : struct, IUbcFamily =>
        TFamily.AdmitInstance(new object());

    private static int Start<TFamily>()
        where TFamily : struct, IUbcFamily =>
        TFamily.StartUnit(new object());

    private static void Abandon<TFamily>()
        where TFamily : struct, IUbcFamily =>
        TFamily.AbandonInstance(new object());

    private static UbcVerifiedProgram EntriesControl()
    {
        var (_, spec, configuration) = UbcCorpus.Controls().Single(static control => control.Id == "control-entries-and-positions");
        return Assert.IsType<UbcVerifiedProgram>(UbcCorpusRunner.Run(spec.Bytes(), configuration).Outcome.State);
    }

    [Fact]
    public void The_Default_Entry_Resolution_Answers_What_The_Entries_Section_Answers()
    {
        var program = EntriesControl();

        foreach (var name in new[] { "main", "second", "grüße", "absent", string.Empty, "mai" })
        {
            var answer = Resolve<UbcCorpusHandlers>(program, name);

            if (program.TryGetEntry(Encoding.UTF8.GetBytes(name), out var unit))
            {
                Assert.Equal(UbcEntryAnswerKind.Found, answer.Kind);
                Assert.Equal(unit, answer.Unit);
            }
            else
            {
                Assert.Equal(UbcEntryAnswerKind.Missing, answer.Kind);
            }

            Assert.Null(answer.Fault);
        }

        // Non-vacuous: the control's three entries name two units, so both answers were given.
        Assert.Equal(0, Resolve<UbcCorpusHandlers>(program, "main").Unit);
        Assert.Equal(1, Resolve<UbcCorpusHandlers>(program, "grüße").Unit);
        Assert.Equal(UbcEntryAnswerKind.Missing, Resolve<UbcCorpusHandlers>(program, "absent").Kind);
    }

    [Fact]
    public void The_Default_Instance_Members_Admit_Every_Instance_Name_No_Start_Unit_And_Release_Nothing()
    {
        var admitted = Admit<UbcCorpusHandlers>();

        Assert.Equal(UbcInstanceAnswerKind.Ready, admitted.Kind);
        Assert.Null(admitted.Fault);
        Assert.Equal(-1, Start<UbcCorpusHandlers>());

        // The corpus family's own members all throw, so a default that forwarded to one would too.
        Abandon<UbcCorpusHandlers>();
    }

    [Fact]
    public void A_Default_Answer_Is_No_Answer_Rather_Than_A_Success()
    {
        Assert.Equal(UbcEntryAnswerKind.Unanswered, default(UbcEntryAnswer).Kind);
        Assert.Equal(UbcInstanceAnswerKind.Unanswered, default(UbcInstanceAnswer).Kind);
        Assert.Equal(0, (int)UbcEntryAnswerKind.Unanswered);
        Assert.Equal(0, (int)UbcInstanceAnswerKind.Unanswered);
    }

    [Fact]
    public void Each_Answer_Carries_What_Its_Kind_Names_And_Nothing_Else()
    {
        var fault = new TestPayload();

        Assert.Equal((UbcEntryAnswerKind.Found, 7, (IVmProfilePayload?)null), Parts(UbcEntryAnswer.Found(7)));
        Assert.Equal((UbcEntryAnswerKind.Missing, -1, (IVmProfilePayload?)null), Parts(UbcEntryAnswer.Missing));
        Assert.Equal((UbcEntryAnswerKind.Refused, -1, (IVmProfilePayload?)fault), Parts(UbcEntryAnswer.Refused(fault)));

        Assert.Equal((UbcInstanceAnswerKind.Ready, (IVmProfilePayload?)null), (UbcInstanceAnswer.Ready.Kind, UbcInstanceAnswer.Ready.Fault));
        Assert.Equal((UbcInstanceAnswerKind.Exhausted, (IVmProfilePayload?)null), (UbcInstanceAnswer.Exhausted.Kind, UbcInstanceAnswer.Exhausted.Fault));
        Assert.Equal((UbcInstanceAnswerKind.Faulted, (IVmProfilePayload?)fault), (UbcInstanceAnswer.Faulted(fault).Kind, UbcInstanceAnswer.Faulted(fault).Fault));

        // A refusal or a fault without the payload it answers with cannot be built.
        Assert.Throws<ArgumentNullException>(() => UbcEntryAnswer.Refused(null!));
        Assert.Throws<ArgumentNullException>(() => UbcInstanceAnswer.Faulted(null!));

        static (UbcEntryAnswerKind, int, IVmProfilePayload?) Parts(UbcEntryAnswer answer) => (answer.Kind, answer.Unit, answer.Fault);
    }

    private sealed class TestPayload : IVmProfilePayload
    {
        public VmPayloadIdentity Identity { get; } = new(UbcCorpusFamily.ProfileId, 901, 1);
    }
}
