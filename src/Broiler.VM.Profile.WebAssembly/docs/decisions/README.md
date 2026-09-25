# Broiler.VM.Profile.WebAssembly decision records

The WebAssembly profile's own dated decisions, numbered `WAD-nnnn`. They are a **separate series from
the core's ADRs and from every other profile's series**, and none numbers into another: a `WAD` record
decides something about this profile, an `ADR` decides something about the Broiler.VM core, and where a
decision needs both halves each record carries its own and names the other.

The series was minted on 2026-09-25 by the universal bytecode programme's milestone UBC-4, whose work
package UBC-4.4 asks for the memory decision "in its own decision series". Until then
[the corrections file](../roadmap.corrections.md) said this profile had none, and the nine-row value,
store and frame routes lived in a source comment; [WAD-0003](0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)
gives them this home.

The [roadmap](../roadmap.md) states planned work and objective exit gates. The
[status ledger](../roadmap.status.md) is the authority for what has been accepted. **A decision recorded
here is not evidence that it was implemented**, and a record whose subject the checkout does not contain
says so in its own text.

| Record | Decides | Milestone |
|---|---|---|
| [WAD-0001](0001-the-memory-representation.md) | That a linear memory is a pinned managed byte array, reallocated on a successful growth with its base republished; that a growth invalidates every view; what the base's stability means for a form that addresses it; and what the route keeps | none in the `WA-` series; programme milestone UBC-4, decision UBC-D-2 |
| [WAD-0002](0002-the-family-table-stays-under-the-slice-identity.md) | That the WebAssembly family's instruction table is selected by `broiler.webassembly.slice`, that `broiler.webassembly.numeric1` is not minted, and the recorded consequence that the table admits more than the identity's definition | none in the `WA-` series; programme milestone UBC-4, decision UBC-D-3 |
| [WAD-0003](0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md) | Where each of the nine value, store and frame routes stands once the universal bytecode executes this profile's modules: which carry over, which the universal bytecode now answers, and which is reversed | none in the `WA-` series; programme milestone UBC-4 |

## What a record must carry

Each record states its **status**, its **date**, its **owner** and any **co-signer**, the **decision**,
what it **rejects and why**, and - where the decision is provisional - the named condition that would
settle it and the milestone that owns settling it. A decision with no recorded rejection is a decision
nobody chose between.

**Where owner and co-signer are the same person, the record says so in those words.** Every role this
series names is held by one person on the date the series was minted, and no record claims its
co-signature is independent.

No record holds a figure. A limit this profile declares - a page ceiling, a table ceiling - is named by
the member that declares it, and its value is read from the code.
