// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// Answers the template object of the site at <paramref name="site"/>, building it the first
    /// time the site is evaluated.
    /// </summary>
    /// <remarks>
    /// <b>GetTemplateObject as the specification writes it</b>: a cooked Array and a raw Array of
    /// the site's chunks, <c>raw</c> defined on the first with every attribute off, and both frozen.
    /// Nothing here reads a global, so a guest that replaced <c>Object.freeze</c> or
    /// <c>Object.defineProperty</c> sees no call and cannot change the object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E52C8E
    // Broiler-Falsified-If: two evaluations of one site answer different objects, two sites answer one object, or the answer is observably not frozen
    // Broiler-Human:        PENDING
    private JsObject TemplateObject(JsProgram program, int site, JsValue[] stack, int at, int count) =>
        TemplateObject(
            program,
            site,
            new ReadOnlySpan<JsValue>(stack, at, count),
            new ReadOnlySpan<JsValue>(stack, at + count, count));

    /// <summary>
    /// Answers the template object of the site at <paramref name="site"/> from its cooked and raw
    /// chunks, building it the first time the site is evaluated.
    /// </summary>
    /// <remarks>
    /// The one construction both <see cref="JsOpcode.GetTemplateObject"/> and
    /// <see cref="JsOpcode.GetTemplateObjectWide"/> reach, so a site's object is the same thing
    /// whichever width its count needed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=21AFAD
    // Broiler-Falsified-If: two evaluations of one site answer different objects, two sites answer one object, or the answer is observably not frozen
    // Broiler-Human:        PENDING
    private JsObject TemplateObject(JsProgram program, int site, ReadOnlySpan<JsValue> cookedChunks, ReadOnlySpan<JsValue> rawChunks)
    {
        var registry = templates.GetOrCreateValue(program);

        if (registry.TryGetValue(site, out var existing))
        {
            return existing;
        }

        var cooked = new JsArray(Realm.ArrayPrototype);
        var raw = new JsArray(Realm.ArrayPrototype);

        for (var index = 0; index < cookedChunks.Length; index++)
        {
            Charge(1);
            cooked.Push(cookedChunks[index]);
            raw.Push(rawChunks[index]);
        }

        JsRealm.ObjectSetIntegrity(this, raw, freeze: true);
        cooked.SetOwnProperty("raw", JsProperty.Data(JsValue.Object(raw), JsPropertyAttributes.None));
        JsRealm.ObjectSetIntegrity(this, cooked, freeze: true);
        registry[site] = cooked;
        return cooked;
    }
}
