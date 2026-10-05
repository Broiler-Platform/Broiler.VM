<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Segments of text, checked against ICU

This is the dataset slice I4's `Intl.Segmenter` record keeps, decision
[JSD-0050](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0050-intl-segmenter.md). The
boundaries themselves are also checked against Unicode's own conformance files, which the slice
compiler runs from the pinned UCD. This dataset holds what those files do not: whole texts, the
word-like flag, and the objects ECMA-402 wraps the boundaries in.

- [`segments.js`](segments.js) is the program. It segments 17 texts by grapheme, word and sentence,
  under `en` and `de`, and writes every segment with its index, and for words whether it is
  word-like. The texts are:
  - German and English prose, with abbreviations, numbers, quotes and apostrophes;
  - an address, a URL and line ends;
  - emoji sequences and flags;
  - combining marks and a Devanagari conjunct, Hangul jamo and syllables;
  - Japanese, Hebrew and lone surrogates;
  - the empty string.

  Ten last lines answer `containing` at twelve indices, the iterator, the defaults and tags, a number
  as input, the errors ECMA-402 states and `supportedLocalesOf`. It uses no host function, and it
  completes with its lines, so Node and the profile run the same text.
- [`segments.icu-77.1.txt`](segments.icu-77.1.txt) is Node's answer, retained as written. It comes
  from Node 22.22.0, whose `process.versions` reports ICU 77.1 and Unicode 16.0. It was produced on
  2026-10-04 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('segments.js','utf8'))+'\n')" > segments.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 2 of its 118 lines on which the profile answers
  otherwise, in one group: the Japanese text by words, which ICU joins by its dictionary and UAX #29
  leaves one ideograph to a word. Each line is the profile's answer.

The slice compiler's check `intl/i4/segments-match-icu` runs the same program under the profile and
compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** ICU 77.1 implements Unicode 16.0's rules, and the
profile Unicode 17.0's, which the conformance files of the pinned UCD check. ICU is a second
implementation, not the specification. ECMA-402 leaves the boundaries to the implementation and names
UAX #29's defaults, which the profile follows without a dictionary.

**Re-running.** If `segments.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and the reason is checked again.
