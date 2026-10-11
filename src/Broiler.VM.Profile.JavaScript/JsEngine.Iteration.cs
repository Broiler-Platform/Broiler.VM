// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           0
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  5/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- the iteration protocol ----------------------------------------------------------------

    /// <summary>
    /// The abstract operation <c>GetIterator</c>: the guest's own protocol, driven from here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here shortcuts the protocol for a value this engine happens to recognise.</b>
    /// Spreading an Array calls <c>Array.prototype[Symbol.iterator]</c> and then calls the
    /// <c>next</c> that answered, every time - because a guest may replace either, and a program
    /// that does is entitled to see its own function run. A fast path over the dense elements would
    /// be faster and would answer the wrong thing for exactly the programs that would notice.
    /// </para>
    /// <para>
    /// <b>It reads <c>Symbol.iterator</c> and not a <c>length</c>.</b> An array-like and an
    /// iterable are different things, and a construct that fell back to indices when the Symbol was
    /// absent would iterate a plain object that happened to have a <c>length</c> — which the
    /// language refuses, loudly, and for good reason.
    /// </para>
    /// <para>
    /// <b>One helper for every construct that iterates</b> — spread, <c>for … of</c>, array
    /// destructuring, and any built-in taking an iterable. Each of those is the same three steps
    /// with a different thing done to the value, and writing them separately is how three of them
    /// end up agreeing and the fourth does not.
    /// </para>
    /// <para>
    /// <c>next</c> is read ONCE, here, and the record keeps it. The specification reads it at
    /// <c>GetIterator</c> and calls that same function at every step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8BAE7A
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIterator(JsValue iterable)
    {
        Charge(4);

        if (iterable.IsNullish)
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        if (!TryGetSymbolMethod(iterable, Realm.IteratorSymbol, out var method))
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        return GetIteratorFromMethod(iterable, method);
    }

    /// <summary>
    /// The specification's <c>GetIteratorFromMethod</c>: an iterator record from a
    /// <c>Symbol.iterator</c> method the caller has already read.
    /// </summary>
    /// <remarks>
    /// <b>It exists for the callers that have to read the method BEFORE deciding to iterate.</b>
    /// A typed array constructor asks <c>GetMethod(argument, @@iterator)</c> to choose between the
    /// iteration protocol and the array-like reading; reading the method a second time through
    /// <see cref="GetIterator"/> would run a getter twice that the language runs once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F56B36
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIteratorFromMethod(JsValue iterable, JsValue method)
    {
        var iterator = Call(method, iterable, []);

        if (!iterator.IsObject)
        {
            ThrowTypeError("The result of the iterator method is not an object");
        }

        return new JsIteratorRecord(iterator, GetProperty(iterator, "next"));
    }

    /// <summary>
    /// One step of <c>IteratorStep</c>: answers the next value, or that the iterator is finished.
    /// </summary>
    /// <remarks>
    /// <b>A record whose <c>next</c> threw is marked done before the exception leaves.</b> The
    /// specification does not close an iterator whose <c>next</c> failed - it has no reason to
    /// believe the object is in a state that can answer <c>return</c> - and the flag is what carries
    /// that decision to the <c>IterateClose</c> the lowering emits unconditionally.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=9E8B92
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(JsIteratorRecord record, out JsValue value) =>
        TryIterateNext(record, [], out value, out _);

    /// <summary>
    /// The same step, with the argument <c>next</c> is called with and the value a DONE step
    /// carried.
    /// </summary>
    /// <remarks>
    /// <b>Both extras exist for <c>yield*</c> and for nothing else, which is why they are an
    /// overload rather than a second walk over the protocol.</b> A delegation forwards what
    /// <c>gen.next(v)</c> sent to the inner iterator's own <c>next</c>, and what <c>yield*</c>
    /// EVALUATES TO is the value of the step that reported done - which every other construct
    /// discards, and which a helper that only answered "finished" could not give back. The
    /// argument list is passed through rather than built here so that the ordinary overload calls
    /// <c>next</c> with NO arguments, exactly as <c>IteratorNext</c> does when it has no value to
    /// send: an iterator written in the guest can see the difference in its own
    /// <c>arguments.length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=91005A
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(JsIteratorRecord record, JsValue[] sent, out JsValue value, out JsValue completed, bool wantsCompleted = false)
    {
        value = JsValue.Undefined;
        completed = JsValue.Undefined;

        if (record.Done)
        {
            return false;
        }

        Charge(2);
        JsValue result;

        try
        {
            result = Call(record.Next, record.Iterator, sent);
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        if (!result.IsObject)
        {
            record.Done = true;
            ThrowTypeError("Iterator result " + Describe(result) + " is not an object");
        }

        // THE `value` OF A DONE RESULT IS READ ONLY BY A CALLER THAT WANTS IT, which is one: the
        // `yield*` delegation, whose own value is the inner iterator's return value. Reading it for
        // everybody is observable through a getter - the pinned suite's set-like iterators count
        // exactly these reads - and a `for … of` that read it would be asking a question the
        // language does not ask.
        //
        // A THROWING `done` OR `value` GETTER MARKS THE RECORD DONE AS A THROWING `next` DOES.
        // `IteratorComplete` and `IteratorValue` are part of the step, and a step that failed
        // leaves the iterator unclosed: a `for … of` that went on to call `return` would run guest
        // code the language never runs there.
        try
        {
            if (GetProperty(result, "done").ToBooleanValue())
            {
                record.Done = true;

                if (wantsCompleted)
                {
                    completed = GetProperty(result, "value");
                }

                return false;
            }

            value = GetProperty(result, "value");
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        return true;
    }

    /// <summary>
    /// <c>IteratorClose</c> under a normal or a <c>break</c>-shaped completion.
    /// </summary>
    /// <remarks>
    /// Errors from <c>return</c> propagate here, and a <c>return</c> answering a non-object is
    /// itself a <c>TypeError</c> - both of which a guest can observe and both of which a quiet close
    /// would swallow.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5F7DA1
    // Broiler-Human:        PENDING
    internal void CloseIterator(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;
        var method = GetProperty(record.Iterator, "return");

        if (method.IsNullish)
        {
            return;
        }

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            ThrowTypeError("The iterator's return is not a function");
        }

        var result = Call(method, record.Iterator, []);

        if (!result.IsObject)
        {
            ThrowTypeError("The iterator's return answered " + Describe(result) + " and not an object");
        }
    }

    /// <summary>
    /// <c>IteratorClose</c> under a throw completion, which discards whatever <c>return</c> does.
    /// </summary>
    /// <remarks>
    /// <b>The exception already in flight is the one the program is owed.</b> A <c>for … of</c>
    /// body that throws still has to give the iterator its <c>return</c>, but replacing the body's
    /// exception with one the clean-up raised would report the second failure and lose the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C8F9C1
    // Broiler-Human:        PENDING
    internal void CloseIteratorQuietly(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;

        try
        {
            var method = GetProperty(record.Iterator, "return");

            if (method.IsObject && method.AsObject().IsCallable)
            {
                Call(method, record.Iterator, []);
            }
        }
        catch (JsThrow)
        {
            // Deliberately swallowed: see the remark.
        }
    }

    /// <summary>Drains an iterable into a list, closing nothing because it ran to completion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DA2505
    // Broiler-Human:        PENDING
    internal void IterateInto(JsValue iterable, List<JsValue> into)
    {
        var record = GetIterator(iterable);

        while (TryIterateNext(record, out var element))
        {
            into.Add(element);
        }
    }

    /// <summary>Drains a record that is already open, which is what a rest element takes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=AA5907
    // Broiler-Human:        PENDING
    internal JsArray DrainIterator(JsIteratorRecord record)
    {
        var rest = Realm.NewArray();

        while (TryIterateNext(record, out var element))
        {
            rest.Push(element);
        }

        return rest;
    }
}
